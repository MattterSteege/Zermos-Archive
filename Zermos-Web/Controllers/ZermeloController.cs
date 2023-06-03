using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Zermos_Web.Models;

namespace Zermos_Web.Controllers
{
    public class ZermeloController : Controller
    {
        private readonly ILogger<ZermeloController> _logger;
        private readonly Users _users;


        public ZermeloController(ILogger<ZermeloController> logger, Users users)
        {
            _logger = logger;
            _users = users;
        }

        public async Task<IActionResult> Rooster(string year, string week)
        {
            ViewData["add_css"] = "zermelo";
            //the request was by ajax, so return the partial view
            
            year ??= DateTime.Now.Year.ToString();
            week ??= DateTime.Now.GetWeekNumber().ToString();
            
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var date = year + (week.ToCharArray().Length == 1 ? "0" + week : week);

                user user = await _users.GetUserAsync("8f3e7598-615f-4b43-9705-ba301c6e2fcd");

                string baseURL = $"https://ccg.zportal.nl/api/v3/liveschedule" +
                                 $"?access_token={user.zermelo_access_token}" +
                                 $"&student={user.school_id}" +
                                 $"&week={date}";

                //GET request
                using var httpClient = new HttpClient();
                var response = await httpClient.GetStringAsync(baseURL);

                return View(JsonConvert.DeserializeObject<ZermeloRoosterModel>(response));
            }

            //the request was by a legitimate user, so return the loading view
            ViewData["laad_tekst"] = "Je rooster wordt geladen";
            ViewData["url"] = "/" + ControllerContext.RouteData.Values["controller"] + "/" + ControllerContext.RouteData.Values["action"] + "?week=" + week;
            return View("_Loading");
        }

        [HttpGet]
        public IActionResult Inloggen()
        {
            ViewData["add_css"] = "zermelo";

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Inloggen(string username, string password)
        {
            Dictionary<string, string> form = new Dictionary<string, string>();
            form.Add("username", username);
            form.Add("password", password);
            form.Add("client_id", "OAuthPage");
            form.Add("redirect_uri", "/main/");
            form.Add("scope", "");
            form.Add("state", RandomStateString());
            form.Add("response_type", "code");
            form.Add("tenant", "ccg");

            string baseURL = $"https://ccg.zportal.nl/api/v3/oauth";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Zermos-Web");
            var response = await httpClient.PostAsync(baseURL, new FormUrlEncodedContent(form));
            var responseString = await response.Content.ReadAsStringAsync();

            string accessToken = Regex.Matches(responseString, "[a-zA-Z0-9]{20}")[0].Value;

            form = new Dictionary<string, string>();
            form.Add("code", accessToken);
            form.Add("client_id", "ZermeloPortal");
            form.Add("client_secret", "42");
            form.Add("grant_type", "authorization_code");
            form.Add("rememberMe", "true");

            baseURL = $"https://ccg.zportal.nl/api/v3/oauth/token";

            response = await httpClient.PostAsync(baseURL, new FormUrlEncodedContent(form));
            responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Inloggen", "Zermelo");
            }

            var zermeloAuthentication = JsonConvert.DeserializeObject<ZermeloAuthenticatieModel>(responseString);

            user user = new user {zermelo_access_token = zermeloAuthentication.access_token};
            await _users.UpdateUserAsync("8f3e7598-615f-4b43-9705-ba301c6e2fcd", user);

            return RedirectToAction("Rooster", "Zermelo");
        }

        Random random = new Random();

        private string RandomStateString(int length = 6)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string result = "";
            for (int i = 0; i < length; i++)
                result += chars[random.Next(0, chars.Length)];


            return result;
        }
    }
}