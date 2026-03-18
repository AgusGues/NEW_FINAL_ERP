using NEW_FINAL_ERP.DTo;
using NEW_FINAL_ERP.Infrastructure;
using NEW_FINAL_ERP.Models;

namespace NEW_FINAL_ERP.Repositories
{
    public interface IBussinesPatnerRepository
    {
        Task<PagedResultBussinesPatnerDto<BussinesPatnerListDto>> GetAll(string? search, int page, int pageSize);
        Task<BussinesPatner?> GetById(int id);
        Task Insert(UnitOfWork uow, BussinesPatner entity);
        Task Update(UnitOfWork uow, BussinesPatner entity);
        Task Delete(UnitOfWork uow, int id);
        Task<IEnumerable<object>> SearchItemAsync(string term);
        Task<IEnumerable<object>> SearchUnitAsync(string term);
    }
}
