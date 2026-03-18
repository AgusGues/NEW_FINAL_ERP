using Dapper;
using Microsoft.Data.SqlClient;
using NEW_FINAL_ERP.DTo;
using NEW_FINAL_ERP.Infrastructure;
using NEW_FINAL_ERP.Models;

namespace NEW_FINAL_ERP.Repositories.Implementations
{
    public class BussinesPatnerRepository : IBussinesPatnerRepository
    {
        private readonly string _connString;

        public BussinesPatnerRepository(IConfiguration config)
        {
            _connString = config.GetConnectionString("DefaultConnection");
        }
        public async Task Delete(UnitOfWork uow, int id)
        {
            await uow.Conn.ExecuteAsync(@"
                                         Update BussinesPatner 
                                         set IsActive=0, 
                                         UpdatedAt=GETDATE() 
                                         where BusinessPartnerId=@id and IsActive=1",
                                         new { id },uow.Tx);
        }

        public async Task<PagedResultBussinesPatnerDto<BussinesPatnerListDto>> GetAll(string? search, int page, int pageSize)
        {
            using var conn = new SqlConnection(_connString);
            var where = @"";

            if (!string.IsNullOrWhiteSpace(search))
                where += "";
            var totalData = await conn.ExecuteScalarAsync<int>($@"");

            var offset = (page - 1) * pageSize;
            var data = await conn.QueryAsync<BussinesPatnerListDto>($@"");
            return new PagedResultBussinesPatnerDto<BussinesPatnerListDto>
            {
                Data = data,
                Page = page,
                PageSize = pageSize,
                TotalData = totalData,
                TotalPages = (int)Math.Ceiling(totalData / (double)pageSize)
            };
        }

        public async Task<BussinesPatner?> GetById(int id)
        {
            using var conn = new SqlConnection(_connString);
            return await conn.QueryFirstOrDefaultAsync<BussinesPatner>(@"select * from BussinesPatner
                                                                        where IsActive = 1 
                                                                        and BusinessPartnerId = @id", 
                                                                        new {id });
        }

        public async Task Insert(UnitOfWork uow, BussinesPatner entity)
        {
            await uow.Conn.ExecuteAsync(@"
                                        insert into BussinesPatner(
                                        BusinessPartnerId,
                                        BPCode,
                                        BPName,
                                        BPType,
                                        CurrencyCode,
                                        CreditLimit,
                                        PaymentTerm,
                                        TaxNumber,
                                        Phone,
                                        Email,
                                        Fax,
                                        Website,
                                        AddressLine1,
                                        AddressLine2,
                                        City,
                                        Province,
                                        PostalCode,
                                        IsActive,
                                        CreatedAt)
                                        VALUES
                                        (@BusinessPartnerId,
                                        @BPCode,
                                        @BPName,
                                        @BPType,
                                        @CurrencyCode,
                                        @CreditLimit,
                                        @PaymentTerm,
                                        @TaxNumber,
                                        @Phone,
                                        @Email,
                                        @Fax,
                                        @Website,
                                        @AddressLine1,
                                        @AddressLine2,
                                        @City,
                                        @Province,
                                        @PostalCode,
                                        1,
                                        GETDATE())",
                                        entity, uow.Tx);
        }

        public Task<IEnumerable<object>> SearchItemAsync(string term)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<object>> SearchUnitAsync(string term)
        {
            throw new NotImplementedException();
        }

        public async Task Update(UnitOfWork uow, BussinesPatner entity)
        {
            await uow.Conn.ExecuteAsync(@"
                                        update BussinesPatner set
                                        BPName=@BPName,
                                        BPType=@,
                                        CurrencyCode=@,
                                        CreditLimit=@,
                                        PaymentTerm=@,
                                        TaxNumber=@,
                                        Phone=@,
                                        Email=@,
                                        Fax=@,
                                        Website=@,
                                        AddressLine1=@,
                                        AddressLine2=@,
                                        City=@,
                                        Province=@,
                                        PostalCode=@,
                                        IsActive=@,
                                        UpdatedAt=GETDATE()
                                        where BussinesPatnerId = @BussinesPatnerId and IsActive = 1",
                                        entity, uow.Tx
                                        );
        }
    }
}
