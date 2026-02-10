using System.Data;
using Microsoft.Data.SqlClient;

namespace NEW_FINAL_ERP.Infrastructure
{
    public class ConnectionFactory
    {
        private readonly string _connString;

        public ConnectionFactory(IConfiguration config)
        {
            _connString = config.GetConnectionString("DefaultConnection");
        }

        public IDbConnection Create()
        {
            return new SqlConnection(_connString);
        }
    }
}
