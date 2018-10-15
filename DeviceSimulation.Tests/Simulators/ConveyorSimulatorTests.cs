using System;
using DeviceSimulation.Simulation;
using DeviceSimulation.Simulation.Options;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DeviceSimulation.Tests.Simulators
{
    [TestClass]
    public class ConveyorSimulatorTests
    {
        [TestMethod]
        public void CtorWhenIdIsNullShouldThrowArgumentNullException()
        {
            Action target = () => new ConveyorSimulator(null, Options.Create(new SimulatorSettingsOptions()));

            Assert.ThrowsException<ArgumentNullException>(target);
        }

        [TestMethod]
        public void CtorWhenIdIsWhiteSpaceShouldThrowArgumentNullException()
        {
            Action target = () => new ConveyorSimulator(" ", Options.Create(new SimulatorSettingsOptions()));

            Assert.ThrowsException<ArgumentNullException>(target);
        }

        [TestMethod]
        public void CtorWhenSimulatorOptionsIsNullShouldThrowArgumentNullException()
        {
            Action target = () => new ConveyorSimulator("1234", null);

            Assert.ThrowsException<ArgumentNullException>(target);
        }

        [TestMethod]
        public void SimulateWithValidSimulatorOptionsShouldReturnSimulationResultAccordingly()
        {
            var options = Options.Create(new SimulatorSettingsOptions
            {
                MaximumItemsPerSecond = 4,
                SpeedMax = 10,
                SpeedMin = 5
            });

            var simulator = new ConveyorSimulator("1234", options);

            var result = simulator.SimulateAt(DateTime.Now);

            Assert.IsTrue(result.Speed >= options.Value.SpeedMin);
            Assert.IsTrue(result.Speed <= options.Value.SpeedMax);
            Assert.IsTrue(result.CurrentRecipeCount <= options.Value.MaximumItemsPerSecond);
            Assert.IsTrue(result.CurrentBoards <= options.Value.MaximumItemsPerSecond);
        }
    }
}
