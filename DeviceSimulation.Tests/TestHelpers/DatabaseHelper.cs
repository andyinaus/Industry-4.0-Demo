using Dapper;
using Microsoft.Data.Sqlite;
using Industry.Simulation.Core.Infrastructures;
using Moq;

namespace DeviceSimulation.Tests.TestHelpers
{
    public class DatabaseHelper
    {
        private const string CreateDevicesScript = "CREATE TABLE Devices ([ID] varchar(36) NOT NULL PRIMARY KEY, [Type] varchar(100) NOT NULL)";
        private const string CreateDeviceReadingsScript = "CREATE TABLE DeviceReadings ([DeviceID] varchar(36) NOT NULL," +
            " [DateTime] DateTime NOT NULL, [Speed] INT NOT NULL, [PackageTrackingAlarmState] varchar(10) NOT NULL," +
            " [CurrentBoards] INT NOT NULL, [CurrentRecipeCount] INT NOT NULL, PRIMARY KEY([DeviceID], [DateTime]), FOREIGN KEY([DeviceID]) REFERENCES Devices(ID))";

        private const string InMemoryConnectionString = "Filename=Test; Mode=Memory; Cache=Shared";

        public static IConnectionFactory GetMockConnectionFactory()
        {
            var mockFactory = new Mock<IConnectionFactory>();

            mockFactory.Setup(m => m.CreateConnectionAsync())
                .ReturnsAsync(() =>
                {
                    var connection = new SqliteConnection(InMemoryConnectionString);

                    connection.Open();

                    return connection;
                });

            return mockFactory.Object;
        }

        public static void CreateTablesIfNotExist()
        {
            var factory = GetMockConnectionFactory();

            using (var connection = factory.CreateConnectionAsync().Result)
            {
                connection.Execute(CreateDevicesScript);
                connection.Execute(CreateDeviceReadingsScript);
            }
        }
    }
}
