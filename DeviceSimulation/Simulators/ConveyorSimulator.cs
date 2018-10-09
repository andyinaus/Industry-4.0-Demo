using System;

namespace DeviceSimulation.Simulators
{
    public class ConveyorSimulator
    {
        public ConveyorSimulator(DateTime dateTime)
        {
            SerialNumber = Guid.NewGuid();
            DateTime = dateTime;
        }

        public ConveyorSimulator Simulate()
        {
            var random = new Random();
            Speed = random.Next(1, 99);
            PackageTrackingAlarmState = (PackageTrackingAlarmState) random.Next(0, 1);
            CurrentRecipeCount += random.Next(3);
            CurrentTotalBoards += random.Next(3);
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
