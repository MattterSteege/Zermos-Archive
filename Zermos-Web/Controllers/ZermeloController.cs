using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Zermos_Web.Models;
using Zermos_Web.Models.Requirements;
using Zermos_Web.Utilities;

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

        [Authorize]
        [ZermeloRequirement]
        public async Task<IActionResult> Rooster(string year, string week)
        {
            ViewData["add_css"] = "zermelo";
            //the request was by ajax, so return the partial view

            year ??= DateTime.Now.Year.ToString();
            week ??= DateTime.Now.GetWeekNumber().ToString();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var date = year + (week.ToCharArray().Length == 1 ? "0" + week : week);

                var user = await _users.GetUserAsync(User.FindFirstValue("email"));

                var baseURL = "https://ccg.zportal.nl/api/v3/liveschedule" +
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
            ViewData["url"] = "/" + ControllerContext.RouteData.Values["controller"] + "/" +
                              ControllerContext.RouteData.Values["action"] + "?week=" + week;
            return View("_Loading");
        }
    }
}