using Microsoft.Data.SqlClient;

namespace LegacySql.Legacy.Data
{
    public class LegacyDbConnection
    {
        private readonly string _connString;
        public LegacyDbConnection(string connectionString)
        {
            _connString = connectionString;
        }

        public SqlConnection Connection => new SqlConnection(_connString);
    }
}
