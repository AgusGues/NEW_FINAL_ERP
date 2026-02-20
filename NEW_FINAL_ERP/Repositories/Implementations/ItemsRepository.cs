using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using NEW_FINAL_ERP.Models;
using NEW_FINAL_ERP.Infrastructure;

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

        public async Task<IEnumerable<Items>> GetAll()
        {
            using var conn = new SqlConnection(_connString);
            var sql = @"
                        select i.ItemId,i.itemcode,i.itemname,u.unitname from Items i join unit u on i.Unit = u.unitid where i.isactive=1
                      ";
            return await conn.QueryAsync<Items>(sql);
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
