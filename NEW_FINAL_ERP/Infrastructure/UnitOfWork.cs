using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace NEW_FINAL_ERP.Infrastructure
{
    public class UnitOfWork : IDisposable
    {
        public IDbConnection Conn { get; private set; }
        public IDbTransaction Tx { get; private set; }

        // Constructor menerima IDbConnection
        public UnitOfWork(IDbConnection connection)
        {
            Conn = connection ?? throw new ArgumentNullException(nameof(connection));

            if (Conn.State != ConnectionState.Open)
                Conn.Open();

            Tx = Conn.BeginTransaction(IsolationLevel.Serializable);
        }

        public void Commit()
        {
            try { Tx?.Commit(); }
            finally { Dispose(); }
        }

        public void Rollback()
        {
            try { Tx?.Rollback(); }
            finally { Dispose(); }
        }

        public void Dispose()
        {
            Tx?.Dispose();
            if (Conn.State == ConnectionState.Open)
                Conn.Close();
            Conn.Dispose();
        }
    }
}
