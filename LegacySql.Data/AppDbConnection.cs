using Npgsql;

namespace LegacySql.Data
{
    public class AppDbConnection
    {
        public AppDbConnection(string connectionString)
        {
            Connection = new NpgsqlConnection(connectionString);
        }

        public NpgsqlConnection Connection { get; }
    }
}
