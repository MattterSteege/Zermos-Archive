using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Zermos_Web.Controllers
{
    public class HoofdmenuController : Controller
    {
        private readonly ILogger<HoofdmenuController> _logger;

        public HoofdmenuController(ILogger<HoofdmenuController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}