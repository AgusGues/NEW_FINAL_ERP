using Dapper;
using NEW_FINAL_ERP.Infrastructure;
using NEW_FINAL_ERP.Models;
using NEW_FINAL_ERP.Repositories;
using System.Data;

public class CompanyRepository : ICompanyRepository
{
    public async Task<IEnumerable<Company>> GetAll(IDbConnection conn)
    {
        const string sql = "SELECT * FROM Company ORDER BY CompanyName";
        return await conn.QueryAsync<Company>(sql);
    }

    public async Task Insert(UnitOfWork uow, Company company)
    {
        const string sql = @"INSERT INTO Company
                             (CompanyCode,CompanyName,BaseCurrencyCode ,IsActive)
                             VALUES(@CompanyCode,@CompanyName,@BaseCurrencyCode,@IsActive)";

        await uow.Conn.ExecuteAsync(sql, company, uow.Tx);
        
    }
}
