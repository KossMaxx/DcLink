using System;
using System.Data.SqlClient;

namespace E2E.Tests.Infrastructure
{
    public class SqlDatabase
    {
        public SqlConnection Connection { get; private set; }

        public SqlDatabase()
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__LegacyDbContext");
            Connection = new SqlConnection(connectionString);
        }
    }
}
