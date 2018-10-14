using System;

namespace IoTPlatform.Models
{
    public class LatestDeviceReadingModel
    {
        public string Id { get; set; }

        public string DeviceType { get; set; }

        public DateTime DateTime { get; set; }

        public int Speed { get; set; }

        public string PackageTrackingAlarmState { get; set; }

        public long TotalBoards { get; set; }

        public long TotalRecipeCount { get; set; }
    }
}
