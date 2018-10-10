using System;
using System.ComponentModel.DataAnnotations;
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
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceRepository _deviceRepository;

        public DeviceController(IDeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));
        }

        /// <summary>
        /// Creates a Device.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Device
        ///     {
        ///        "Type": "Conveyor"
        ///     }
        ///
        /// </remarks>
        /// <param name="device"></param>
        /// <returns>A newly created device</returns>
        /// <response code="201">Returns the newly created device</response>
        /// <response code="400">If the devie is null</response>           
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<JsonResult>> CreateAsync(DeviceModel device)
        {
            var deviceId = await _deviceRepository.AddAsync(new Device(device.Type));

            return Created(string.Empty, new {Id = deviceId});
        }

        /// <summary>
        /// Gets a Device.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /Device
        ///     {
        ///        "id": "Conveyor"
        ///     }
        ///
        /// </remarks>
        /// <param name="id"></param>
        /// <returns>A device with the given id</returns>
        /// <response code="201">Returns device with the given id</response>
        /// <response code="400">If the id is invalid</response>  
        /// <response code="404">If no device found with the given id</response>    

        [HttpGet]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<DeviceModel>> GetByIdAsync([Required] string id)
        {
            var device = await _deviceRepository.GetByIdAsync(id);

            if (device == null) return NotFound();

            return new DeviceModel
            {
                Id = device.Id,
                Type = device.Type
            };
        }
    }
}