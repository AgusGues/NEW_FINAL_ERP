using Microsoft.Data.SqlClient;
using NEW_FINAL_ERP.Infrastructure;
using NEW_FINAL_ERP.Models;
using NEW_FINAL_ERP.Repositories;

namespace NEW_FINAL_ERP.Services
{
    public class NumberSequenceService
    {
        private readonly string _connString;
        private readonly INumberSequenceRepository _repo;

        public NumberSequenceService(
            IConfiguration config,
            INumberSequenceRepository repo)
        {
            _connString = config.GetConnectionString("DefaultConnection")!;
            _repo = repo;
        }

        public Task<IEnumerable<NumberSequence>> GetAll()
            => _repo.GetAll();

        public async Task Create(NumberSequence seq)
        {
            using var uow = new UnitOfWork(new SqlConnection(_connString));

            try
            {
                await _repo.Insert(uow, seq);
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
                await _repo.SoftDelete(uow, id);
                uow.Commit();
            }
            catch
            {
                uow.Rollback();
                throw;
            }
        }

        

    }
}
