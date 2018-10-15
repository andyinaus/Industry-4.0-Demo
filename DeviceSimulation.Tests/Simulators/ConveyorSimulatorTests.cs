using System;
using DeviceSimulation.Simulation;
using DeviceSimulation.Simulation.Options;
using DeviceSimulation.Utils;
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
            Action target = () => new ConveyorSimulator(null, new Clock(DateTime.Now), Options.Create(new SimulatorSettingsOptions()));

            Assert.ThrowsException<ArgumentNullException>(target);
        }

        [TestMethod]
        public void CtorWhenIdIsWhiteSpaceShouldThrowArgumentNullException()
        {
            Action target = () => new ConveyorSimulator(" ", new Clock(DateTime.Now), Options.Create(new SimulatorSettingsOptions()));

            Assert.ThrowsException<ArgumentNullException>(target);
        }

        [TestMethod]
        public void CtorWhenClockIsNullShouldThrowArgumentNullException()
        {
            Action target = () => new ConveyorSimulator("123", null, Options.Create(new SimulatorSettingsOptions()));

            Assert.ThrowsException<ArgumentNullException>(target);
        }

        [TestMethod]
        public void CtorWhenSimulatorOptionsIsNullShouldThrowArgumentNullException()
        {
            Action target = () => new ConveyorSimulator("1234", new Clock(DateTime.Now), null);

            Assert.ThrowsException<ArgumentNullException>(target);
        }

        [TestMethod]
        public void SimulateWithValidSimulatorOptionsShouldSimulateAccordingly()
        {
            var options = Options.Create(new SimulatorSettingsOptions
            {
                MaximumItemsPerSecond = 4,
                SpeedMax = 10,
                SpeedMin = 5
            });

            var simulator = new ConveyorSimulator("1234", new Clock(DateTime.Now), options);

            simulator.Simulate();

            Assert.IsTrue(simulator.Speed >= options.Value.SpeedMin);
            Assert.IsTrue(simulator.Speed <= options.Value.SpeedMax);
            Assert.IsTrue(simulator.CurrentRecipeCount <= options.Value.MaximumItemsPerSecond);
            Assert.IsTrue(simulator.CurrentBoards <= options.Value.MaximumItemsPerSecond);
        }
    }
}
