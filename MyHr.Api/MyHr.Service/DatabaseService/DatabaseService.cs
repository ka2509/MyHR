using Microsoft.Data.SqlClient;

namespace MyHr.Service.DatabaseService
{
    internal abstract class DatabaseService
    {
        private static readonly string _defaultConnectionString = new SqlConnectionStringBuilder
        {
            DataSource = @"(localdb)\MSSQLLocalDB",
            InitialCatalog = "MyHRDB",
            IntegratedSecurity = true,
            TrustServerCertificate = true
        }.ConnectionString;

        protected async Task<SqlConnection> GetSqlConnectionAsync()
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                ?? _defaultConnectionString;
            var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}
