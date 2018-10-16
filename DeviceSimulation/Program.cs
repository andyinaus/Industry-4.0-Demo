using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using DeviceSimulation.Database;
using DeviceSimulation.Factories;
using DeviceSimulation.Simulation.Options;
using Industry.Simulation.Core.Infrastructures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;

namespace DeviceSimulation
{
    public class Program
    {
        private static readonly IConfigurationRoot Configuration = BuildConfiguration();

        private const string SimulatorSettingsSectionName = "SimulatorSettings";
        private const string RequiredSimulatorsSectionName = "RequiredSimulators";

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }

        private static ILogger BuildLogger()
        {
            return new LoggerConfiguration()
                .ReadFrom
                .Configuration(Configuration)
                .CreateLogger();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SimulatorSettingsOptions>(Configuration.GetSection(SimulatorSettingsSectionName));
            services.Configure<RequiredSimulatorsOptions>(Configuration.GetSection(RequiredSimulatorsSectionName));

            services.AddSingleton(BuildLogger());

            services.AddSingleton(new HttpClient());
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<ISimulatorFactory, SimulatorFactory>();
            services.AddSingleton<IConnectionFactory, ConnectionFactory>();
            services.AddSingleton<IDatabaseWriter, DatabaseWriter>();
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Starting console app ...");
            Console.WriteLine("Configuring ...");

            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            Console.WriteLine("Completed Configuration ...");

            Console.WriteLine("Preparing simulation ...");

            var simulatorFactory = serviceProvider.GetService<ISimulatorFactory>();
            var requiredDeviceIds = serviceProvider.GetService<IOptions<RequiredSimulatorsOptions>>()
                .Value.Simulators.Select(s => s.DeviceId);
            var requiredSimulators = requiredDeviceIds.Select(s => simulatorFactory.CreateSimulator(s));
            var writer = serviceProvider.GetService<IDatabaseWriter>();

            var timer = new Timer(1000);
            timer.Elapsed += (sender, eventArgs) =>
            {
                var now = DateTime.UtcNow;

                Parallel.ForEach(requiredSimulators, async simulator =>
                {
                    var result = simulator.SimulateAt(now);
                    await writer.WriteAsync(result);
                });
            };

            Console.WriteLine("Start Simulating ...");

            timer.Start();

            Console.WriteLine("Press any key to stop simulation ...");
            Console.ReadLine();
            timer.Stop();
            timer.Dispose();
        }
    }
}
