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
        public void SimulateWhenSimulatorOptionsHaveNoSettingsShouldThrowArgumentNullException()
        {
            var options = Options.Create(new ConveyorSimulatorOptions
            {
                Settings = null
            });

            Action target = () => new ConveyorSimulator(new Clock(DateTime.Now), options);

            Assert.ThrowsException<ArgumentNullException>(target);
        }

        [TestMethod]
        public void SimulateWithValidSimulatorOptionsShouldSimulateAccordingly()
        {
            var options = Options.Create(new ConveyorSimulatorOptions
            {
                Settings = new ConveyorSimulatorOptions.SimulatorSettings
                {
                    MaximumItemsPerSecond = 4,
                    SpeedMax = 10,
                    SpeedMin = 5
                }
            });

            var simulator = new ConveyorSimulator(new Clock(DateTime.Now), options);

            simulator.Simulate();

            Assert.IsTrue(simulator.Speed >= options.Value.Settings.SpeedMin);
            Assert.IsTrue(simulator.Speed <= options.Value.Settings.SpeedMax);
            Assert.IsTrue(simulator.CurrentRecipeCount <= options.Value.Settings.MaximumItemsPerSecond);
            Assert.IsTrue(simulator.CurrentTotalBoards <= options.Value.Settings.MaximumItemsPerSecond);
        }
    }
}
