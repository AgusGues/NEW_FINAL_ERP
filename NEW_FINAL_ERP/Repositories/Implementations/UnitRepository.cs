using Dapper;
using Microsoft.Data.SqlClient;
using NEW_FINAL_ERP.Infrastructure;
using NEW_FINAL_ERP.Models;

namespace NEW_FINAL_ERP.Repositories.Implementations
{
    public class UnitRepository : IUnitRepository
    {
        private readonly string _connString;

        public UnitRepository(IConfiguration config)
        {
            _connString = config.GetConnectionString("DefaultConnection");
        }
        public async Task Delete(UnitOfWork uow, int id)
        {
            var sql = @"Update unit set IsActive = 0 where UnitId=@id and IsActive = 1";
            await uow.Conn.ExecuteAsync(sql, new {id}, uow.Tx);
        }

        public async Task<IEnumerable<Unit>> GetAll()
        {
            using var conn = new SqlConnection(_connString);
            var sql = @"select UnitId,UnitCode,UnitName,Description from Unit where IsActive = 1";
            return await conn.QueryAsync<Unit>(sql);
        }

        public async Task<Unit?> GetById(int id)
        {
            using var conn = new SqlConnection(_connString);
            return await conn.QueryFirstOrDefaultAsync<Unit>(
                @"select UnitId, UnitCode, UnitName, Description from Unit where UnitId=@id and IsActive=1",
                new { id }
                );
        }

        public async Task Insert(UnitOfWork uow, Unit unit)
        {
            var sql = @"
                      insert into Unit
                      (UnitCode,UnitName,Description,IsActive)
                      VALUES
                      (@UnitCode,@UnitName,@Description,1)";

            await uow.Conn.ExecuteAsync (sql, unit, uow.Tx);
        }

        public async Task Update(UnitOfWork uow, Unit unit)
        {
            var sql = @"
                      update unit set UnitName = @UnitName, Description = @Description where IsActive = 1 and UnitId=@UnitId
                      ";
            await uow.Conn.ExecuteAsync(sql,unit, uow.Tx);
        }
    }
}
