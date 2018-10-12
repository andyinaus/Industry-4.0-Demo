using Microsoft.AspNetCore.Mvc;

namespace IoTPlatform.Controllers
{
    public class MonitorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}