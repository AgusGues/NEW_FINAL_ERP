using NEW_FINAL_ERP.DTo;
using NEW_FINAL_ERP.Infrastructure;
using NEW_FINAL_ERP.Models;

namespace NEW_FINAL_ERP.Repositories
{
    public interface IItemPriceRepository
    {
        Task<PagedResultItemPriceDto<ItemPriceListDto>> GetAll(string? search, int page, int pageSize);
        Task<ItemPrice?> GetById(int id);
        Task Insert(UnitOfWork uow, ItemPrice entity);
        Task Update(UnitOfWork uow, ItemPrice entity);
        Task Delete(UnitOfWork uow, int id);
        Task<IEnumerable<object>> SearchItemAsync(string term);
        Task<IEnumerable<object>> SearchUnitAsync(string term);
    }
}
