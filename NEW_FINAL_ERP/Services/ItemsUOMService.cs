using Dapper;
using NEW_FINAL_ERP.Infrastructure;
using System.Data;
using NEW_FINAL_ERP.Repositories;
using NEW_FINAL_ERP.Models;
using Microsoft.Data.SqlClient;
using NEW_FINAL_ERP.DTo;

namespace NEW_FINAL_ERP.Services
{
    public class ItemsUOMService
    {
        private readonly string _connString;
        private readonly IItemsUOMRepository _repo;
        public ItemsUOMService(IConfiguration config, IItemsUOMRepository repo)
        {
            _connString = config.GetConnectionString("DefaultConnection");
            _repo = repo;
        }

        // =========================================
        // PRIVATE - GENERATE BARCODE (SP)
        // =========================================
        private async Task<string> GenerateBarcode(UnitOfWork uow)
        {
            var cmd = new CommandDefinition(
                "sp_GenerateInternalBarcode",
                transaction: uow.Tx,
                commandType: CommandType.StoredProcedure
            );

            var barcode = await uow.Conn.ExecuteScalarAsync<string>(cmd);

            if (string.IsNullOrEmpty(barcode))
                throw new Exception("Generate Barcode gagal");

            return barcode;
        }

        // =========================================
        // LIST
        // =========================================
        public Task<PagedResultItemsUom<ItemsUomListDto>>GetAll(string? search, int page, int pageSize)
        {
            return _repo.GetAll(search, page, pageSize);
        }

        // =========================================
        // GET ENTITY BY ID (Edit)
        // =========================================
        public async Task<ItemsUom?> GetById(int id)
        {
            return await _repo.GetById(id);
        }

        public async Task<ItemsUomListDto?> GetByIdDtoAsync(int id)
        {
            return await _repo.GetByIdDtoAsync(id);
        }

        //autocomplete items
        public Task<IEnumerable<object>> SearchItemAsync(string term)
        {
            return _repo.SearchItemAsync(term);
        }

        //autocomplete units
        public Task<IEnumerable<object>> SearchUnitAsync(string term)
        {
            return _repo.SearchUnitAsync(term);
        }

        public Task<ItemUOMModalDto> GetModalDtoAsync(int id = 0)
        {
            return _repo.GetModalDtoAsync(id);
        }
        public async Task Create(ItemsUom itemsuom)
        {
            using var uow = new UnitOfWork(new SqlConnection(_connString));
            try
            {

                //cek duplikasi item yang sudah terinput di table ItemUom
                var exists = await uow.Conn.QueryFirstOrDefaultAsync<int>(
                                @"SELECT COUNT(1) FROM ItemUOM 
                                  WHERE ItemId = @ItemId AND UnitId = @UnitId AND IsActive = 1",
                                new { itemsuom.ItemId, itemsuom.UnitId },
                                transaction: uow.Tx  // wajib pakai transaction
);

                if (exists > 0)
                    throw new Exception("Konversi Unit untuk Item ini sudah ada.");

                if (string.IsNullOrWhiteSpace(itemsuom.Barcode))
                {
                    itemsuom.Barcode = await GenerateBarcode(uow);
                    itemsuom.IsInternalBarcode = true;
                }

                await _repo.Insert(uow, itemsuom);
                uow.Commit();
            }
            catch
            {
                uow.Rollback();
                throw;
            }
        }

        // =========================================
        // UPDATE
        // =========================================
        public async Task Update(ItemsUom itemsuom)
        {
            using var uow = new UnitOfWork(new SqlConnection(_connString));

            try
            {
                var exists = await uow.Conn.QueryFirstOrDefaultAsync<int>(
                        @"SELECT COUNT(1) FROM ItemUOM 
                          WHERE ItemId = @ItemId AND UnitId = @UnitId 
                            AND ItemUOMId <> @ItemUOMId AND IsActive = 1",
                        new { itemsuom.ItemId, itemsuom.UnitId, itemsuom.ItemUOMId },
                        transaction: uow.Tx
                    );

                if (exists > 0)
                    throw new Exception("Konversi Unit untuk Item ini sudah ada.");
                // 2️⃣ Generate barcode jika kosong
                if (string.IsNullOrWhiteSpace(itemsuom.Barcode))
                {
                    itemsuom.Barcode = await GenerateBarcode(uow);
                    itemsuom.IsInternalBarcode = true;
                }

                // 3️⃣ Update record
                await _repo.Update(uow, itemsuom); // repo ExecuteAsync pakai uow.Tx

                // 4️⃣ Commit transaction
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

    }
    
}
