﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        ///        "CurrentTotalBoards": 3
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
                CurrentTotalBoards = reading.CurrentTotalBoards,
                CurrentRecipeCount = reading.CurrentRecipeCount
            });

            return Created(string.Empty, reading);
        }

        /// <summary>
        /// Gets All DeviceReadings With the Given ID.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /Device
        ///     {
        ///        "id": "12345"
        ///     }
        ///
        /// </remarks>
        /// <param name="id"></param>
        /// <returns>All device readings with the given id</returns>
        /// <response code="201">Returns all device readings with the given id</response>
        /// <response code="400">If the id is invalid</response>  
        /// <response code="404">If no device readings found with the given id</response>    
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<DeviceReadingModel>>> GetAllByIdAsync([Required] string id)
        {
            var results = await _deviceReadingRepository.GetAllReadingsByIdAsync(id);

            var deviceReadings = results.Select(r => new DeviceReadingModel
            {
                Id = r.DeviceId,
                CurrentRecipeCount = r.CurrentRecipeCount,
                CurrentTotalBoards = r.CurrentTotalBoards,
                DateTime = r.DateTime,
                PackageTrackingAlarmState = r.PackageTrackingAlarmState,
                Speed = r.Speed
            }).ToList();

            if (!deviceReadings.Any()) return NotFound();

            return deviceReadings;
        }
    }
}