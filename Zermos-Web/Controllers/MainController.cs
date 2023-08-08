using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Zermos_Web.Models.Requirements;

namespace Zermos_Web.Controllers
{
    //[Route("[action]")]
    public class MainController : Controller
    {
        private readonly ILogger<MainController> _logger;
        private readonly Users _users;

        public MainController(ILogger<MainController> logger, Users users)
        {
            _logger = logger;
            _users = users;
        }
        
        #if DEBUG
        [AddLoadingScreen("hoofdmenu laden...")]
        public async Task<IActionResult> Index()
        {
            ViewData["add_css"] = "hoofdmenu";
            return View(await _users.GetUserAsync(User.FindFirstValue("email")));
        }

        public IActionResult Laadscherm()
        {
            ViewData["laad_tekst"] = "Bezig met laden";
            return View("_Loading");
        }
        #elif RELEASE
        public async Task<IActionResult> Index()
        {
            return RedirectToAction("Rooster", "Zermelo");
        }
        #endif
        
        [Route(".well-known/microsoft-identity-association.json")]
        public IActionResult MicrosoftIdentityAssociation()
        {
            return Json(new
            {
                associatedApplications = new[]
                {
                    new
                    {
                        applicationId = "REDACTED_MS_CLIENT_ID"
                    }
                }
            }, new JsonSerializerOptions {WriteIndented = true });
        }
    }
}