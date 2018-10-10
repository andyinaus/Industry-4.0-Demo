﻿using System;

namespace IoTPlatform.Persistences
{
    public class DeviceReading
    {
        public string DeviceId { get; set; }

        public DateTime DateTime { get; set; }

        public int Speed { get; set; }

        public string PackageTrackingAlarmState { get; set; }

        public int CurrentTotalBoards { get; set; }

        public int CurrentRecipeCount { get; set; }
    }
}