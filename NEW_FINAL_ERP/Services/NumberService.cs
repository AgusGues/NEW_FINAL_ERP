using Dapper;
using NEW_FINAL_ERP.Infrastructure;
using System.Data;

namespace NEW_FINAL_ERP.Services
{
    public class NumberService
    {
        private readonly UnitOfWork _uow;

        public NumberService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<string> Generate(
            int companyId,
            string entityName,
            Guid documentId)
        {
            var number = await _uow.Conn.ExecuteScalarAsync<string>(
                "sp_NumberSequence_Generate",
                new
                {
                    CompanyId = companyId,
                    EntityName = entityName,
                    DocumentId = documentId,
                    CommandId = Guid.NewGuid()
                },
                _uow.Tx,
                commandType: CommandType.StoredProcedure
            );

            return number!;
        }
    }
}
