using System;
using System.Collections.Generic;

namespace IoTPlatform.Models
{
    public class DeviceReadingsModel
    {
        public string Id { get; set; }

        public string DeviceType { get; set; }

        public IEnumerable<Reading> Readings { get; set; }

        public class Reading
        {
            public int Speed { get; set; }

            public DateTime DateTime { get; set; }

            public string PackageTrackingAlarmState { get; set; }

            public int CurrentBoards { get; set; }

            public int CurrentRecipeCount { get; set; }
        }
    }
}
