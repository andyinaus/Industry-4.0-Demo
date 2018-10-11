using System;
using DeviceSimulation.Simulators.Options;
using DeviceSimulation.Utils;
using Microsoft.Extensions.Options;

namespace DeviceSimulation.Simulators
{
    public class ConveyorSimulator
    {
        private readonly SimulatorSettingsOptions _options;

        public ConveyorSimulator(string id, IClock clock, IOptions<SimulatorSettingsOptions> options)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));
            if (clock == null) throw new ArgumentNullException(nameof(clock));
            if (options?.Value == null) throw new ArgumentNullException(nameof(options));
            if (options.Value.Settings == null) throw new ArgumentNullException(nameof(options.Value.Settings));

            Id = id;
            DateTime = clock.Now();
            _options = options.Value;
        }

        public ConveyorSimulator Simulate()
        {
            var random = new Random();
            Speed = random.Next(_options.Settings.SpeedMin, _options.Settings.SpeedMax);
            PackageTrackingAlarmState = (PackageTrackingAlarmState) random.Next(0, 1);
            CurrentRecipeCount += random.Next(_options.Settings.MaximumItemsPerSecond);
            CurrentTotalBoards += random.Next(_options.Settings.MaximumItemsPerSecond);
            DateTime = DateTime.AddSeconds(1);

            return this;
        }

        public string Id { get; private set; }

        public DateTime DateTime { get; private set; }

        public int Speed { get; private set; }

        public PackageTrackingAlarmState PackageTrackingAlarmState { get; private set; }
        
        public int CurrentTotalBoards { get; private set; }

        public int CurrentRecipeCount { get; private set; }
    }
}
