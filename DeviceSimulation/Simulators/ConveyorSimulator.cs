using System;
using DeviceSimulation.Simulators.Options;
using DeviceSimulation.Utils;
using Microsoft.Extensions.Options;

namespace DeviceSimulation.Simulators
{
    public class ConveyorSimulator
    {
        private readonly ConveyorSimulatorOptions _options;

        public ConveyorSimulator(IClock clock, IOptions<ConveyorSimulatorOptions> options)
        {
            if (clock == null) throw new ArgumentNullException(nameof(clock));
            if (options == null) throw new ArgumentNullException(nameof(options));

            SerialNumber = Guid.NewGuid();
            DateTime = clock.Now();
            _options = options.Value;
        }

        public ConveyorSimulator Simulate()
        {
            var random = new Random();
            Speed = random.Next(_options.SpeedMin, _options.SpeedMax);
            PackageTrackingAlarmState = (PackageTrackingAlarmState) random.Next(0, 1);
            CurrentRecipeCount += random.Next(_options.MaximumItemsPerSecond);
            CurrentTotalBoards += random.Next(_options.MaximumItemsPerSecond);
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
