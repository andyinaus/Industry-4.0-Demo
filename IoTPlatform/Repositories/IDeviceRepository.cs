using System.Collections.Generic;
using System.Threading.Tasks;
using IoTPlatform.Persistences;

namespace IoTPlatform.Repositories
{
    public interface IDeviceRepository
    {
        Task<string> AddAsync(Device device);

        Task<Device> GetByIdAsync(string id);

        Task<IEnumerable<Device>> GetAllAsync();
    }
}