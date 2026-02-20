using Dapper;
using Microsoft.Data.SqlClient;
using NEW_FINAL_ERP.Infrastructure;
using NEW_FINAL_ERP.Models;

namespace NEW_FINAL_ERP.Repositories.Implementations
{
    public class ItemsUOMRepository : IItemsUOMRepository
    {
        private readonly string _connString;

        public ItemsUOMRepository(IConfiguration config)
        {
            _connString = config.GetConnectionString("DefaultConnection");
        }
        public async Task Delete(UnitOfWork uow, int id)
        {
            var sql = @"Update ItemUOM set IsActive = 0 where IsActive = 1 and ItemUOMId=@id";
            await uow.Conn.ExecuteAsync(sql, new {id},uow.Tx);
        }

        public async Task<IEnumerable<ItemsUom>> GetAll()
        {
            using var conn = new SqlConnection(_connString);
            var sql = @"select ItemUOMId,ItemId,UnitId,ConversionToBase,IsBase,IsDefaultSales,IsDefaultPurchase,Barcode,IsActive,isInternalBarcode from ItemUOM where IsActive=1";
            return await conn.QueryAsync<ItemsUom>(sql);
        }

        public async Task<ItemsUom?> GetById(int id)
        {
            using var conn = new SqlConnection(_connString);
            return await conn.QueryFirstOrDefaultAsync<ItemsUom>
                (
                "select ItemUOMId,ItemId,UnitId,ConversionToBase,IsBase,IsDefaultSales,IsDefaultPurchase,Barcode,IsActive,isInternalBarcode from ItemUOM where IsActive=1 and ItemUOMId=@id",
                new { id });
        }

        public async Task Insert(UnitOfWork uow, ItemsUom itemsuom)
        {
            var sql = @"
                      insert into ItemUOM(
                        ItemId,
                        UnitId,
                        ConversionToBase,
                        IsBase,
                        IsDefaultSales,
                        IsDefaultPurchase,
                        Barcode,
                        IsActive,
                        CreatedAt,
                        IsInternalBarcode
                        )
                        values
                        (
                        @ItemId,
                        @UnitId,
                        @ConversionToBase,
                        @IsBase,
                        @IsDefaultSales,
                        @IsDefaultPurchase,
                        @Barcode,
                        1,
                        getdate(),
                        1
                        )
                      ";

            await uow.Conn.ExecuteAsync(sql,itemsuom,uow.Tx);
        }

        public Task Update(UnitOfWork uow, ItemsUom itemsuom)
        {
            throw new NotImplementedException();
        }
    }
}
