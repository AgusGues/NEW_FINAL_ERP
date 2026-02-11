using NEW_FINAL_ERP.Infrastructure;
using NEW_FINAL_ERP.Models;
using System.Data;

namespace NEW_FINAL_ERP.Repositories 
{
  
        public interface IBranchRepository
        {
            Task<IEnumerable<Branch>> GetAll();

            Task<Branch?> GetById(int id);

            Task Insert(UnitOfWork uow, Branch branch);

            Task Update(UnitOfWork uow, Branch branch);

            Task Delete(UnitOfWork uow, int id);
        }
    }
    




