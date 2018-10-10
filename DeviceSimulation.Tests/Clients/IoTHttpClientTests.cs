using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DeviceSimulation.Clients;
using DeviceSimulation.Clients.Options;
using DeviceSimulation.Simulators;
using DeviceSimulation.Simulators.Options;
using DeviceSimulation.Utils;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Serilog;

namespace DeviceSimulation.Tests.Clients
{
    [TestClass]
    public class IoTHttpClientTests
    {
        private static ILogger _logger;
        private static IOptions<HttpClientOptions> _httpOptions;
        private static IOptions<IoTPlatformOptions> _ioTPlatformOptions;
        private static Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private static HttpClient _client;

        [TestInitialize]
        public void Init()
        {
            _logger = Mock.Of<ILogger>();
            _httpOptions = Options.Create(new HttpClientOptions());
            _ioTPlatformOptions = Options.Create(new IoTPlatformOptions
            {
                BaseAddress = "http://localhost:8080"
            });
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            
            _client = new HttpClient(_httpMessageHandlerMock.Object);
        }

        [TestMethod]
        public void CtorWhenIoTOptionsIsNullShouldThrowArgumentNullException()
        {
            Action target = () => new IoTHttpClient(null, _httpOptions, _logger, _client);

            Assert.ThrowsException<ArgumentNullException>(target);
        }

        [TestMethod]
        public void CtorWhenHttpOptionsIsNullShouldThrowArgumentNullException()
        {
            Action target = () => new IoTHttpClient(_ioTPlatformOptions, null, _logger, _client);

            Assert.ThrowsException<ArgumentNullException>(target);
        }

        [TestMethod]
        public void CtorWhenILoggerIsNullShouldThrowArgumentNullException()
        {
            Action target = () => new IoTHttpClient(_ioTPlatformOptions, _httpOptions, null, _client);

            Assert.ThrowsException<ArgumentNullException>(target);
        }

        [TestMethod]
        public void CtorWhenHttpClientIsNullShouldThrowArgumentNullException()
        {
            Action target = () => new IoTHttpClient(_ioTPlatformOptions, _httpOptions, _logger, null);

            Assert.ThrowsException<ArgumentNullException>(target);
        }

        [TestMethod]
        public void CtorWithCorrectDetailsShouldHaveInitializeProperly()
        {
            const string baseAddress = "http://localhost:8080";

            _ioTPlatformOptions.Value.BaseAddress = baseAddress;

            var client = new IoTHttpClient(_ioTPlatformOptions, _httpOptions, _logger, _client);

            Assert.AreEqual(new Uri(baseAddress), _client.BaseAddress);
        }

        [TestMethod]
        public void SendSimulatedDeviceDataAsyncWhenSimulatorIsNullShouldThrowArgumentNullException()
        {
            var client = new IoTHttpClient(_ioTPlatformOptions, _httpOptions, _logger, _client);

            Func<Task> target = async () => await client.SendSimulatedDeviceDataAsync(null);

            Assert.ThrowsExceptionAsync<ArgumentNullException>(target);
        }

        [TestMethod]
        public void SendSimulatedDeviceDataAsyncWithProperRelativeSendingDataUrlShouldSendToCorrectDest()
        {
            SetResponseForHttpMessageHandlerMock();
            _ioTPlatformOptions.Value.BaseAddress = "http://localhost:9090";
            _ioTPlatformOptions.Value.RelativeSendingDataUrl = "/test/123";

            var client = new IoTHttpClient(_ioTPlatformOptions, _httpOptions, _logger, _client);
            var simulator = new ConveyorSimulator(new Clock(DateTime.Now), Options.Create(new ConveyorSimulatorOptions()));

            client.SendSimulatedDeviceDataAsync(simulator).Wait();

            var expectedDestination = new Uri($"{_ioTPlatformOptions.Value.BaseAddress}{_ioTPlatformOptions.Value.RelativeSendingDataUrl}");

            _httpMessageHandlerMock.Protected().Verify("SendAsync", Times.Once(), 
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Put && req.RequestUri == expectedDestination),
                ItExpr.IsAny<CancellationToken>());
        }

        [TestMethod]
        public void SendSimulatedDeviceDataAsyncWhenNoOfRetriesIsFiveShouldSendExactlySixTimes()
        {
            SetResponseForHttpMessageHandlerMock(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadGateway
            });
            _httpOptions.Value.NumberOfRetries = 5;

            var client = new IoTHttpClient(_ioTPlatformOptions, _httpOptions, _logger, _client);
            var simulator = new ConveyorSimulator(new Clock(DateTime.Now), Options.Create(new ConveyorSimulatorOptions()));

            client.SendSimulatedDeviceDataAsync(simulator).Wait();

            _httpMessageHandlerMock.Protected().Verify("SendAsync", Times.Exactly(6),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        private static void SetResponseForHttpMessageHandlerMock(HttpResponseMessage message = null)
        {
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(message ?? new HttpResponseMessage());
        }
    }
}
