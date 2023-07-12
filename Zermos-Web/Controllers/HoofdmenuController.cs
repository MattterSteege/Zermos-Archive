using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zermos_Web.Models.Requirements;

namespace Zermos_Web.Controllers
{
    [Route("[action]")]
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
        
        public IActionResult Docs()
        {
            return View();
        }
    }
}