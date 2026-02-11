using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using NEW_FINAL_ERP.Models;
using NEW_FINAL_ERP.Infrastructure;

namespace NEW_FINAL_ERP.Repositories.Implementations
{
    public class BranchRepository : IBranchRepository
    {
        private readonly string _connString;

        public BranchRepository(IConfiguration config)
        {
            _connString = config.GetConnectionString("DefaultConnection")!;
        }

        public async Task<IEnumerable<Branch>> GetAll()
        {
            using var conn = new SqlConnection(_connString);

            var sql = @"
                        SELECT b.*, c.CompanyName
                        FROM Branch b
                        JOIN Company c ON b.CompanyId = c.CompanyId where b.IsActive=1";

            return await conn.QueryAsync<Branch>(sql);
        }

        public async Task<Branch?> GetById(int id)
        {
            using var conn = new SqlConnection(_connString);

            return await conn.QueryFirstOrDefaultAsync<Branch>(
                "SELECT * FROM Branch WHERE BranchId=@id and IsActive=1",
                new { id });
        }

        public async Task Insert(UnitOfWork uow, Branch branch)
        {
            var sql = @"
                        INSERT INTO Branch
                        (BranchCode,BranchName,CompanyId,VersionNo,CreatedAt,UpdatedAt)
                        VALUES
                        (@BranchCode,@BranchName,@CompanyId,1,GETDATE(),GETDATE())";

            await uow.Conn.ExecuteAsync(sql, branch, uow.Tx);
        }

        public async Task Update(UnitOfWork uow, Branch branch)
        {
            var sql = @"
                        UPDATE Branch SET
                        BranchName=@BranchName,
                        CompanyId=@CompanyId,
                        VersionNo = VersionNo + 1,
                        UpdatedAt = GETDATE()
                        WHERE BranchId=@BranchId and IsActive=1";

            await uow.Conn.ExecuteAsync(sql, branch, uow.Tx);
        }

        public async Task Delete(UnitOfWork uow, int id)
        {
            var sql = @"
            UPDATE Branch
            SET IsActive = 0,
                VersionNo = VersionNo + 1,
                UpdatedAt = GETDATE()
            WHERE BranchId=@id and IsActive=1";

            await uow.Conn.ExecuteAsync(sql, new { id }, uow.Tx);
        }
    }


}
