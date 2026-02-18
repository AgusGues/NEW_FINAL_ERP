using Dapper;
using Microsoft.Data.SqlClient;
using NEW_FINAL_ERP.Infrastructure;
using NEW_FINAL_ERP.Models;
using NEW_FINAL_ERP.Repositories;
using NEW_FINAL_ERP.Repositories.Implementations;
using System.Data;

namespace NEW_FINAL_ERP.Services
{
    public class BranchService
    {
        private readonly string _connString;
        private readonly IBranchRepository _repo;

        public BranchService(IConfiguration config,IBranchRepository repo)
        {
            _connString = config.GetConnectionString("DefaultConnection")!;
            _repo = repo;
        }

        public Task<IEnumerable<Branch>> GetAll()  => _repo.GetAll();

        public Task<Branch?> GetById(int id) => _repo.GetById(id);

        public async Task Create(Branch branch)
        {
            using var uow = new UnitOfWork(new SqlConnection(_connString));

            try
            {
                branch.BranchCode = await GenerateCode(uow);

                await _repo.Insert(uow, branch);

                uow.Commit();
            }
            catch
            {
                uow.Rollback();
                throw;
            }
        }

        public async Task Update(Branch branch)
        {
            using var uow = new UnitOfWork(new SqlConnection(_connString));

            try
            {
                await _repo.Update(uow, branch);
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
                    EntityName = "TEST",
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