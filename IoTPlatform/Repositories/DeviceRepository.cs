using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using IoTPlatform.Infrastructures;
using IoTPlatform.Persistences;
using Serilog;

namespace IoTPlatform.Repositories
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly IConnectionFactory _connectionFactory;
        private const string TableName = "Devices";

        public DeviceRepository(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public async Task<string> AddAsync(Device device)
        {
            if (device == null) throw new ArgumentNullException(nameof(device));

            using (var connection = await _connectionFactory.CreateConnection())
            {
                var sql = $"INSERT INTO {TableName} (ID, Type)"
                    + " VALUES(@Id, @Type)";

                await connection.ExecuteAsync(sql, device);

                Log.Information($"A device with ID '{device.Id}' added.");

                return device.Id;
            }
        }

        public async Task<Device> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));

            using (var connection = await _connectionFactory.CreateConnection())
            {
                var sql = $"SELECT * FROM {TableName}"
                          + " WHERE Id = @Id";

                var devices = await connection.QueryAsync<Device>(sql, new {Id = id});

                return devices.FirstOrDefault();
            }
        }

        public async Task<IEnumerable<Device>> GetAllAsync()
        {
            using (var connection = await _connectionFactory.CreateConnection())
            {
                var sql = $"SELECT * FROM {TableName}";

                return await connection.QueryAsync<Device>(sql);
            }
        }
    }
}
