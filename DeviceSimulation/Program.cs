using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using DeviceSimulation.Clients;
using DeviceSimulation.Clients.Options;
using DeviceSimulation.Simulators;
using DeviceSimulation.Simulators.Options;
using DeviceSimulation.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace DeviceSimulation
{
    public class Program
    {
        private static readonly IConfigurationRoot Configuration = BuildConfiguration();

        private const string IoTPlatformSectionName = "IoTPlatform";
        private const string HttpClientSectionName = "HttpClient";
        private const string SimulatorSettingsSectionName = "SimulatorSettings";
        private const string RequiredSimulatorsSectionName = "RequiredSimulators";

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

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
            services.Configure<IoTPlatformOptions>(Configuration.GetSection(IoTPlatformSectionName));
            services.Configure<HttpClientOptions>(Configuration.GetSection(HttpClientSectionName));
            services.Configure<SimulatorSettingsOptions>(Configuration.GetSection(SimulatorSettingsSectionName));
            services.Configure<RequiredSimulatorsOptions>(Configuration.GetSection(RequiredSimulatorsSectionName));

            services.AddSingleton(BuildLogger());

            services.AddSingleton(new HttpClient());
            services.AddSingleton<IoTHttpClient>();
            services.AddSingleton<IClock>(new Clock(DateTime.Now));
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

            var client = serviceProvider.GetService<IoTHttpClient>();
            var timer = new Timer(1000);

            var simulatedConveyors = new[]
            {
                serviceProvider.GetService<ConveyorSimulator>(),
                serviceProvider.GetService<ConveyorSimulator>(),
                serviceProvider.GetService<ConveyorSimulator>()
            };

            timer.Elapsed += async (sender, eventArgs) =>
            {
                var tasks = simulatedConveyors.Select(s => client.SendSimulatedDeviceDataAsync(s.Simulate()));
                await Task.WhenAll(tasks);
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
