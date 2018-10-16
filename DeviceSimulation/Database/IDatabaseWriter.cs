using System.Threading.Tasks;
using DeviceSimulation.Simulation;

namespace DeviceSimulation.Database
{
    public interface IDatabaseWriter
    {
        Task WriteAsync(ConveyorSimulator.SimulationResult result);
    }
}