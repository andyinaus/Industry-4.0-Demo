using System;
using DeviceSimulation.Simulation.Options;
using DeviceSimulation.Utils;
using Microsoft.Extensions.Options;

namespace DeviceSimulation.Simulation
{
    public class ConveyorSimulator
    {
        private readonly SimulatorSettingsOptions _options;

        public ConveyorSimulator(string id, IClock clock, IOptions<SimulatorSettingsOptions> options)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));
            if (clock == null) throw new ArgumentNullException(nameof(clock));
            if (options?.Value == null) throw new ArgumentNullException(nameof(options));

            Id = id;
            DateTime = clock.Now();
            _options = options.Value;
        }

        public ConveyorSimulator Simulate()
        {
            var random = new Random();
            Speed = random.Next(_options.SpeedMin, _options.SpeedMax);
            PackageTrackingAlarmState = (PackageTrackingAlarmState) random.Next(0, 1);
            CurrentRecipeCount = random.Next(_options.MaximumItemsPerSecond);
            CurrentBoards = random.Next(_options.MaximumItemsPerSecond);
            DateTime = DateTime.AddSeconds(1);

            return this;
        }

        public string Id { get; private set; }

        public DateTime DateTime { get; private set; }

        public int Speed { get; private set; }

        public PackageTrackingAlarmState PackageTrackingAlarmState { get; private set; }
        
        public int CurrentBoards { get; private set; }

        public int CurrentRecipeCount { get; private set; }
    }
}
