using System.Collections.Generic;

namespace DeviceSimulation.Simulators.Options
{
    public class RequiredSimulatorsOptions
    {
        public IEnumerable<Simulator> Simulators { get; set; }

        public class Simulator
        {
            public string DeviceId { get; set; }
        }
    }
}
