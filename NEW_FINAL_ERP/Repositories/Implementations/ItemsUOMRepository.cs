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

        public async Task<PagedResultItemsUom<ItemsUomListDto>>GetAll(string? search, int page, int pageSize)
        {
            using var conn = new SqlConnection(_connString);

            var where = @"
                        WHERE iu.IsActive = 1 
                        AND i.IsActive = 1
                        ";

            if (!string.IsNullOrWhiteSpace(search))
            {
                where += @"
                         AND (
                                i.ItemCode LIKE '%' + @search + '%'
                                OR i.ItemName LIKE '%' + @search + '%'
                                OR iu.Barcode LIKE '%' + @search + '%'
                             )
                             ";
            }

            // =========================
            // HITUNG TOTAL DATA
            // =========================
            var countSql = $@"
                            SELECT COUNT(1)
                            FROM ItemUOM iu
                            JOIN Items i ON iu.ItemId = i.ItemId
                            JOIN Unit u ON i.Unit = u.UnitId
                            JOIN Unit ux ON iu.UnitId = ux.UnitId
        {where}
                            ";

            var totalData = await conn.ExecuteScalarAsync<int>(
                countSql,
                new { search }
            );

            // =========================
            // PAGING
            // =========================
            var offset = (page - 1) * pageSize;

            var dataSql = $@"
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
        {where}
        ORDER BY iu.ItemUOMId DESC
        OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY
       ";

            var data = await conn.QueryAsync<ItemsUomListDto>(
                dataSql,
                new { search, offset, pageSize }
            );

            return new PagedResultItemsUom<ItemsUomListDto>
            {
                Data = data,
                Page = page,
                PageSize = pageSize,
                TotalData = totalData,
                TotalPages = (int)Math.Ceiling(totalData / (double)pageSize)
            };
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

        public async Task<ItemsUomListDto?> GetByIdDtoAsync(int id)
        {
            using var connection = new SqlConnection(_connString);

            var sql = @"
                        SELECT 
                            u.ItemUOMId,
                            i.ItemCode,
                            i.ItemName,
                            un.UnitName As SatuanKonversi,
                            u.ConversionToBase,
                            u.Barcode
                        FROM ItemUOM u
                        INNER JOIN Items i ON u.ItemId = i.ItemId
                        join Unit un on u.UnitId=un.UnitId
                        WHERE u.ItemUOMId = @Id and u.IsActive=1 and i.IsActive=1 and un.IsActive=1
                    ";

            return await connection.QueryFirstOrDefaultAsync<ItemsUomListDto>(sql, new { Id = id });
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
