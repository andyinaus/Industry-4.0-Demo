using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using IoTPlatform.Infrastructures;
using IoTPlatform.Persistences;
using Serilog;

namespace IoTPlatform.Repositories
{
    public class DeviceReadingRepository : IDeviceReadingRepository
    {
        private readonly IConnectionFactory _connectionFactory;

        private const string TableName = "DeviceReadings";

        public DeviceReadingRepository(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }


        public async Task AddAsync(DeviceReading reading)
        {
            if (reading == null) throw new ArgumentNullException(nameof(reading));

            using (var connection = await _connectionFactory.CreateConnection())
            {
                var sql = $"INSERT INTO {TableName} (DeviceId, DateTime, Speed, PackageTrackingAlarmState, CurrentBoards, CurrentRecipeCount)"
                          + " VALUES(@DeviceId, @DateTime, @Speed, @PackageTrackingAlarmState, @CurrentBoards, @CurrentRecipeCount)";

                await connection.ExecuteAsync(sql, reading);

                Log.Information($"A deviceReading with ID '{reading.DeviceId}' added.");
            }
        }

        public async Task<IEnumerable<DeviceReading>> GetAllReadingsByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));

            using (var connection = await _connectionFactory.CreateConnection())
            {
                var sql = $"SELECT * FROM {TableName}"
                          + " WHERE DeviceId = @Id";

                return await connection.QueryAsync<DeviceReading>(sql, new { Id = id });
            }
        }
    }
}
