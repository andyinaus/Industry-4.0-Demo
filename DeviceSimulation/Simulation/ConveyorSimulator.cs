using System;
using DeviceSimulation.Simulation.Options;
using Microsoft.Extensions.Options;

namespace DeviceSimulation.Simulation
{
    public class ConveyorSimulator
    {
        private readonly SimulatorSettingsOptions _options;

        public ConveyorSimulator(string id, IOptions<SimulatorSettingsOptions> options)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));
            if (options?.Value == null) throw new ArgumentNullException(nameof(options));

            Id = id;
            _options = options.Value;
        }

        public SimulationResult SimulateAt(DateTime dateTime)
        {
            
            var random = new Random();
            return new SimulationResult
            {
                Id = Id,
                Speed = random.Next(_options.SpeedMin, _options.SpeedMax + 1),
                PackageTrackingAlarmState = (PackageTrackingAlarmState)random.Next(0, 2),
                CurrentRecipeCount = random.Next(_options.MaximumItemsPerSecond + 1),
                CurrentBoards = random.Next(_options.MaximumItemsPerSecond + 1),
                DateTime = dateTime
            };
        }

        public string Id { get; private set; }

        public class SimulationResult
        {
            public string Id { get; set; }

            public DateTime DateTime { get; set; }

            public int Speed { get; set; }

            public PackageTrackingAlarmState PackageTrackingAlarmState { get; set; }

            public int CurrentBoards { get; set; }

            public int CurrentRecipeCount { get; set; }
        }
    }
}
