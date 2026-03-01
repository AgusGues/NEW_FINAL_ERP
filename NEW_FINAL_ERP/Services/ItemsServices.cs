using Dapper;
using Microsoft.Data.SqlClient;
using NEW_FINAL_ERP.DTo;
using NEW_FINAL_ERP.Infrastructure;
using NEW_FINAL_ERP.Models;
using NEW_FINAL_ERP.Repositories;
using NEW_FINAL_ERP.Repositories.Implementations;
using System.Data;

namespace NEW_FINAL_ERP.Services
{
    public class ItemsServices
    {
        private readonly string _connString;
        private readonly IItemsRepository _repo;

        public ItemsServices(IConfiguration config, IItemsRepository repo)
        {
            _connString = config.GetConnectionString("DefaultConnection");
            _repo = repo;
        }

        public Task<PagedResultItemsDto<Items>> GetAll(string? search, int page, int pageSize)
    => _repo.GetAll(search, page, pageSize);

        public Task<Items?> GetById(int id) => _repo.GetById(id);

        public async Task Create(Items items)
        {
            using var uow = new UnitOfWork(new SqlConnection(_connString));
            try 
            {
                items.ItemCode = await GenerateCode(uow);
                await _repo.Insert(uow, items);
                uow.Commit();
            }
            catch 
            {
                uow.Rollback();
                throw;
            }
        }

        public async Task Update(Items items)
        {
            using var uow = new UnitOfWork(new SqlConnection(_connString));

            try
            {
                await _repo.Update(uow, items);
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
                    EntityName = "ITM",
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
