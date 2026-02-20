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

        public Task<IEnumerable<ItemsUomListDto>> GetAll() => _repo.GetAll();


        public async Task Create(ItemsUom itemsuom)
        {
            using var uow = new UnitOfWork(new SqlConnection(_connString));
            try
            {
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
    }
}
