using System.Text.Json;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zermos_Web.Models.Requirements;

namespace Zermos_Web.Controllers
{
    //[Route("[action]")]
    public class MainController : BaseController
    {
        public MainController(Users user, ILogger<BaseController> logger) : base(user, logger) { }
        
        public IActionResult Index(string url = null)
        {
            if ((User.Identity == null || !User.Identity.IsAuthenticated) && url == null)
                return PartialView("ZermosPromo");
            
            if (url != null)
                ViewData["url"] = url;
            else if (Request.Cookies["this_session_last_page"] != null)
            {
                ViewData["url"] = Request.Cookies["this_session_last_page"];
                Response.Cookies.Delete("this_session_last_page");
            }
            else
                ViewData["url"] = Request.Cookies["default_page"] ?? "/Zermelo/Rooster";
            
            return View();
        }
        
        [ZermosPage]
        [Authorize]
        [Route("/Hoofdmenu")]
        public IActionResult Hoofdmenu()
        {
            return PartialView(ZermosUser);
        }
        
        [Route("/ZermosPromo")]
        public IActionResult ZermosPromo()
        {
            return View();
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
            return File("~/js/serviceworker.js", "text/javascript");
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