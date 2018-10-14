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
        /// Create a DeviceReading.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /DeviceReading
        ///     {
        ///        "Id": "1234",
        ///        "DateTime": "2018/10/10 P.M. 01:02:01",
        ///        "PackageTrackingAlarmState": "Okay",
        ///        "Speed": 3,
        ///        "CurrentRecipeCount": 3,
        ///        "CurrentBoards": 3
        ///     }
        ///
        /// </remarks>
        /// <param name="reading"></param>
        /// <returns>A newly created device reading</returns>
        /// <response code="201">Returns the newly created device reading</response>
        /// <response code="400">If the reading is null or reading.Id does not exist</response>           
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<JsonResult>> CreateAsync(DeviceReadingModel reading)
        {
            if (await _deviceRepository.GetByIdAsync(reading.Id) == null)
                return BadRequest(new {Error = $"No Device found with the given Id '{reading.Id}'."});

            await _deviceReadingRepository.AddAsync(new DeviceReading
            {
                DeviceId = reading.Id,
                DateTime = reading.DateTime,
                Speed = reading.Speed,
                PackageTrackingAlarmState = reading.PackageTrackingAlarmState,
                CurrentBoards = reading.CurrentBoards,
                CurrentRecipeCount = reading.CurrentRecipeCount
            });

            return Created(string.Empty, reading);
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
                DateTime = r.DateTime,
                Speed = r.Speed,
                PackageTrackingAlarmState = r.PackageTrackingAlarmState,
                TotalBoards = r.TotalBoards,
                TotalRecipeCount = r.TotalRecipeCount
            }).ToList();
        }
    }
}