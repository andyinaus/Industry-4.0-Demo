using System.Collections.Generic;

namespace DeviceSimulation.Simulators.Options
{
    public class ConveyorSimulatorOptions
    {
        public IEnumerable<Simulator> Simulators { get; set; }
        
        public SimulatorSettings Settings { get; set; }

        public class Simulator
        {
            public string DeviceId { get; set; }
        }

        public class SimulatorSettings
        {
            public int SpeedMin { get; set; }

            public int SpeedMax { get; set; }

            public int MaximumItemsPerSecond { get; set; }
        }
    }
}
