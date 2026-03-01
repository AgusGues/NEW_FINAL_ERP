using NEW_FINAL_ERP.DTo;
using NEW_FINAL_ERP.Infrastructure;
using NEW_FINAL_ERP.Models;

namespace NEW_FINAL_ERP.Repositories
{
    public interface IItemsUOMRepository
    {
        Task<PagedResultItemsUom<ItemsUomListDto>>GetAll(string? search, int page, int pageSize);
        Task<ItemUOMModalDto> GetModalDtoAsync(int id = 0);
        Task<IEnumerable<object>> SearchItemAsync(string term);
        Task<IEnumerable<object>> SearchUnitAsync(string term);

        Task<ItemsUom?> GetById(int id);
        Task<ItemsUomListDto?> GetByIdDtoAsync(int id);

        Task Insert(UnitOfWork uow, ItemsUom itemsuom);

        Task Update(UnitOfWork uow, ItemsUom itemsuom);

        Task Delete(UnitOfWork uow, int id);
    }
}
