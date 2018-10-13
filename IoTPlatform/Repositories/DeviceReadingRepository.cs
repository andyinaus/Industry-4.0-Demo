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

        private const string DeviceReadingsTableName = "DeviceReadings";
        private const string DevicesTableName = "Devices";

        public DeviceReadingRepository(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }


        public async Task AddAsync(DeviceReading reading)
        {
            if (reading == null) throw new ArgumentNullException(nameof(reading));

            using (var connection = await _connectionFactory.CreateConnection())
            {
                var sql = $"INSERT INTO {DeviceReadingsTableName} ({nameof(DeviceReading.DeviceId)}, {nameof(DeviceReading.DateTime)}, {nameof(DeviceReading.Speed)}," +
                          $" {nameof(DeviceReading.PackageTrackingAlarmState)}, {nameof(DeviceReading.CurrentBoards)}, {nameof(DeviceReading.CurrentRecipeCount)})"
                          + $" VALUES(@{nameof(DeviceReading.DeviceId)}, @{nameof(DeviceReading.DateTime)}, @{nameof(DeviceReading.Speed)}," +
                          $" @{nameof(DeviceReading.PackageTrackingAlarmState)}, @{nameof(DeviceReading.CurrentBoards)}, @{nameof(DeviceReading.CurrentRecipeCount)})";

                await connection.ExecuteAsync(sql, reading);

                Log.Information($"A deviceReading with ID '{reading.DeviceId}' added.");
            }
        }

        public async Task<IEnumerable<DeviceReading>> GetAllReadingsByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));

            using (var connection = await _connectionFactory.CreateConnection())
            {
                var sql = $"SELECT * FROM {DeviceReadingsTableName} AS R INNER JOIN {DevicesTableName} AS D" +
                          $" ON R.{nameof(DeviceReading.DeviceId)} = D.{nameof(Device.Id)}"
                          + $" WHERE {nameof(DeviceReading.DeviceId)} = @Id";

                return await connection.QueryAsync<DeviceReading, Device, DeviceReading>(sql,
                    (reading, device) =>
                    {
                        reading.DeviceType = device.Type;
                        return reading;
                    }, splitOn: $"{nameof(Device.Type)}", param: new { Id = id });
            }
        }

        public async Task<IEnumerable<DeviceReading>> GetAllReadingsAsync()
        {
            using (var connection = await _connectionFactory.CreateConnection())
            {
                var sql = $"SELECT * FROM {DeviceReadingsTableName} AS R INNER JOIN {DevicesTableName} AS D" +
                          $" ON R.{nameof(DeviceReading.DeviceId)} = D.{nameof(Device.Id)}";

                return await connection.QueryAsync<DeviceReading, Device, DeviceReading>(sql,
                    (reading, device) =>
                    {
                        reading.DeviceType = device.Type;
                        return reading;
                    }, splitOn: $"{nameof(Device.Type)}");
            }
        }
    }
}
