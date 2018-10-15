using System;
using DeviceSimulation.Simulation;
using DeviceSimulation.Simulation.Options;
using Microsoft.Extensions.Options;

namespace DeviceSimulation.Factories
{
    public class SimulatorFactory : ISimulatorFactory
    {
        private readonly IOptions<SimulatorSettingsOptions> _options;

        public SimulatorFactory(IOptions<SimulatorSettingsOptions> options)
        {
            if (options?.Value == null) throw new ArgumentNullException(nameof(options));

            _options = options;
        }

        public ConveyorSimulator CreateSimulator(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));

            return new ConveyorSimulator(id, _options);
        }
    }
}
