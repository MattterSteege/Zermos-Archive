using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zermos_Web.Models.Requirements;
using Zermos_Web.Models.SortedSomtodayGradesModel;
using Zermos_Web.Models.zermelo;
using Zermos_Web.Utilities;

namespace Zermos_Web.Controllers
{
    public class MainController : BaseController
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public MainController(Users user, Shares share, CustomAppointments customCustomAppointment,
            ILogger<BaseController> logger) : base(user, share,
            customCustomAppointment, logger) { }
        
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
            Response.Cookies.Append("prefers_font", user.prefers_font ?? "normal");
            
            ViewData["isZermeloGekoppeld"] = user.zermelo_access_token_expires_at > DateTime.Now;
            ViewData["isInfowijsGekoppeld"] = TokenUtils.CheckToken(user.infowijs_access_token);
            ViewData["isSomtodayGekoppeld"] = TokenUtils.CheckToken(user.somtoday_refresh_token);
            ViewData["prefersSomtodayRooster"] = user.prefered_rooster_engine == "somtoday";
            ViewData["forceShow"] = false;
            
            #if RELEASE && BETA
            ViewData["releaseType"] = "BETA";
            #elif RELEASE && !BETA
            ViewData["releaseType"] = "RELEASE";
            #else
            ViewData["releaseType"] = "DEV";
            #endif

            return View();
        }

        [Route("/data/sidebar")]
        public IActionResult Sidebar(bool forceShow = false)
        {
            var user = ZermosUser;
            ViewData["isZermeloGekoppeld"] = user.zermelo_access_token_expires_at > DateTime.Now;
            ViewData["isInfowijsGekoppeld"] = TokenUtils.CheckToken(user.infowijs_access_token);
            ViewData["isSomtodayGekoppeld"] = TokenUtils.CheckToken(user.somtoday_refresh_token);
            ViewData["prefersSomtodayRooster"] = user.prefered_rooster_engine == "somtoday";
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
            
            //set no-cache headers and referer policy to any source
            Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            Response.Headers.Add("Pragma", "no-cache");
            Response.Headers.Add("Expires", "0");
            Response.Headers.Add("Referrer-Policy", "no-referrer");
            
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

        [Route("/.well-known/assetlinks.json")]
        public IActionResult AssetLinks()
        {
            return Json(new object(), new JsonSerializerOptions {WriteIndented = true});
        }
        
        //any request that requests a png (/x.png) will be redirected to /images/x.png
        [Route("/{url}.png")]
        public IActionResult Png(string url)
        {
            //return the file from the images folder
            try
            {
                return File(System.IO.File.ReadAllBytes($"wwwroot/images/{url}.png"), "image/png");
            }
            catch (Exception e)
            {
                return NotFound();
            }
        }
        
        //any request that requests a png (/x.png) will be redirected to /images/x.png
        [Route("/installeren/{app}")]
        public IActionResult install(string app)
        {
            Log(LogLevel.Information, $"User: {ZermosEmail} initiated install of {app}");
            
            //app can be equel to Zermos.exe and Zermos.apk
            //files are in wwwroot/installs/[app]
            try
            {
                return File(System.IO.File.ReadAllBytes($"wwwroot/installs/{app}"), "application/octet-stream");
            }
            catch (Exception e)
            {
                return NotFound();
            }
        }
        
        //basic do whatever you want robots.txt
        [Route("/robots.txt")]
        public IActionResult Robots()
        {
            return Content("User-agent: *\nDisallow: \nDisallow: /js/\nDisallow: /css/\nDisallow: /Fonts/");
        }
        
        [Route("/manifest.webmanifest")]
        public IActionResult Webmanifest()
        {
            //return the file from the images folder
            try
            {
                return File(System.IO.File.ReadAllBytes($"wwwroot/manifest.json"), "application/manifest+json");
            }
            catch (Exception e)
            {
                return NotFound();
            }
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
            DateTime timeRequestRecieved = DateTime.UtcNow;
            DateTime timeRequestSend = Request.Form.ContainsKey("timesend") ? new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(long.Parse(Request.Form["timesend"])) : DateTime.MinValue;
            
            var stopwatch = Stopwatch.StartNew();
            var user = ZermosUser;
            stopwatch.Stop();
            
            Log(LogLevel.Information ,$"User: {user.email} initiated api test\n" +
                                          $"Request send at: {timeRequestSend:HH:mm:ss.fff}\n" +
                                          $"Request recieved at: {timeRequestRecieved:HH:mm:ss.fff}\n" +
                                          $"Response send at: {DateTime.UtcNow:HH:mm:ss.fff}\n" +
                                          $"DataBase query took: {stopwatch.ElapsedMilliseconds}ms");
            
            return Json(new
            {
                timeRequestRecieved,
                timeRequestSend,
                timeResponseSend = DateTime.UtcNow,
                timeQuery = stopwatch.ElapsedMilliseconds
            }, new JsonSerializerOptions {WriteIndented = true});
        }
        
        [ZermosPage]
        [Route("/gedeeld/{url}")]
        public async Task<IActionResult> Gedeeld(string url = "")
        {
            var share = await GetShare(url);
            
            if (share == null)
                return RedirectToAction("Verlopen", "Error");
            
            if (share.expires_at < DateTime.Now)
            {
                await DeleteShare(url);
                return RedirectToAction("Verlopen", "Error");
            }
            
            if (share.page == "/Zermelo/Rooster/Gedeeld")
                return PartialView("~/Views/Zermelo/GedeeldRooster.cshtml", share.value.Base64StringToObject<ZermeloRoosterModel>());
            if (share.page == "/Somtoday/Rooster/Gedeeld")
                return PartialView("~/Views/Zermelo/GedeeldRooster.cshtml", share.value.Base64StringToObject<ZermeloRoosterModel>());
            if (share.page == "/Somtoday/Cijfers/Gedeeld")
                return PartialView("~/Views/Somtoday/GedeeldCijfers.cshtml", share.value.Base64StringToObject<SortedSomtodayGradesModel>());

            return RedirectToAction("Verlopen", "Error");
        }
        
        [HttpGet]
        [Authorize]
        [Route("/test/ip")]
        public async Task<IActionResult> TestIp()
        {
            if (ZermosEmail != "58373@ccg-leerling.nl")
                return NotFound();
            
            var response = await _httpClient.GetAsync("https://api6.ipify.org");
            var responseString = await response.Content.ReadAsStringAsync();
            return Ok(responseString);
        }
        
        
    }
}