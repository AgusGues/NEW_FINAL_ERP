using Dapper;
using Microsoft.Data.SqlClient;
using NEW_FINAL_ERP.DTo;
using NEW_FINAL_ERP.Infrastructure;
using NEW_FINAL_ERP.Models;

namespace NEW_FINAL_ERP.Repositories.Implementations
{
    public class ItemsUOMRepository : IItemsUOMRepository
    {
        private readonly string _connString;

        public ItemsUOMRepository(IConfiguration config)
        {
            _connString = config.GetConnectionString("DefaultConnection");
        }
        public async Task Delete(UnitOfWork uow, int id)
        {
            var sql = @"
                UPDATE ItemUOM
                SET IsActive = 0,
                    UpdatedAt = GETDATE()
                WHERE IsActive = 1
                  AND ItemUOMId = @id
            ";

            await uow.Conn.ExecuteAsync(sql, new { id }, uow.Tx);
        }

        public async Task<IEnumerable<ItemsUomListDto>> GetAll()
        {
            using var conn = new SqlConnection(_connString);

            var sql = @"
                SELECT 
                    iu.ItemUOMId,
                    iu.ItemId,
                    i.ItemCode,
                    i.ItemName,
                    u.UnitName  AS Satuan,
                    ux.UnitName AS SatuanKonversi,
                    iu.ConversionToBase,
                    iu.IsBase,
                    iu.IsDefaultSales,
                    iu.IsDefaultPurchase,
                    iu.Barcode,
                    iu.IsInternalBarcode
                FROM ItemUOM iu
                JOIN Items i ON iu.ItemId = i.ItemId
                JOIN Unit u ON i.Unit = u.UnitId         
                JOIN Unit ux ON iu.UnitId = ux.UnitId     
                WHERE iu.IsActive = 1 
                  AND i.IsActive = 1
            ";

            return await conn.QueryAsync<ItemsUomListDto>(sql);
        }

        // ================================
        // GET BY ID (Entity for Update)
        // ================================
        public async Task<ItemsUom?> GetById(int id)
        {
            using var conn = new SqlConnection(_connString);

            var sql = @"
                SELECT 
                    ItemUOMId,
                    ItemId,
                    UnitId,
                    ConversionToBase,
                    IsBase,
                    IsDefaultSales,
                    IsDefaultPurchase,
                    Barcode,
                    IsActive,
                    IsInternalBarcode
                FROM ItemUOM
                WHERE IsActive = 1 AND ItemUOMId = @id
            ";

            return await conn.QueryFirstOrDefaultAsync<ItemsUom>(sql, new { id });
        }


        // ================================
        // GET MODAL DTO (Create + Edit)
        // ================================
        public async Task<ItemUOMModalDto> GetModalDtoAsync(int id = 0)
        {
            using var conn = new SqlConnection(_connString);

            var items = await conn.QueryAsync<ItemDto>(
                "SELECT ItemId, ItemName, ItemCode FROM Items WHERE IsActive=1");

            var units = await conn.QueryAsync<UnitDto>(
                "SELECT UnitId, UnitName FROM Unit WHERE IsActive=1");

            var dto = new ItemUOMModalDto
            {
                Items = items,
                Units = units
            };

            if (id > 0)
            {
                var sql = @"
                    SELECT 
                        ItemUOMId,
                        ItemId,
                        UnitId,
                        ConversionToBase,
                        IsBase,
                        IsDefaultSales,
                        IsDefaultPurchase,
                        Barcode
                    FROM ItemUOM
                    WHERE IsActive=1 AND ItemUOMId=@id
                ";

                var data = await conn.QueryFirstOrDefaultAsync<ItemUOMModalDto>(sql, new { id });

                if (data != null)
                {
                    dto.ItemUOMId = data.ItemUOMId;
                    dto.ItemId = data.ItemId;
                    dto.UnitId = data.UnitId;
                    dto.ConversionToBase = data.ConversionToBase;
                    dto.IsBase = data.IsBase;
                    dto.IsDefaultSales = data.IsDefaultSales;
                    dto.IsDefaultPurchase = data.IsDefaultPurchase;
                    dto.Barcode = data.Barcode;
                }
            }

            return dto;
        }

        // ================================
        // INSERT (Entity + UoW)
        // ================================
        public async Task Insert(UnitOfWork uow, ItemsUom itemsuom)
        {
            var sql = @"
                INSERT INTO ItemUOM
                (
                    ItemId,
                    UnitId,
                    ConversionToBase,
                    IsBase,
                    IsDefaultSales,
                    IsDefaultPurchase,
                    Barcode,
                    IsActive,
                    CreatedAt,
                    IsInternalBarcode
                )
                VALUES
                (
                    @ItemId,
                    @UnitId,
                    @ConversionToBase,
                    @IsBase,
                    @IsDefaultSales,
                    @IsDefaultPurchase,
                    @Barcode,
                    1,
                    GETDATE(),
                    @IsInternalBarcode
                )
            ";

            await uow.Conn.ExecuteAsync(sql, itemsuom, uow.Tx);
        }

        public async Task<IEnumerable<object>> SearchItemAsync(string term)
        {
            using var conn = new SqlConnection(_connString);

            var sql = @"
                    SELECT TOP 10
                        ItemId as id,
                        ItemName as text
                    FROM Items
                    WHERE IsActive = 1
                      AND ItemName LIKE '%' + @term + '%'
                    ORDER BY ItemName";

            return await conn.QueryAsync(sql, new { term });
        }


        //Autocomplete unit
        public async Task<IEnumerable<object>> SearchUnitAsync(string term)
        {
            using var conn = new SqlConnection(_connString);
            var sql = @"
                      SELECT TOP 10
                        UnitId as id,
                        UnitName as text
                    FROM Unit
                    WHERE IsActive = 1
                      AND UnitName LIKE '%' + @term + '%'
                    ORDER BY UnitName  
                      ";
            return await conn.QueryAsync(sql, new { term });
        }

        // ================================
        // UPDATE (Entity + UoW)
        // ================================
        public async Task Update(UnitOfWork uow, ItemsUom itemsuom)
        {
            var sql = @"
                UPDATE ItemUOM SET
                    ItemId = @ItemId,
                    UnitId = @UnitId,
                    ConversionToBase = @ConversionToBase,
                    IsBase = @IsBase,
                    IsDefaultSales = @IsDefaultSales,
                    IsDefaultPurchase = @IsDefaultPurchase,
                    Barcode = @Barcode,
                    IsInternalBarcode = @IsInternalBarcode,
                    UpdatedAt = GETDATE()
                WHERE ItemUOMId = @ItemUOMId
            ";

            await uow.Conn.ExecuteAsync(sql, itemsuom, uow.Tx);
        }

    }
}
