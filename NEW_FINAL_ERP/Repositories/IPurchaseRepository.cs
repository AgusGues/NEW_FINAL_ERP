using NEW_FINAL_ERP.DTo;
using NEW_FINAL_ERP.Infrastructure;
using NEW_FINAL_ERP.Models;

namespace NEW_FINAL_ERP.Repositories
{
    public interface IPurchaseRepository
    {
        // ============================
        // PAGING + SEARCH
        // ============================
        Task<PagedResultPurchaseDto<PurchaseListDto>> GetAll(string? search, int page, int pageSize);

        // ============================
        // HEADER CRUD
        // ============================
        Task<int> InsertHeader(UnitOfWork uow, Purchase entity);
        Task UpdateHeader(UnitOfWork uow, Purchase entity);
        Task<Purchase?> GetHeaderForUpdate(UnitOfWork uow, int id);

        // ============================
        // DETAIL CRUD
        // ============================
        Task InsertDetail(UnitOfWork uow, PurchaseDetail detail);
        Task<IEnumerable<PurchaseDetail>> GetDetails(UnitOfWork uow, int purchaseId);
        Task SoftDeleteDetails(UnitOfWork uow, int purchaseId);

        // ============================
        // DELETE HEADER
        // ============================
        Task SoftDeleteHeader(UnitOfWork uow, int id);

        // ============================
        // NUMBER GENERATION
        // ============================
        Task<string?> GenerateNumber(UnitOfWork uow);

        // ============================
        // FK VALIDATION
        // ============================
        Task<bool> SupplierExists(UnitOfWork uow, int supplierId);
        Task<bool> ItemExists(UnitOfWork uow, int itemId);
        Task<bool> ItemUomExists(UnitOfWork uow, int itemUomId);
        Task<bool> UnitExists(UnitOfWork uow, int unitId);

        // ============================
        // SELECT2 SEARCH
        // ============================
        Task<IEnumerable<object>> SearchSupplier(string term);
        Task<IEnumerable<object>> SearchItem(string term);
        Task<IEnumerable<object>> SearchUnit(string term);
    }
}