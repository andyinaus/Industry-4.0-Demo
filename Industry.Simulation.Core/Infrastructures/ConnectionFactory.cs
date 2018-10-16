using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Industry.Simulation.Core.Infrastructures
{
    public class ConnectionFactory : IConnectionFactory
    {
        private const string ConnectionStringSectionName = "IoTDatabase";
        private readonly string _connectionString;

        public ConnectionFactory(IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            _connectionString = configuration.GetConnectionString(ConnectionStringSectionName);
        }

        public async Task<IDbConnection> CreateConnectionAsync()
        {
            var sqlConnection = new SqlConnection(_connectionString);
            await sqlConnection.OpenAsync();

            return sqlConnection;
        }
    }
}
