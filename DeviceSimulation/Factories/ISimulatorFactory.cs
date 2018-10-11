using DeviceSimulation.Simulators;

namespace DeviceSimulation.Factories
{
    public interface ISimulatorFactory
    {
        ConveyorSimulator CreateSimulator(string id);
    }
}