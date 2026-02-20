using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;
using NEW_FINAL_ERP.Infrastructure;
using NEW_FINAL_ERP.Models;
using NEW_FINAL_ERP.Repositories;

namespace NEW_FINAL_ERP.Services
{
    public class UnitService
    {
        private readonly string _connString;
        private readonly IUnitRepository _repo;

        public UnitService(IConfiguration config, IUnitRepository repo)
        {
            _connString = config.GetConnectionString("DefaultConnection");
            _repo = repo;
        }

        public Task<IEnumerable<Unit>>GetAll()=>_repo.GetAll();
        public Task<Unit?>GetById(int id) =>_repo.GetById(id);

        public async Task Create(Unit unit)
        {
            using var uow = new UnitOfWork(new SqlConnection(_connString));
            try
            {
                unit.UnitCode = await GenerateCode(uow);
                await _repo.Insert(uow, unit);
                uow.Commit();
            }
            catch
            {
                uow.Rollback();
                throw;
            }
        }

        public async Task Update(Unit unit)
        {
            using var uow = new UnitOfWork(new SqlConnection(_connString));
            try
            {
                await _repo.Update(uow, unit);
                uow.Commit();
            }
            catch
            {
                uow.Rollback();
                throw;
            }
        }

        public async Task Delete(int id)
        {
            using var uow = new UnitOfWork(new SqlConnection(_connString));
            try
            {
                await _repo.Delete(uow, id);
                uow.Commit();
            }
            catch 
            {
                uow.Rollback();
                throw;
            }
        }

        private async Task<string> GenerateCode(UnitOfWork uow)
        {
            var cmd = new CommandDefinition(
                "sp_NumberSequence_Generate",
                new
                {
                    CompanyId = 0,
                    EntityName = "UNT",
                    DocumentId = Guid.NewGuid(),
                    CommandId = Guid.NewGuid()
                },
                uow.Tx,
                commandType: CommandType.StoredProcedure);

            return await uow.Conn.ExecuteScalarAsync<string>(cmd)
                   ?? throw new Exception("Generate BranchCode gagal");
        }

    }
}
