using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Zermos_Web.Models.Requirements;
using Zermos_Web.Utilities;

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
        public IActionResult Laadscherm()
        {
            ViewData["laad_tekst"] = "Bezig met laden";
            return View("_Loading");
        }
        #endif
        
        [AddLoadingScreen("hoofdmenu laden...")]
        public async Task<IActionResult> Index()
        {
            ViewData["add_css"] = "hoofdmenu";
            HttpContext.AddNotification("Pas op", "Deze pagina is no niet af, er kunnen nog bugs in zitten", NotificationCenter.NotificationType.WARNING);
            return View(await _users.GetUserAsync(User.FindFirstValue("email")));
        }

        //to send the correct deeplink format do this: location.href = 'web+zermos://' + url;
        [Route("/Deeplink")]
        public IActionResult Deeplink(string data = null)
        {
            if (data == null)
            {
                data = System.Net.WebUtility.UrlDecode(Request.QueryString.Value);
            }
            if (data != null)
            {
                data = data.Replace("web+zermos://", "");
                if (data[data.Length - 1] == '/')
                {
                    data = data.Substring(0, data.Length - 1);
                }
                
                return Redirect(data);
            }
            return Redirect("/");
        }
        
        [Route("/serviceworker.js")]
        public IActionResult ServiceWorker()
        {
            return File("~/serviceworker.js", "text/javascript");
        }
        
        [Route("/Offline")]
        public IActionResult Offline()
        {
            return Ok("Je bent offline en deze pagina is niet gecached");
        }
        
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