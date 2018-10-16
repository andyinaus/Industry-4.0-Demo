using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Industry.Simulation.Core.Infrastructures;
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

        public async Task<IEnumerable<DeviceReading>> GetAllLatestReadingsAsync()
        {
            using (var connection = await _connectionFactory.CreateConnection())
            {
                var sql = $"SELECT R.{nameof(DeviceReading.DeviceId)}, D.{nameof(Device.Type)}, R.{nameof(DeviceReading.DateTime)}," +
                    $" R.{nameof(DeviceReading.Speed)}, R.{nameof(DeviceReading.PackageTrackingAlarmState)}," +
                    " TotalReadings.TotalBoards, TotalReadings.TotalRecipeCount" +
                    $" FROM {DeviceReadingsTableName} AS R, {DevicesTableName} AS D," +
                    $" (SELECT {nameof(DeviceReading.DeviceId)}, MAX({nameof(DeviceReading.DateTime)}) AS DateTime FROM {DeviceReadingsTableName}" +
                    $" GROUP BY {nameof(DeviceReading.DeviceId)}) AS LatestReadings," +
                    $" (SELECT {nameof(DeviceReading.DeviceId)}, SUM({nameof(DeviceReading.CurrentBoards)}) AS TotalBoards," +
                    $" SUM({nameof(DeviceReading.CurrentRecipeCount)}) AS TotalRecipeCount" +
                    $" FROM {DeviceReadingsTableName}" +
                    $" GROUP BY {nameof(DeviceReading.DeviceId)}) AS TotalReadings" +
                    $" WHERE LatestReadings.{nameof(DeviceReading.DeviceId)} = R.{nameof(DeviceReading.DeviceId)}" +
                    $" AND R.{nameof(DeviceReading.DeviceId)} = D.{nameof(Device.Id)}" +
                    $" AND TotalReadings.{nameof(DeviceReading.DeviceId)} = D.{nameof(Device.Id)}" +
                    $" AND R.{nameof(DeviceReading.DateTime)} = LatestReadings.DateTime";

                var results = await connection.QueryAsync<dynamic>(sql);

                return results.Select(r => new DeviceReading
                {
                    DeviceId = r.DeviceId,
                    DateTime = r.DateTime,
                    DeviceType = r.Type,
                    Speed = r.Speed,
                    PackageTrackingAlarmState = r.PackageTrackingAlarmState,
                    TotalBoards = r.TotalBoards,
                    TotalRecipeCount = r.TotalRecipeCount
                });
            }
        }
    }
}
