using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using DeviceSimulation.Clients;
using DeviceSimulation.Clients.Options;
using DeviceSimulation.Simulators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace DeviceSimulation
{
    public class Program
    {
        private static readonly IConfigurationRoot Configuration = GetConfiguration();
        

        private const string IoTPlatformSectionName = "IoTPlatform";
        private const string HttpClientSectionName = "HttpClient";

        private static IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            return builder.Build();
        }

        private static void InitializeSerilog()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File($"{Assembly.GetExecutingAssembly().GetName().Name}.log")
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Warning)
                .CreateLogger();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.Configure<IoTPlatformOptions>(Configuration.GetSection(IoTPlatformSectionName));
            services.Configure<HttpClientOptions>(Configuration.GetSection(HttpClientSectionName));

            services.AddSingleton(new LoggerFactory()
                .AddSerilog());
            services.AddLogging();
            InitializeSerilog();

            services.AddSingleton<IoTHttpClient>();
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

            var currentDateTime = DateTime.Now;
            var simulatedConveyors = new[] { new ConveyorSimulator(currentDateTime), new ConveyorSimulator(currentDateTime), new ConveyorSimulator(currentDateTime) };

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
