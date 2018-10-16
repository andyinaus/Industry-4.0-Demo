using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IoTPlatform.Models;
using IoTPlatform.Persistences;
using IoTPlatform.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace IoTPlatform.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceReadingController : ControllerBase
    {
        private readonly IDeviceReadingRepository _deviceReadingRepository;
        private readonly IDeviceRepository _deviceRepository;

        public DeviceReadingController(IDeviceReadingRepository deviceReadingRepository, IDeviceRepository deviceRepository)
        {
            _deviceReadingRepository = deviceReadingRepository ?? throw new ArgumentNullException(nameof(deviceReadingRepository));
            _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));
        }

        /// <summary>
        /// Gets Latest DeviceReadings For All Devices.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /Device
        ///     
        ///
        /// </remarks>
        /// <returns>All device readings</returns>
        /// <response code="200">Returns latest readings for all devices</response>
        /// <response code="404">If no device readings found</response>    
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<LatestDeviceReadingModel>>> GetAllLatestAsync()
        {
            var readings = (await _deviceReadingRepository.GetAllLatestReadingsAsync()).ToList();
            if (!readings.Any()) return NotFound();

            return readings.Select(r => new LatestDeviceReadingModel
            {
                Id = r.DeviceId,
                DeviceType = r.DeviceType,
                DateTime = DateTime.SpecifyKind(r.DateTime, DateTimeKind.Utc),
                Speed = r.Speed,
                PackageTrackingAlarmState = r.PackageTrackingAlarmState,
                TotalBoards = r.TotalBoards,
                TotalRecipeCount = r.TotalRecipeCount
            }).ToList();
        }
    }
}