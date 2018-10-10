using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DeviceSimulation.Clients.Options;
using DeviceSimulation.Simulators;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;

namespace DeviceSimulation.Clients
{
    public class IoTHttpClient : IDisposable
    {
        private readonly IoTPlatformOptions _ioTOptions;
        private readonly HttpClientOptions _httpOptions;
        private readonly ILogger _logger;
        private readonly HttpClient _client;

        public IoTHttpClient(IOptions<IoTPlatformOptions> ioTOptions, IOptions<HttpClientOptions> httpOptions, ILogger logger, HttpClient client)
        {
            _httpOptions = httpOptions?.Value ?? throw new ArgumentNullException(nameof(httpOptions));
            _ioTOptions = ioTOptions?.Value ?? throw new ArgumentNullException(nameof(ioTOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));

            _client.BaseAddress = new Uri(_ioTOptions.BaseAddress);
        }
        
        public async Task<HttpResponseMessage> SendSimulatedDeviceDataAsync(ConveyorSimulator simulator)
        {
            if (simulator == null) throw new ArgumentNullException(nameof(simulator));

            HttpResponseMessage response = null;

            var content = new StringContent(JsonConvert.SerializeObject(simulator), Encoding.UTF8,
                "application/json");

            for (var i = 0; i <= _httpOptions.NumberOfRetries; i++)
            {
                try
                {
                    response = await _client.PutAsync(_ioTOptions.RelativeSendingDataUrl, content);

                    response.EnsureSuccessStatusCode();
                    Console.WriteLine($"Successfully sent for device {simulator.SerialNumber} with speed {simulator.Speed}.");
                    break;
                }
                catch (HttpRequestException exception)
                {
                    if (i == _httpOptions.NumberOfRetries)
                    {
                        _logger.Warning($"Error at Device {simulator.SerialNumber}: {exception.Message}");
                    }
                    else
                    {
                        Console.WriteLine($"{_httpOptions.NumberOfRetries - i} Attempts remaining ...");
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
