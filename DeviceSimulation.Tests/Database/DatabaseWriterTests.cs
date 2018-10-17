using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DeviceSimulation.Database;
using DeviceSimulation.Simulation;
using DeviceSimulation.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Serilog;

namespace DeviceSimulation.Tests.Database
{
    [TestClass]
    public class DatabaseWriterTests
    {
        [TestMethod]
        public void CtorWhenConnectionFactoryIsNullShouldThrowArgumentNullException()
        {
            Action target = () => new DatabaseWriter(null, Mock.Of<ILogger>());

            Assert.ThrowsException<ArgumentNullException>(target);
        }

        [TestMethod]
        public void CtorWhenLoggerIsNullShouldThrowArgumentNullException()
        {
            Action target = () => new DatabaseWriter(DatabaseHelper.GetMockConnectionFactory(), null);

            Assert.ThrowsException<ArgumentNullException>(target);
        }

        [TestMethod]
        public void WriteAsyncWhenResultIsNullShouldThrowArgumentNullException()
        {
            var writer = new DatabaseWriter(DatabaseHelper.GetMockConnectionFactory(), Mock.Of<ILogger>());

            Func<Task> target = () => writer.WriteAsync(null);

            Assert.ThrowsExceptionAsync<ArgumentNullException>(target);
        }

        [TestMethod]
        public void WriteAsyncWhenResultIsValidShouldWriteToDatabase()
        {
            var factory = DatabaseHelper.GetMockConnectionFactory();
            using (var connection = factory.CreateConnectionAsync().Result)
            {
                DatabaseHelper.CreateTablesIfNotExist();
                var deviceId = AddDummyDevviceToInMemoryDatabase();

                var result = new ConveyorSimulator.SimulationResult
                {
                    Id = deviceId,
                    Speed = 213,
                    DateTime = DateTime.UtcNow,
                    PackageTrackingAlarmState = PackageTrackingAlarmState.Okay,
                    CurrentRecipeCount = 2,
                    CurrentBoards = 3
                };

                var writer = new DatabaseWriter(factory, Mock.Of<ILogger>());

                writer.WriteAsync(result).Wait();

                var results = connection.Query("SELECT * FROM DeviceReadings");

                Assert.AreEqual(1, results.Count());
            }
        }

        private static string AddDummyDevviceToInMemoryDatabase()
        {
            var device = new
            {
                Id = "33333",
                Type = "Dummy"
            };

            using (var connection = DatabaseHelper.GetMockConnectionFactory().CreateConnectionAsync().Result)
            {
                connection.Execute("INSERT INTO Devices VALUES(@Id, @Type)", device);
            }

            return device.Id;
        }
    }
}
