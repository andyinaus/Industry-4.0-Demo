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
        public void SimulateWhenSimulatorOptionsHaveNoSettingsShouldThrowArgumentNullException()
        {
            var options = Options.Create(new SimulatorSettingsOptions
            {
                Settings = null
            });

            Action target = () => new ConveyorSimulator("1234", new Clock(DateTime.Now), options);

            Assert.ThrowsException<ArgumentNullException>(target);
        }

        [TestMethod]
        public void SimulateWithValidSimulatorOptionsShouldSimulateAccordingly()
        {
            var options = Options.Create(new SimulatorSettingsOptions
            {
                Settings = new SimulatorSettingsOptions.SimulatorSettings
                {
                    MaximumItemsPerSecond = 4,
                    SpeedMax = 10,
                    SpeedMin = 5
                }
            });

            var simulator = new ConveyorSimulator("1234", new Clock(DateTime.Now), options);

            simulator.Simulate();

            Assert.IsTrue(simulator.Speed >= options.Value.Settings.SpeedMin);
            Assert.IsTrue(simulator.Speed <= options.Value.Settings.SpeedMax);
            Assert.IsTrue(simulator.CurrentRecipeCount <= options.Value.Settings.MaximumItemsPerSecond);
            Assert.IsTrue(simulator.CurrentTotalBoards <= options.Value.Settings.MaximumItemsPerSecond);
        }
    }
}
