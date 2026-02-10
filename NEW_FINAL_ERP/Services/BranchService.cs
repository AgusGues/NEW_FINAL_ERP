using Dapper;
using Microsoft.Data.SqlClient;
using NEW_FINAL_ERP.Infrastructure;
using NEW_FINAL_ERP.Models;
using NEW_FINAL_ERP.Repositories;
using System.Data;

namespace NEW_FINAL_ERP.Services
{
    public class BranchService
    {
        private readonly string _connString;
        private readonly IBranchRepository _repo;

        public BranchService(IConfiguration config, IBranchRepository repo)
        {
            _connString = config.GetConnectionString("DefaultConnection")!;
            _repo = repo;
        }

        public async Task<IEnumerable<Branch>> GetAll()
        {
            await using var conn = new SqlConnection(_connString);
            return await _repo.GetAll(conn);
        }

        public async Task Create(Branch branch)
        {
            using var uow = new UnitOfWork(new SqlConnection(_connString));

            try
            {
                branch.BranchCode = await GenerateCode(uow, "BR");

                await _repo.Insert(uow, branch);

                uow.Commit();
            }
            catch
            {
                uow.Rollback();
                throw;
            }
        }

        private async Task<string> GenerateCode(UnitOfWork uow, string entity)
        {
            var cmd = new CommandDefinition(
                "sp_NumberSequence_Generate",
                new
                {
                    CompanyId = Guid.Empty,
                    EntityName = entity,
                    DocumentId = Guid.NewGuid(),
                    CommandId = Guid.NewGuid()
                },
                uow.Tx,
                commandType: CommandType.StoredProcedure);

            return await uow.Conn.ExecuteScalarAsync<string>(cmd)
                   ?? throw new Exception("Generate number gagal");
        }
    }

}