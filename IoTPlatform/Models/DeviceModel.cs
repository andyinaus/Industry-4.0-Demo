using System.ComponentModel.DataAnnotations;

namespace IoTPlatform.Models
{
    public class DeviceModel
    {
        public string Id { get; set; }

        [Required]
        public string Type { get; set; }
    }
}
