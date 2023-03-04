using System;
using Npgsql;

namespace E2E.Tests.Infrastructure
{
    public class ErpMbSqlDatabase
    {
        public NpgsqlConnection Connection { get; private set; }

        public ErpMbSqlDatabase()
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__ErpMbAppDbContext");
            Connection = new NpgsqlConnection(connectionString);
        }
    }
}
