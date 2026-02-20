using NEW_FINAL_ERP.Infrastructure;
using NEW_FINAL_ERP.Models;

namespace NEW_FINAL_ERP.Repositories
{
    public interface IUnitRepository
    {
        Task<IEnumerable<Unit>> GetAll();

        Task<Unit?> GetById(int id);

        Task Insert(UnitOfWork uow, Unit unit);

        Task Update(UnitOfWork uow, Unit unit);

        Task Delete(UnitOfWork uow, int id);
    }
}
