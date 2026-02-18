using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using NEW_FINAL_ERP.Infrastructure;
using NEW_FINAL_ERP.Models;

namespace NEW_FINAL_ERP.Repositories.Implementations
{
    public class NumberSequenceRepository : INumberSequenceRepository
    {
        private readonly string _connString;

        public NumberSequenceRepository(IConfiguration config)
        {
            _connString = config.GetConnectionString("DefaultConnection")!;
        }

        public async Task<IEnumerable<NumberSequence>> GetAll()
        {
            using var conn = new SqlConnection(_connString);

            return await conn.QueryAsync<NumberSequence>(
                @"SELECT * FROM NumberSequence 
                  WHERE IsActive = 1");
        }

        public async Task Insert(UnitOfWork uow, NumberSequence seq)
        {
            var sql = @"
            INSERT INTO NumberSequence
            (
            CompanyId,
            EntityName,
            Prefix,
            NumberLength,
            ResetType,
            VersionNo,
            IsActive
            )
            VALUES
            (
            @CompanyId,
            @EntityName,
            @Prefix,
            @NumberLength,
            'Y',
            1,
            1
            )";

            await uow.Conn.ExecuteAsync(sql, seq, uow.Tx);
        }

        public async Task SoftDelete(UnitOfWork uow, int id)
        {
            var sql = @"
            UPDATE NumberSequence
            SET IsActive = 0,
                VersionNo = VersionNo + 1
            WHERE SequenceId = @id";

            await uow.Conn.ExecuteAsync(sql, new { id }, uow.Tx);
        }

        public async Task<string> GenerateNumber(
        UnitOfWork uow,
        int companyId,
        string entityName,
        Guid documentId)
        {
            var cmd = new CommandDefinition(
                "sp_NumberSequence_Generate",
                new
                {
                    CompanyId = companyId,
                    EntityName = entityName,
                    DocumentId = documentId,
                    CommandId = Guid.NewGuid()
                },
                uow.Tx,
                commandType: CommandType.StoredProcedure);

            return await uow.Conn.ExecuteScalarAsync<string>(cmd)
                   ?? throw new Exception("Generate number gagal");
        }

    }
}
