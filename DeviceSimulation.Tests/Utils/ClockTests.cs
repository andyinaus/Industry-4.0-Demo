using System;
using DeviceSimulation.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DeviceSimulation.Tests.Utils
{
    [TestClass]
    public class ClockTests
    {
        [TestMethod]
        public void NowShouldReturnTheExpectedValue()
        {
            var datetime = DateTime.Today;
            var clock = new Clock(datetime);

            Assert.AreEqual(datetime, clock.Now());
        }
    }
}
