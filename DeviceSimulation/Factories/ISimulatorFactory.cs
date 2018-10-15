using DeviceSimulation.Simulation;

namespace DeviceSimulation.Factories
{
    public interface ISimulatorFactory
    {
        ConveyorSimulator CreateSimulator(string id);
    }
}