using Microsoft.Data.SqlClient;
using NEW_FINAL_ERP.DTo;
using NEW_FINAL_ERP.Infrastructure;
using NEW_FINAL_ERP.Models;
using NEW_FINAL_ERP.Repositories;

namespace NEW_FINAL_ERP.Services
{
    public class PurchaseService
    {
        private readonly string _connString;
        private readonly IPurchaseRepository _repo;

        public PurchaseService(IConfiguration config, IPurchaseRepository repo)
        {
            _connString = config.GetConnectionString("DefaultConnection");
            _repo = repo;
        }

        // ============================
        // GET ALL PAGED
        // ============================
        public Task<PagedResultPurchaseDto<PurchaseListDto>> GetAll(string? search, int page, int pageSize)
            => _repo.GetAll(search, page, pageSize);

        // ============================
        // GET BY ID + DETAILS
        // ============================
        public async Task<PurchaseFormDto?> GetByIdAsync(int id)
        {
            using var uow = new UnitOfWork(new SqlConnection(_connString));

            var header = await _repo.GetHeaderForUpdate(uow, id);
            if (header == null) return null;

            var details = await _repo.GetDetails(uow, id);

            return new PurchaseFormDto
            {
                PurchaseId = header.PurchaseId,
                PurchaseDate = header.PurchaseDate,
                SupplierId = header.SupplierId,
                CurrencyCode = header.CurrencyCode,
                ExchangeRate = header.ExchangeRate,
                DiscountAmount = header.DiscountAmount,
                TaxAmount = header.TaxAmount,
                OtherCost = header.OtherCost,
                Remarks = header.Remarks,
                Status = header.Status,
                UpdatedAt = header.UpdatedAt ?? header.CreatedAt,
                Details = details.ToList()
            };
        }

