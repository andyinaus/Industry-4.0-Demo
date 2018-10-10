using System.Data;
using System.Threading.Tasks;

namespace IoTPlatform.Infrastructures
{
    public interface IConnectionFactory
    {
        Task<IDbConnection> CreateConnection();
    }
}
