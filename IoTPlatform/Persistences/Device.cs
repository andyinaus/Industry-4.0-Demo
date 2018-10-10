using System;

namespace IoTPlatform.Persistences
{
    public class Device
    {
        protected Device() { }

        public Device(string type)
        {
            if (string.IsNullOrWhiteSpace(type)) throw new ArgumentNullException(nameof(type));

            Id = Guid.NewGuid().ToString();
            Type = type;
        }

        public string Id { get; }

        public string Type { get; }
    }
}