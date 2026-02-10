using Dapper;
using NEW_FINAL_ERP.Infrastructure;
using NEW_FINAL_ERP.Models;
using NEW_FINAL_ERP.Repositories;
using System.Data;

namespace NEW_FINAL_ERP.Repositories.Implementations
{
    public class BranchRepository : IBranchRepository
    {
        public async Task<IEnumerable<Branch>> GetAll(IDbConnection conn)
        {
            const string sql = "SELECT * FROM Branch ORDER BY BranchName";
            return await conn.QueryAsync<Branch>(sql);
        }

        public async Task Insert(UnitOfWork uow, Branch branch)
        {
            const string sql = @"INSERT INTO Branch
                            (BranchCode,BranchName,CompanyId,IsActive)
                            VALUES(@BranchCode,@BranchName,@CompanyId,@IsActive)";

            await uow.Conn.ExecuteAsync(sql, branch, uow.Tx);
        }
    }
}