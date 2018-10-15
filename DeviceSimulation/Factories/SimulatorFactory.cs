using System;
using DeviceSimulation.Simulation;
using DeviceSimulation.Simulation.Options;
using DeviceSimulation.Utils;
using Microsoft.Extensions.Options;

namespace DeviceSimulation.Factories
{
    public class SimulatorFactory : ISimulatorFactory
    {
        private readonly IOptions<SimulatorSettingsOptions> _options;
        private readonly IClock _clock;

        public SimulatorFactory(IOptions<SimulatorSettingsOptions> options, IClock clock)
        {
            if (clock == null) throw new ArgumentNullException(nameof(clock));
            if (options?.Value == null) throw new ArgumentNullException(nameof(options));

            _options = options;
            _clock = clock;
        }

        public ConveyorSimulator CreateSimulator(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));

            return new ConveyorSimulator(id, _clock, _options);
        }
    }
}
