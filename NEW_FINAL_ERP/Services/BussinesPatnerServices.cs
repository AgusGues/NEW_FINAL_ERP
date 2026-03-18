using Microsoft.Data.SqlClient;
using NEW_FINAL_ERP.DTo;
using NEW_FINAL_ERP.Infrastructure;
using NEW_FINAL_ERP.Models;
using NEW_FINAL_ERP.Repositories;

namespace NEW_FINAL_ERP.Services
{
    public class BussinesPatnerServices
    {
        private readonly string _connString;
        private readonly IBussinesPatnerRepository _repo;
        public BussinesPatnerServices(IConfiguration config, IBussinesPatnerRepository repo)
        {
            _connString = config.GetConnectionString("DefaultConnection");
            _repo = repo;
        }

        public async Task Create(BussinesPatner model)
        {
            using var uow = new UnitOfWork(new SqlConnection(_connString));
            try
            {
                await _repo.Insert(uow, model);
                uow.Commit();
            }
            catch 
            {
                uow.Rollback();
                throw;
            }
        }

        public async Task Update(BussinesPatner model)
        {
            using var uow = new UnitOfWork(new SqlConnection(_connString));
            try
            {
                await _repo.Update(uow, model);
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
            using var uow = new UnitOfWork (new SqlConnection(_connString));
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

       
    }
}
