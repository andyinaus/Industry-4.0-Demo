using System;
using System.Linq;
using System.Threading.Tasks;
using IoTPlatform.Models;
using IoTPlatform.Persistences;
using IoTPlatform.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace IoTPlatform.Controllers
{
    public class DeviceController : Controller
    {
        private readonly IDeviceRepository _deviceRepository;

        public DeviceController(IDeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));
        }

        [HttpGet]
        public async Task <IActionResult> Index()
        {
            var devices = await _deviceRepository.GetAllAsync();

            return View(devices.Select(d => new DeviceModel
            {
                Id = d.Id,
                Type = d.Type
            }));
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(DeviceModel device)
        {
            if (!ModelState.IsValid)
                return View();

            await _deviceRepository.AddAsync(new Device(device.Type));

            return RedirectToAction(nameof(Index));
        }
    }
}