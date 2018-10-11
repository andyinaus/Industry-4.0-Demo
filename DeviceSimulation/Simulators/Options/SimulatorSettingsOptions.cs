namespace DeviceSimulation.Simulators.Options
{
    public class SimulatorSettingsOptions
    {
        public SimulatorSettings Settings { get; set; }

        public class SimulatorSettings
        {
            public int SpeedMin { get; set; }

            public int SpeedMax { get; set; }

            public int MaximumItemsPerSecond { get; set; }
        }
    }
}
