using NEW_FINAL_ERP.Infrastructure;
using NEW_FINAL_ERP.Models;
using System.Data;

namespace NEW_FINAL_ERP.Repositories;

public interface IBranchRepository
{
Task<IEnumerable<Branch>> GetAll(IDbConnection conn);
Task Insert(UnitOfWork uow, Branch branch);
}
