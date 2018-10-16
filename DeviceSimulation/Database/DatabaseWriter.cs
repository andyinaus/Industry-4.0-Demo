using System;
using System.Threading.Tasks;
using Dapper;
using DeviceSimulation.Simulation;
using Industry.Simulation.Core.Infrastructures;
using Serilog;

namespace DeviceSimulation.Database
{
    public class DatabaseWriter : IDatabaseWriter
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger _logger;

        public DatabaseWriter(IConnectionFactory connectionFactory, ILogger logger)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task WriteAsync(ConveyorSimulator.SimulationResult result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                var sql = "INSERT INTO DeviceReadings (DeviceId, DateTime, Speed," +
                          " PackageTrackingAlarmState, CurrentBoards, CurrentRecipeCount)" +
                          $" VALUES(@{nameof(ConveyorSimulator.SimulationResult.Id)}, @{nameof(ConveyorSimulator.SimulationResult.DateTime)}," +
                          $" @{nameof(ConveyorSimulator.SimulationResult.Speed)}, @{nameof(ConveyorSimulator.SimulationResult.PackageTrackingAlarmState)}," +
                          $" @{nameof(ConveyorSimulator.SimulationResult.CurrentBoards)}, @{nameof(ConveyorSimulator.SimulationResult.CurrentRecipeCount)})";

                await connection.ExecuteAsync(sql, result);

                _logger.Information($"A deviceReading with ID '{result.Id}' added.");
            }
        }
    }
}
