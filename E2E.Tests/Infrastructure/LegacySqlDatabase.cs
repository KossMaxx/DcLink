using System;
using Npgsql;

namespace E2E.Tests.Infrastructure
{    
    public class LegacySqlDatabase
    {
        public NpgsqlConnection Connection { get; private set; }

        public LegacySqlDatabase()
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__AppDbContext");
            Connection = new NpgsqlConnection(connectionString);
        }

    }
}
