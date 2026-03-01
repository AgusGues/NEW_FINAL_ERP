using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using NEW_FINAL_ERP.Models;
using NEW_FINAL_ERP.Infrastructure;
using NEW_FINAL_ERP.DTo;

namespace NEW_FINAL_ERP.Repositories.Implementations
{
    public class ItemsRepository : IItemsRepository
    {
        private readonly string _connString;

        public ItemsRepository(IConfiguration config)
        {
            _connString = config.GetConnectionString("DefaultConnection");
        }
        public async Task Delete(UnitOfWork uow, int id)
        {
            var sql = @"Update Items set IsActive = 0 where ItemId=@id and IsActive = 1";
            await uow.Conn.ExecuteAsync(sql, new {id}, uow.Tx);
        }

        public async Task<PagedResultItemsDto<Items>> GetAll(string? search, int page, int pageSize)
        {
            using var conn = new SqlConnection(_connString);

            var where = "WHERE i.IsActive = 1";

            if (!string.IsNullOrWhiteSpace(search))
                where += " AND (i.ItemCode LIKE '%' + @search + '%' OR i.ItemName LIKE '%' + @search + '%')";

            var totalData = await conn.ExecuteScalarAsync<int>($@"
        SELECT COUNT(1)
        FROM Items i
        {where}", new { search });

            var offset = (page - 1) * pageSize;

            var data = await conn.QueryAsync<Items>($@"
        SELECT 
            i.ItemId,
            i.ItemCode,
            i.ItemName,
            i.Unit as UnitId,
            u.UnitName
        FROM Items i
        JOIN Unit u ON i.Unit = u.UnitId
        {where}
        ORDER BY i.ItemId DESC
        OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY",
                new { search, offset, pageSize });

            return new PagedResultItemsDto<Items>
            {
                Data = data,
                Page = page,
                PageSize = pageSize,
                TotalData = totalData,
                TotalPages = (int)Math.Ceiling(totalData / (double)pageSize),
                Search = search
            };
        }

        public async Task<Items?> GetById(int id)
        {
            using var conn = new SqlConnection(_connString);
            return await conn.QueryFirstOrDefaultAsync<Items>(
                                "select * from Items where ItemId=@id and IsActive=1",

                new { id });

        }

        public async Task Insert(UnitOfWork uow, Items items)
        {
            var sql = @"insert into Items
                      (ItemCode,ItemName,Unit,IsActive) 
                      values
                      (@ItemCode,@ItemName,@UnitId,1)";
            await uow.Conn.ExecuteAsync(sql,items,uow.Tx);
        }

        public async Task Update(UnitOfWork uow, Items items)
        {
            var sql = @"
                        update items set ItemName=@ItemName,
                        Unit = @UnitId where ItemId=@ItemId and IsActive=1
                      ";
            await uow.Conn.ExecuteAsync(sql,items,uow.Tx);
        }
    }
}