        // ============================
        // CREATE
        // ============================
        public async Task CreateAsync(PurchaseFormDto dto)
        {
            using var uow = new UnitOfWork(new SqlConnection(_connString));

            try
            {
                ValidateHeader(dto);
                await ValidateDetails(dto, uow);

                var number = await _repo.GenerateNumber(uow);
                if (string.IsNullOrWhiteSpace(number))
                    throw new Exception("Gagal generate nomor.");

                var header = new Purchase
                {
                    PurchaseNumber = number,
                    PurchaseDate = dto.PurchaseDate,
                    SupplierId = dto.SupplierId,
                    CurrencyCode = dto.CurrencyCode ?? "IDR",
                    ExchangeRate = dto.ExchangeRate <= 0 ? 1 : dto.ExchangeRate,
                    DiscountAmount = Normalize(dto.DiscountAmount),
                    TaxAmount = Normalize(dto.TaxAmount),
                    OtherCost = Normalize(dto.OtherCost),
                    Status = "Draft",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                RecalculateHeader(header, dto.Details);

                var newId = await _repo.InsertHeader(uow, header);

                foreach (var d in dto.Details)
                {
                    PrepareDetail(d, newId);
                    await _repo.InsertDetail(uow, d);
                }

                uow.Commit();
            }
            catch
            {
                uow.Rollback();
                throw;
            }
        }

        // ============================
        // UPDATE
        // ============================
        public async Task UpdateAsync(PurchaseFormDto dto)
        {
            using var uow = new UnitOfWork(new SqlConnection(_connString));

            try
            {
                if (dto.PurchaseId <= 0)
                    throw new Exception("ID tidak valid.");

                var header = await _repo.GetHeaderForUpdate(uow, dto.PurchaseId);
                if (header == null)
                    throw new Exception("Data tidak ditemukan.");
                if (header.Status != "Draft")
                    throw new Exception("Hanya Draft bisa diubah.");

                if ((header.UpdatedAt ?? header.CreatedAt) != dto.UpdatedAt)
                    throw new Exception("Data sudah diubah user lain.");

                ValidateHeader(dto);
                await ValidateDetails(dto, uow);

                header.PurchaseDate = dto.PurchaseDate;
                header.SupplierId = dto.SupplierId;
                header.CurrencyCode = dto.CurrencyCode ?? "IDR";
                header.ExchangeRate = dto.ExchangeRate <= 0 ? 1 : dto.ExchangeRate;
                header.DiscountAmount = Normalize(dto.DiscountAmount);
                header.TaxAmount = Normalize(dto.TaxAmount);
                header.OtherCost = Normalize(dto.OtherCost);
                header.Remarks = dto.Remarks;

                RecalculateHeader(header, dto.Details);

                await _repo.UpdateHeader(uow, header);

                // Replace details
                await _repo.SoftDeleteDetails(uow, header.PurchaseId);
                foreach (var d in dto.Details)
                {
                    PrepareDetail(d, header.PurchaseId);
                    await _repo.InsertDetail(uow, d);
                }

                uow.Commit();
            }
            catch
            {
                uow.Rollback();
                throw;
            }
        }

        // ============================
        // DELETE
        // ============================
        public async Task DeleteAsync(int id)
        {
            using var uow = new UnitOfWork(new SqlConnection(_connString));

            try
            {
                var header = await _repo.GetHeaderForUpdate(uow, id);
                if (header == null)
                    throw new Exception("Data tidak ditemukan.");
                if (header.Status != "Draft")
                    throw new Exception("Hanya Draft bisa dihapus.");

                await _repo.SoftDeleteDetails(uow, id);
                await _repo.SoftDeleteHeader(uow, id);

                uow.Commit();
            }
            catch
            {
                uow.Rollback();
                throw;
            }
        }

        // ============================
        // PRIVATE HELPERS
        // ============================
        private void ValidateHeader(PurchaseFormDto dto)
        {
            if (dto.SupplierId <= 0) throw new Exception("Supplier tidak valid.");
            if (dto.Details == null || !dto.Details.Any()) throw new Exception("Detail minimal 1.");
        }

        private async Task ValidateDetails(PurchaseFormDto dto, UnitOfWork uow)
        {
            foreach (var d in dto.Details)
            {
                if (d.Quantity <= 0) throw new Exception("Quantity harus > 0.");
                if (d.UnitPrice < 0) throw new Exception("UnitPrice tidak boleh negatif.");

                if (!await _repo.ItemExists(uow, d.ItemId))
                    throw new Exception("Item tidak valid.");
                if (!await _repo.ItemUomExists(uow, d.ItemUOMId))
                    throw new Exception("ItemUOM tidak valid.");
                if (!await _repo.UnitExists(uow, d.UnitId))
                    throw new Exception("Unit tidak valid.");
            }
        }

        private decimal Normalize(decimal value) => value < 0 ? 0 : value;

        private void RecalculateHeader(Purchase header, List<PurchaseDetail> details)
        {
            decimal subTotal = 0;
            foreach (var d in details)
            {
                d.Quantity = Normalize(d.Quantity);
                d.UnitPrice = Normalize(d.UnitPrice);
                d.DiscountPercent = Normalize(d.DiscountPercent);
                d.DiscountAmount = Normalize(d.DiscountAmount);
                d.TaxPercent = Normalize(d.TaxPercent);
                d.TaxAmount = Normalize(d.TaxAmount);

                d.LineTotal = (d.Quantity * d.UnitPrice) - d.DiscountAmount + d.TaxAmount;
                subTotal += d.LineTotal;
            }

            header.SubTotal = subTotal;
            header.GrandTotal = subTotal - header.DiscountAmount + header.TaxAmount + header.OtherCost;
        }

        private void PrepareDetail(PurchaseDetail d, int purchaseId)
        {
            d.PurchaseId = purchaseId;
            d.IsActive = true;
            d.CreatedAt = DateTime.Now;
            d.UpdatedAt = null;
        }

        // ============================
        // SEARCH FOR SELECT2
        // ============================
        public Task<IEnumerable<object>> SearchSupplier(string term) => _repo.SearchSupplier(term);
        public Task<IEnumerable<object>> SearchItem(string term) => _repo.SearchItem(term);
        public Task<IEnumerable<object>> SearchUnit(string term) => _repo.SearchUnit(term);
    }
}