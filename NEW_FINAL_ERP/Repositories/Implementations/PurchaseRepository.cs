using Dapper;
using Microsoft.Data.SqlClient;
using NEW_FINAL_ERP.DTo;
using NEW_FINAL_ERP.Infrastructure;
using NEW_FINAL_ERP.Models;

namespace NEW_FINAL_ERP.Repositories.Implementations
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly string _connString;

        public PurchaseRepository(IConfiguration config)
        {
            _connString = config.GetConnectionString("DefaultConnection");
        }

        // ============================
        // GET ALL (PAGING + SEARCH)
        // ============================
        public async Task<PagedResultPurchaseDto<PurchaseListDto>> GetAll(string? search, int page, int pageSize)
        {
            using var conn = new SqlConnection(_connString);

            var where = "WHERE p.IsActive=1 AND s.IsActive=1";
            if (!string.IsNullOrWhiteSpace(search))
                where += @" AND (p.PurchaseNumber LIKE '%' + @search + '%' OR s.SupplierName LIKE '%' + @search + '%')";

            var totalData = await conn.ExecuteScalarAsync<int>($@"
                SELECT COUNT(1)
                FROM Purchase p
                JOIN Suppliers s ON p.SupplierId = s.SupplierId
                {where}", new { search });

            var offset = (page - 1) * pageSize;

            var data = await conn.QueryAsync<PurchaseListDto>($@"
                SELECT 
                    p.PurchaseId, p.PurchaseNumber, p.PurchaseDate,
                    s.SupplierName, p.Status, p.GrandTotal, p.UpdatedAt
                FROM Purchase p
                JOIN Suppliers s ON p.SupplierId = s.SupplierId
                {where}
                ORDER BY p.PurchaseId DESC
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY",
                new { search, offset, pageSize });

            return new PagedResultPurchaseDto<PurchaseListDto>
            {
                Data = data,
                TotalData = totalData,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalData / (double)pageSize)
            };
        }

        // ============================
        // HEADER CRUD
        // ============================
        public async Task<int> InsertHeader(UnitOfWork uow, Purchase entity)
        {
            var id = await uow.Conn.ExecuteScalarAsync<int>(@"
                INSERT INTO Purchase
                    (PurchaseNumber, PurchaseDate, SupplierId, CurrencyCode, ExchangeRate, SubTotal, DiscountAmount,
                     TaxAmount, OtherCost, GrandTotal, Status, Remarks, IsActive, CreatedAt)
                VALUES
                    (@PurchaseNumber, @PurchaseDate, @SupplierId, @CurrencyCode, @ExchangeRate, @SubTotal, @DiscountAmount,
                     @TaxAmount, @OtherCost, @GrandTotal, @Status, @Remarks, 1, GETDATE());
                SELECT CAST(SCOPE_IDENTITY() as int);",
                entity, uow.Tx);

            return id;
        }

        public async Task UpdateHeader(UnitOfWork uow, Purchase entity)
        {
            var rows = await uow.Conn.ExecuteAsync(@"
                UPDATE Purchase SET
                    PurchaseDate=@PurchaseDate,
                    SupplierId=@SupplierId,
                    CurrencyCode=@CurrencyCode,
                    ExchangeRate=@ExchangeRate,
                    SubTotal=@SubTotal,
                    DiscountAmount=@DiscountAmount,
                    TaxAmount=@TaxAmount,
                    OtherCost=@OtherCost,
                    GrandTotal=@GrandTotal,
                    Remarks=@Remarks,
                    UpdatedAt=GETDATE()
                WHERE PurchaseId=@PurchaseId
                  AND IsActive=1
                  AND Status='Draft'
                  AND ((@UpdatedAt IS NULL AND UpdatedAt IS NULL) OR UpdatedAt=@UpdatedAt)",
                entity, uow.Tx);

            if (rows == 0)
                throw new Exception("Data sudah berubah oleh user lain.");
        }

        public async Task<Purchase?> GetHeaderForUpdate(UnitOfWork uow, int id)
        {
            return await uow.Conn.QueryFirstOrDefaultAsync<Purchase>(@"
                SELECT * FROM Purchase WITH (UPDLOCK, ROWLOCK)
                WHERE PurchaseId=@id AND IsActive=1", new { id }, uow.Tx);
        }

        // ============================
        // DETAIL CRUD
        // ============================
        public async Task InsertDetail(UnitOfWork uow, PurchaseDetail detail)
        {
            await uow.Conn.ExecuteAsync(@"
                INSERT INTO PurchaseDetail
                    (PurchaseId, ItemId, ItemUOMId, UnitId, Quantity, BaseQuantity, UnitPrice, DiscountPercent, DiscountAmount,
                     TaxPercent, TaxAmount, LineTotal, AverageCostBefore, AverageCostAfter, IsActive, CreatedAt)
                VALUES
                    (@PurchaseId, @ItemId, @ItemUOMId, @UnitId, @Quantity, @BaseQuantity, @UnitPrice, @DiscountPercent, @DiscountAmount,
                     @TaxPercent, @TaxAmount, @LineTotal, @AverageCostBefore, @AverageCostAfter, 1, GETDATE())",
                detail, uow.Tx);
        }

        public async Task<IEnumerable<PurchaseDetail>> GetDetails(UnitOfWork uow, int purchaseId)
        {
            return await uow.Conn.QueryAsync<PurchaseDetail>(@"
                SELECT * FROM PurchaseDetail
                WHERE PurchaseId=@purchaseId AND IsActive=1",
                new { purchaseId }, uow.Tx);
        }

        public async Task SoftDeleteDetails(UnitOfWork uow, int purchaseId)
        {
            await uow.Conn.ExecuteAsync(@"
                UPDATE PurchaseDetail
                SET IsActive=0, UpdatedAt=GETDATE()
                WHERE PurchaseId=@purchaseId AND IsActive=1",
                new { purchaseId }, uow.Tx);
        }

        // ============================
        // DELETE HEADER
        // ============================
        public async Task SoftDeleteHeader(UnitOfWork uow, int id)
        {
            await uow.Conn.ExecuteAsync(@"
                UPDATE Purchase
                SET IsActive=0, UpdatedAt=GETDATE()
                WHERE PurchaseId=@id AND IsActive=1 AND Status='Draft'",
                new { id }, uow.Tx);
        }

        // ============================
        // NUMBER GENERATION
        // ============================
        public async Task<string?> GenerateNumber(UnitOfWork uow)
        {
            var next = await uow.Conn.ExecuteScalarAsync<int?>(@"
                UPDATE NumberSequence WITH (UPDLOCK, ROWLOCK)
                SET LastNumber = LastNumber + 1
                OUTPUT INSERTED.LastNumber
                WHERE Code='PUR'",
                transaction: uow.Tx);

            if (next == null) return null;

            return $"PUR-{DateTime.Now:yyyyMMdd}-{next.Value:D4}";
        }

        // ============================
        // FK VALIDATION
        // ============================
        public async Task<bool> SupplierExists(UnitOfWork uow, int supplierId)
        {
            var count = await uow.Conn.ExecuteScalarAsync<int>(@"
                SELECT COUNT(1) FROM Suppliers WHERE SupplierId=@supplierId AND IsActive=1",
                new { supplierId }, uow.Tx);

            return count > 0;
        }

        public async Task<bool> ItemExists(UnitOfWork uow, int itemId)
        {
            var count = await uow.Conn.ExecuteScalarAsync<int>(@"
                SELECT COUNT(1) FROM Items WHERE ItemId=@itemId AND IsActive=1",
                new { itemId }, uow.Tx);

            return count > 0;
        }

        public async Task<bool> ItemUomExists(UnitOfWork uow, int itemUomId)
        {
            var count = await uow.Conn.ExecuteScalarAsync<int>(@"
                SELECT COUNT(1) FROM ItemUOM WHERE ItemUOMId=@itemUomId",
                new { itemUomId }, uow.Tx);

            return count > 0;
        }

        public async Task<bool> UnitExists(UnitOfWork uow, int unitId)
        {
            var count = await uow.Conn.ExecuteScalarAsync<int>(@"
                SELECT COUNT(1) FROM Unit WHERE UnitId=@unitId AND IsActive=1",
                new { unitId }, uow.Tx);

            return count > 0;
        }

        // ============================
        // SELECT2 SEARCH
        // ============================
        public async Task<IEnumerable<object>> SearchSupplier(string term)
        {
            using var conn = new SqlConnection(_connString);
            return await conn.QueryAsync(@"
                SELECT TOP 10 SupplierId AS id, SupplierName AS text
                FROM Suppliers
                WHERE IsActive=1 AND SupplierName LIKE '%' + @term + '%'
                ORDER BY SupplierName", new { term });
        }

        public async Task<IEnumerable<object>> SearchItem(string term)
        {
            using var conn = new SqlConnection(_connString);
            return await conn.QueryAsync(@"
                SELECT TOP 10 ItemId AS id, ItemName AS text
                FROM Items
                WHERE IsActive=1 AND ItemName LIKE '%' + @term + '%'
                ORDER BY ItemName", new { term });
        }

        public async Task<IEnumerable<object>> SearchUnit(string term)
        {
            using var conn = new SqlConnection(_connString);
            return await conn.QueryAsync(@"
                SELECT TOP 10 UnitId AS id, UnitName AS text
                FROM Unit
                WHERE IsActive=1 AND UnitName LIKE '%' + @term + '%'
                ORDER BY UnitName", new { term });
        }
    }
}