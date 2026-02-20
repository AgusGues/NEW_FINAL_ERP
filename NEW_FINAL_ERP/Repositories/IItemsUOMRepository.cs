using NEW_FINAL_ERP.Infrastructure;
using NEW_FINAL_ERP.Models;

namespace NEW_FINAL_ERP.Repositories
{
    public interface IItemsUOMRepository
    {
        Task<IEnumerable<ItemsUom>> GetAll();

        Task<ItemsUom?> GetById(int id);

        Task Insert(UnitOfWork uow, ItemsUom itemsuom);

        Task Update(UnitOfWork uow, ItemsUom itemsuom);

        Task Delete(UnitOfWork uow, int id);
    }
}
