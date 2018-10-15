using System;
using DeviceSimulation.Factories;
using DeviceSimulation.Simulation.Options;
using DeviceSimulation.Utils;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DeviceSimulation.Tests.Factories
{
    [TestClass]
    public class SimulatorFactoryTests
    {
        [TestMethod]
        public void CtorWhenClockIsNullShouldThrowArgumentNullException()
        {
            Action target = () => new SimulatorFactory(CreateDummySimulatorSettingsOptions(), null);

            Assert.ThrowsException<ArgumentNullException>(target);
        }

        [TestMethod]
        public void CtorWhenSimulatorSettingsOptionsIsNullShouldThrowArgumentNullException()
        {
            Action target = () => new SimulatorFactory(null, new Clock(DateTime.Now));

            Assert.ThrowsException<ArgumentNullException>(target);
        }

        [TestMethod]
        public void CreateSimulatorWhenIdIsNullShouldThrowArgumentNullException()
        {
            var factory = new SimulatorFactory(CreateDummySimulatorSettingsOptions(), new Clock(DateTime.Now));

            Action target = () => factory.CreateSimulator(null);

            Assert.ThrowsException<ArgumentNullException>(target);
        }

        [TestMethod]
        public void CreateSimulatorWhenIdIsWhiteSpaceShouldThrowArgumentNullException()
        {
            var factory = new SimulatorFactory(CreateDummySimulatorSettingsOptions(), new Clock(DateTime.Now));

            Action target = () => factory.CreateSimulator(" ");

            Assert.ThrowsException<ArgumentNullException>(target);
        }

        [TestMethod]
        public void CreateSimulatorWhenIdIsValidShouldReturnSimulatorWithExpectedId()
        {
            var factory = new SimulatorFactory(CreateDummySimulatorSettingsOptions(), new Clock(DateTime.Now));

            const string deviceId = "123456";

            var simulator = factory.CreateSimulator(deviceId);

            Assert.AreEqual(deviceId, simulator.Id);
        }

        private static IOptions<SimulatorSettingsOptions> CreateDummySimulatorSettingsOptions()
        {
            return Options.Create(new SimulatorSettingsOptions());
        }
    }
}
