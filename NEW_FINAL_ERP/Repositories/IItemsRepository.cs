using NEW_FINAL_ERP.Infrastructure;
using NEW_FINAL_ERP.Models;
using System.Data;

namespace NEW_FINAL_ERP.Repositories
{
    public interface IItemsRepository
    {
        Task<IEnumerable<Items>> GetAll();

        Task<Items?> GetById(int id);

        Task Insert(UnitOfWork uow, Items items);

        Task Update(UnitOfWork uow, Items items);

        Task Delete(UnitOfWork uow, int id);
    }
}
