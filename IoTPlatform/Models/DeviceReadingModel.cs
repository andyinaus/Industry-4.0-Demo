using System;
using System.ComponentModel.DataAnnotations;

namespace IoTPlatform.Models
{
    public class DeviceReadingModel
    {
        [Required]
        public string Id { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateTime { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int Speed { get; set; }

        [Required]
        public string PackageTrackingAlarmState { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int CurrentBoards { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int CurrentRecipeCount { get; set; }
    }
}
