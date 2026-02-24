using Microsoft.Data.SqlClient;

namespace HelpdeskApp.Data
{
    // Base class that holds the connection string
    // All other Db classes inherit from this
    public class BaseDb
    {
        private readonly string _connectionString;

        public BaseDb(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        protected SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}