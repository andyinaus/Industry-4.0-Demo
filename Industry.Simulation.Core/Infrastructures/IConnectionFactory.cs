using System.Data;
using System.Threading.Tasks;

namespace Industry.Simulation.Core.Infrastructures
{
    public interface IConnectionFactory
    {
        Task<IDbConnection> CreateConnection();
    }
}
