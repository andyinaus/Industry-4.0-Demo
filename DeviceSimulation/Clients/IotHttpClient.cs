using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DeviceSimulation.Clients.Options;
using DeviceSimulation.Simulators;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DeviceSimulation.Clients
{
    public class IoTHttpClient : IDisposable
    {
        private readonly ILogger _logger;
        private readonly HttpClient _client;

        public IoTHttpClient(IOptions<IoTPlatformOptions> ioTOptions, IOptions<HttpClientOptions> httpOptions, ILogger<IoTHttpClient> logger, HttpClient client)
        {
            HttpOptions = httpOptions?.Value ?? throw new ArgumentNullException(nameof(httpOptions));
            IoTOptions = ioTOptions?.Value ?? throw new ArgumentNullException(nameof(ioTOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));

            _client.BaseAddress = new Uri(IoTOptions.BaseAddress);
        }
        
        public HttpClientOptions HttpOptions { get; private set; }

        public IoTPlatformOptions IoTOptions { get; private set; }

        public async Task<HttpResponseMessage> SendSimulatedDeviceDataAsync(ConveyorSimulator simulator)
        {
            if (simulator == null) throw new ArgumentNullException(nameof(simulator));

            HttpResponseMessage response = null;

            var content = new StringContent(JsonConvert.SerializeObject(simulator), Encoding.UTF8,
                "application/json");

            for (var i = 1; i <= HttpOptions.NumberOfRetries; i++)
            {
                try
                {
                    response = await _client.PutAsync(IoTOptions.RelativeSendingDataUrl, content);

                    response.EnsureSuccessStatusCode();
                    Console.WriteLine($"Successfully sent for device {simulator.SerialNumber} with speed {simulator.Speed}.");
                    break;
                }
                catch (HttpRequestException exception)
                {
                    if (i == HttpOptions.NumberOfRetries)
                    {
                        _logger.LogWarning($"Error at Device {simulator.SerialNumber}: {exception.Message}");
                    }
                    else
                    {
                        Console.WriteLine($"{HttpOptions.NumberOfRetries - i} Attempts remaining ...");
                    }
                }
            }

            return response;
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
