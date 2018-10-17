using System;
using System.Data;
using Industry.Simulation.Core.Infrastructures;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Industry.Simulation.Core.Tests.Infrastructures
{
    [TestClass]
    public class ConnectionFactoryTests
    {
        private const string ValidConnectionString = "Server=.;Integrated Security=true;";

        [TestMethod]
        public void CtorWhenConfigurationIsNullShouldThrowArgumentNullException()
        {
            Action target = () => new ConnectionFactory(null);

            Assert.ThrowsException<ArgumentNullException>(target);
        }

        [TestMethod]
        public void CreateConnectionAsyncWhenConfigurationIsValidShouldReturnNewOpenConnection()
        {
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(m => m.GetSection("ConnectionStrings")[It.IsAny<string>()])
                .Returns(ValidConnectionString);

            var factory = new ConnectionFactory(mockConfig.Object);

            var conn = factory.CreateConnectionAsync().Result;

            Assert.AreEqual(ConnectionState.Open, conn.State);
            Assert.AreNotSame(conn, factory.CreateConnectionAsync().Result);
        }
    }
}
