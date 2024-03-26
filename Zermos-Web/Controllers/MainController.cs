using System.Text.Json;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zermos_Web.Models.Requirements;
using Zermos_Web.Utilities;

namespace Zermos_Web.Controllers
{
    public class MainController : BaseController
    {
        public MainController(Users user, Shares share, ILogger<BaseController> logger) : base(user, share, logger) { }
        
        public IActionResult Index(string url = null)
        {
            var user = ZermosUser;
            
            if (url != null)
                ViewData["url"] = url;
            
            else if (Request.Cookies["this_session_last_page"] != null)
            {
                ViewData["url"] = Request.Cookies["this_session_last_page"];
                Response.Cookies.Delete("this_session_last_page");
            }
            else
                ViewData["url"] = user.default_page ?? "/Zermelo/Rooster";
            
            if (ViewData["url"] != null && ViewData["url"].ToString().IsNullOrEmpty())
                ViewData["url"] = user.default_page ?? "/Zermelo/Rooster";
            else if (ViewData["url"] == null)
                ViewData["url"] = user.default_page ?? "/Zermelo/Rooster";
            
            Response.Cookies.Append("version_used", user.version_used ?? "0");
            Response.Cookies.Append("hand_side", user.hand_side ?? "right");
            Response.Cookies.Append("theme", user.theme ?? "light");
            Response.Cookies.Append("font_size", user.font_size ?? "1");

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

        [ZermosPage]
        [Route("/EersteKeer")]
        public IActionResult EersteKeer()
        {
            return PartialView();
        }
    }
}