using System;
using DeviceSimulation.Simulators.Options;
using DeviceSimulation.Utils;
using Microsoft.Extensions.Options;

namespace DeviceSimulation.Simulators
{
    public class ConveyorSimulator
    {
        private readonly ConveyorSimulatorOptions.SimulatorSettings _settings;

        public ConveyorSimulator(IClock clock, IOptions<ConveyorSimulatorOptions> options)
        {
            if (clock == null) throw new ArgumentNullException(nameof(clock));
            if (options?.Value == null) throw new ArgumentNullException(nameof(options));
            if (options.Value.Settings == null) throw new ArgumentNullException(nameof(options.Value.Settings));

            SerialNumber = Guid.NewGuid();
            DateTime = clock.Now();
            _settings = options.Value.Settings;
        }

        public ConveyorSimulator Simulate()
        {
            var random = new Random();
            Speed = random.Next(_settings.SpeedMin, _settings.SpeedMax);
            PackageTrackingAlarmState = (PackageTrackingAlarmState) random.Next(0, 1);
            CurrentRecipeCount += random.Next(_settings.MaximumItemsPerSecond);
            CurrentTotalBoards += random.Next(_settings.MaximumItemsPerSecond);
            DateTime = DateTime.AddSeconds(1);

            return this;
        }

        public Guid SerialNumber { get; private set; }

        public DateTime DateTime { get; private set; }

        public int Speed { get; private set; }

        public PackageTrackingAlarmState PackageTrackingAlarmState { get; private set; }
        
        public int CurrentTotalBoards { get; private set; }

        public int CurrentRecipeCount { get; private set; }
    }
}
