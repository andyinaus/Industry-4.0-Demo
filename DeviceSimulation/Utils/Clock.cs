using System;

namespace DeviceSimulation.Utils
{
    public class Clock : IClock
    {
        private readonly DateTime _dateTime;

        public Clock(DateTime dateTime)
        {
            _dateTime = dateTime;
        }

        public DateTime Now()
        {
            return _dateTime;
        }
    }
}
