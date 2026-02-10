using Dapper;
using Microsoft.Data.SqlClient;
using NEW_FINAL_ERP.Infrastructure;
using NEW_FINAL_ERP.Models;
using NEW_FINAL_ERP.Repositories;
using System.Data;

namespace NEW_FINAL_ERP.Services
{
    public class CompanyService
    {
        private readonly string _connString;
        private readonly ICompanyRepository _repo;

        public CompanyService(IConfiguration config, ICompanyRepository repo)
        {
            _connString = config.GetConnectionString("DefaultConnection")!;
            _repo = repo;
        }

        public async Task<IEnumerable<Company>> GetAll()
        {
            await using var conn = new SqlConnection(_connString);
            return await _repo.GetAll(conn);
        }

        public async Task Create(Company company)
        {
            using var uow = new UnitOfWork(new SqlConnection(_connString));

            try
            {
                company.CompanyCode = await GenerateCode(
                    uow,
                    0,
                    "COMP"
                );

                await _repo.Insert(uow, company);

                uow.Commit();
            }
            catch
            {
                uow.Rollback();
                throw;
            }
        }


        private async Task<string> GenerateCode(UnitOfWork uow, int companyId, string entity)
        {
            var cmd = new CommandDefinition(
                "sp_NumberSequence_Generate",
                new
                {
                    CompanyId = companyId,
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