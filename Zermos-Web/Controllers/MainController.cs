using System;
using System.Diagnostics;
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
        
        private JsMinifier _jsMinifier = new JsMinifier();
        
        public IActionResult Index(string url = null)
        {
            var user = ZermosUser ?? new Infrastructure.Entities.user();
            
            if (User.Identity is not {IsAuthenticated: true} && Request.Cookies["version_used"] == null && (url == null || !url.ToLower().Contains("gedeeld")))
            {
                url = "/Intro";
                Response.Cookies.Append("seen_intro", "true", new Microsoft.AspNetCore.Http.CookieOptions
                {
                    Expires = DateTime.Now.AddYears(1)
                });
            }
            
            if (url != null)
            {
                ViewData["url"] = url;
            }
            else if (Request.Cookies["this_session_last_page"] != null && Request.Cookies["this_session_last_page"] != "/")
            {
                ViewData["url"] = Request.Cookies["this_session_last_page"];
                Response.Cookies.Delete("this_session_last_page");
            }
            else
            {
                ViewData["url"] = user.default_page ?? "/Zermelo/Rooster";
            }

            if (ViewData["url"] != null && string.IsNullOrEmpty(ViewData["url"].ToString()))
            {
                ViewData["url"] = user.default_page ?? "/Zermelo/Rooster";
            }
            else if (ViewData["url"] == null)
            {
                ViewData["url"] = user.default_page ?? "/Zermelo/Rooster";
            }

            Response.Cookies.Append("version_used", user.version_used ?? "0");
            Response.Cookies.Append("hand_side", user.hand_side ?? "right");
            Response.Cookies.Append("theme", user.theme ?? "light");
            Response.Cookies.Append("font_size", user.font_size ?? "1");
            ViewData["isZermeloGekoppeld"] = user.zermelo_access_token_expires_at > DateTime.Now;
            ViewData["isInfowijsGekoppeld"] = TokenUtils.CheckToken(user.infowijs_access_token);
            ViewData["isSomtodayGekoppeld"] = TokenUtils.CheckToken(user.somtoday_refresh_token);
            ViewData["forceShow"] = false;

            return View();
        }

        [Route("/data/sidebar")]
        public IActionResult Sidebar(bool forceShow = false)
        {
            var user = ZermosUser;
            ViewData["isZermeloGekoppeld"] = user.zermelo_access_token_expires_at > DateTime.Now;
            ViewData["isInfowijsGekoppeld"] = TokenUtils.CheckToken(user.infowijs_access_token);
            ViewData["isSomtodayGekoppeld"] = TokenUtils.CheckToken(user.somtoday_refresh_token);
            ViewData["forceShow"] = forceShow;
            return PartialView();
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
        public IActionResult ServiceWorker(bool minify = true)
        {
            string content = System.IO.File.ReadAllText("wwwroot/js/serviceworker.js");
            
            if (!minify)
                return Content(content.Replace("${ZERMOSVERSION}", CurrentZermosVersion), "text/javascript");
            
            return Content(_jsMinifier.Minify(content).Replace("${ZERMOSVERSION}", CurrentZermosVersion), "text/javascript");
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
        
        [ZermosPage]
        [Route("/Intro")]
        public IActionResult Intro()
        {
            return PartialView();
        }
        
        [Route("/api/test")]
        public IActionResult Test()
        {
            //in the body:
            //timesend = new Date().getTime();
            
            DateTime timeRequestRecieved = DateTime.UtcNow;
            DateTime timeRequestSend = Request.Form.ContainsKey("timesend") ? new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(long.Parse(Request.Form["timesend"])) : DateTime.MinValue;
            
            var stopwatch = Stopwatch.StartNew();
            var user = ZermosUser;
            stopwatch.Stop();
            
            Console.WriteLine($"User: {user.email} initiated api test\n" +
                              $"Request send at: {timeRequestSend}\n" +
                              $"Request recieved at: {timeRequestRecieved}\n" +
                              $"Response send at: {DateTime.UtcNow}\n" +
                              $"DataBase query took: {stopwatch.ElapsedMilliseconds}ms");
            
            return Json(new
            {
                timeRequestRecieved,
                timeRequestSend,
                timeResponseSend = DateTime.UtcNow,
                timeQuery = stopwatch.ElapsedMilliseconds
            }, new JsonSerializerOptions {WriteIndented = true});
        }
    }
}