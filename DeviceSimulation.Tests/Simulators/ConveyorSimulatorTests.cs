using System;
using DeviceSimulation.Simulators;
using DeviceSimulation.Simulators.Options;
using DeviceSimulation.Utils;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DeviceSimulation.Tests.Simulators
{
    [TestClass]
    public class ConveyorSimulatorTests
    {
        [TestMethod]
        public void CtorWhenClockIsNullShouldThrowArgumentNullException()
        {
            Action target = () => new ConveyorSimulator(null, Options.Create(new ConveyorSimulatorOptions()));

            Assert.ThrowsException<ArgumentNullException>(target);
        }

        [TestMethod]
        public void CtorWhenSimulatorOptionsIsNullShouldThrowArgumentNullException()
        {
            Action target = () => new ConveyorSimulator(new Clock(DateTime.Now), null);

            Assert.ThrowsException<ArgumentNullException>(target);
        }

        [TestMethod]
        public void SimulateWithCustomOptionsShouldSimulateAccordingly()
        {
            var options = Options.Create(new ConveyorSimulatorOptions
            {
                MaximumItemsPerSecond = 4,
                SpeedMax = 10,
                SpeedMin = 5
            });

            var simulator = new ConveyorSimulator(new Clock(DateTime.Now), options);

            simulator.Simulate();

            Assert.IsTrue(simulator.Speed >= options.Value.SpeedMin);
            Assert.IsTrue(simulator.Speed <= options.Value.SpeedMax);
            Assert.IsTrue(simulator.CurrentRecipeCount <= options.Value.MaximumItemsPerSecond);
            Assert.IsTrue(simulator.CurrentTotalBoards <= options.Value.MaximumItemsPerSecond);
        }
    }
}
