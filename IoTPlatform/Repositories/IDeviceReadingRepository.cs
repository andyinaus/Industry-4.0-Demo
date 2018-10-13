using System.Collections.Generic;
using System.Threading.Tasks;
using IoTPlatform.Persistences;

namespace IoTPlatform.Repositories
{
    public interface IDeviceReadingRepository
    {
        Task AddAsync(DeviceReading reading);
        Task<IEnumerable<DeviceReading>> GetAllReadingsByIdAsync(string id);
        Task<IEnumerable<DeviceReading>> GetAllReadingsAsync();
    }
}