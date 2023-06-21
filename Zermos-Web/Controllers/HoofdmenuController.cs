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

        public IActionResult Laadscherm()
        {
            ViewData["laad_tekst"] = "Bezig met laden";
            return View("_Loading");
        }
    }
}