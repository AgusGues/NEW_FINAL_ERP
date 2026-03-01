using Dapper;
using NEW_FINAL_ERP.DTo;
using NEW_FINAL_ERP.Infrastructure;
using NEW_FINAL_ERP.Models;
using NEW_FINAL_ERP.Repositories.Implementations;
using Microsoft.Data.SqlClient;

namespace NEW_FINAL_ERP.Repositories.Implementations
{
    public class ItemPriceRepository : IItemPriceRepository
    {
        private readonly string _connString;

        public ItemPriceRepository(IConfiguration config)
        {
            _connString = config.GetConnectionString("DefaultConnection");
        }

        public async Task<PagedResultItemPriceDto<ItemPriceListDto>> GetAll(string? search, int page, int pageSize)
        {
            using var conn = new SqlConnection(_connString);

            var where = @"
                WHERE ip.IsActive = 1 AND i.IsActive = 1 AND u.IsActive = 1
            ";

            if (!string.IsNullOrWhiteSpace(search))
                where += " AND (i.ItemCode LIKE '%' + @search + '%' OR i.ItemName LIKE '%' + @search + '%' OR ip.PriceType LIKE '%' + @search + '%')";

            var totalData = await conn.ExecuteScalarAsync<int>($@"
                SELECT COUNT(1) FROM ItemPrice ip
                JOIN Items i ON ip.ItemId = i.ItemId
                JOIN Unit u ON ip.UnitId = u.UnitId
                {where}", new { search });

            var offset = (page - 1) * pageSize;

            var data = await conn.QueryAsync<ItemPriceListDto>($@"
                                                                SELECT 
                                                                    ip.ItemPriceId,
                                                                    ip.ItemId,
                                                                    ip.UnitId,
                                                                    i.ItemCode,
                                                                    i.ItemName,
                                                                    u.UnitName,
                                                                    ip.PriceType,
                                                                    ip.Price,
                                                                    ip.EffectiveDate
                                                                FROM ItemPrice ip
                                                                JOIN Items i ON ip.ItemId = i.ItemId
                                                                JOIN Unit u ON ip.UnitId = u.UnitId
                                                                {where}
                                                                ORDER BY ip.ItemPriceId DESC
                                                                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY",
                                                                new { search, offset, pageSize });

            return new PagedResultItemPriceDto<ItemPriceListDto>
            {
                Data = data,
                Page = page,
                PageSize = pageSize,
                TotalData = totalData,
                TotalPages = (int)Math.Ceiling(totalData / (double)pageSize)
            };
        }

        public async Task<ItemPrice?> GetById(int id)
        {
            using var conn = new SqlConnection(_connString);
            return await conn.QueryFirstOrDefaultAsync<ItemPrice>(
                "SELECT * FROM ItemPrice WHERE IsActive = 1 AND ItemPriceId = @id",
                new { id });
        }

        public async Task Insert(UnitOfWork uow, ItemPrice entity)
        {
            await uow.Conn.ExecuteAsync(@"
                INSERT INTO ItemPrice (ItemId, UnitId, PriceType, Price, EffectiveDate, IsActive, CreatedAt)
                VALUES (@ItemId, @UnitId, @PriceType, @Price, @EffectiveDate, 1, GETDATE())",
                entity, uow.Tx);
        }

        public async Task Update(UnitOfWork uow, ItemPrice entity)
        {
            await uow.Conn.ExecuteAsync(@"
                UPDATE ItemPrice SET
                    ItemId=@ItemId, UnitId=@UnitId, PriceType=@PriceType,
                    Price=@Price, EffectiveDate=@EffectiveDate, UpdatedAt=GETDATE()
                WHERE ItemPriceId=@ItemPriceId", entity, uow.Tx);
        }

        public async Task Delete(UnitOfWork uow, int id)
        {
            await uow.Conn.ExecuteAsync(@"
                UPDATE ItemPrice SET IsActive=0, UpdatedAt=GETDATE()
                WHERE ItemPriceId=@id AND IsActive=1", new { id }, uow.Tx);
        }

        public async Task<IEnumerable<object>> SearchItemAsync(string term)
        {
            using var conn = new SqlConnection(_connString);
            return await conn.QueryAsync(@"
                SELECT TOP 10 ItemId as id, ItemName as text
                FROM Items WHERE IsActive=1 AND ItemName LIKE '%' + @term + '%'
                ORDER BY ItemName", new { term });
        }

        public async Task<IEnumerable<object>> SearchUnitAsync(string term)
        {
            using var conn = new SqlConnection(_connString);
            return await conn.QueryAsync(@"
                SELECT TOP 10 UnitId as id, UnitName as text
                FROM Unit WHERE IsActive=1 AND UnitName LIKE '%' + @term + '%'
                ORDER BY UnitName", new { term });
        }
    }
}