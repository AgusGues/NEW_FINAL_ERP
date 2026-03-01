using NEW_FINAL_ERP.DTo;
using NEW_FINAL_ERP.Infrastructure;
using NEW_FINAL_ERP.Models;
using NEW_FINAL_ERP.Repositories.Implementations;
using Microsoft.Data.SqlClient;
using NEW_FINAL_ERP.Repositories;

namespace NEW_FINAL_ERP.Services
{
    public class ItemPriceService
    {
        private readonly string _connString;
        private readonly IItemPriceRepository _repo;

        public ItemPriceService(IConfiguration config, IItemPriceRepository repo)
        {
            _connString = config.GetConnectionString("DefaultConnection");
            _repo = repo;
        }

        public Task<PagedResultItemPriceDto<ItemPriceListDto>> GetAll(string? search, int page, int pageSize)
            => _repo.GetAll(search, page, pageSize);

        public async Task<ItemPriceListDto?> GetById(int id)
        {
            var entity = await _repo.GetById(id);
            if (entity == null) return null;

            // ambil nama item & unit untuk Select2
            var listData = await _repo.GetAll(null, 1, 1000);
            var match = listData.Data.FirstOrDefault(x => x.ItemPriceId == id);

            return new ItemPriceListDto
            {
                ItemPriceId = entity.ItemPriceId,
                ItemId = entity.ItemId,
                UnitId = entity.UnitId,
                ItemName = match?.ItemName ?? "",
                UnitName = match?.UnitName ?? "",
                PriceType = entity.PriceType,
                Price = entity.Price,
                EffectiveDate = entity.EffectiveDate
            };
        }

        public async Task Create(ItemPrice model)
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

        public async Task Update(ItemPrice model)
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
            using var uow = new UnitOfWork(new SqlConnection(_connString));
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

        public Task<IEnumerable<object>> SearchItemAsync(string term) => _repo.SearchItemAsync(term);
        public Task<IEnumerable<object>> SearchUnitAsync(string term) => _repo.SearchUnitAsync(term);
    }
}