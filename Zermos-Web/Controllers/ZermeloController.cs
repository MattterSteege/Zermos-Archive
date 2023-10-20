using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Zermos_Web.Models.Requirements;
using Zermos_Web.Models.zermelo;
using Zermos_Web.Utilities;

namespace Zermos_Web.Controllers
{
    public class ZermeloController : BaseController
    {
        public ZermeloController(Users user, ILogger<BaseController> logger) : base(user, logger) { }

        [Authorize]
        [ZermeloRequirement]
        [ZermosPage]
        public async Task<IActionResult> Rooster(string year, string week)
        {
            ViewData["add_css"] = "zermelo";

            year ??= DateTime.Now.Year.ToString();
            week ??= DateTime.Now.GetWeekNumber().ToString();

            var date = year + (week.ToCharArray().Length == 1 ? "0" + week : week);

            var user = ZermosUser;

            var baseURL = "https://ccg.zportal.nl/api/v3/liveschedule" +
                          $"?access_token={user.zermelo_access_token}" +
                          $"&student={user.school_id}" +
                          $"&week={date}";

            //GET request
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(baseURL);
            
            if (response.IsSuccessStatusCode == false)
            {   
                if (week == DateTime.Now.GetWeekNumber().ToString() && year == DateTime.Now.Year.ToString())
                {
                    ZermosUser = new user {cached_zermelo_schedule = "{\"response\":{\"data\":[{\"Items\":[{\"appointments\":[]}]}]}}"};
                }
                
                HttpContext.AddNotification("Oops, er is iets fout gegaan", "Je rooster kon niet worden geladen, waarschijnlijk is je Zermelo token verlopen", NotificationCenter.NotificationType.ERROR);
                return PartialView(new ZermeloRoosterModel{ response = new Response { data = new List<Items> { new() { appointments = new List<Appointment>(), MondayOfAppointmentsWeek = DateTimeUtils.GetMondayOfWeekAndYear(week, year)}}}});
            }

            if (week == DateTime.Now.GetWeekNumber().ToString() && year == DateTime.Now.Year.ToString())
            {
                ZermosUser = new user {cached_zermelo_schedule = await response.Content.ReadAsStringAsync()};
            }
            
            var zermeloRoosterModel = JsonConvert.DeserializeObject<ZermeloRoosterModel>(await response.Content.ReadAsStringAsync());
            zermeloRoosterModel.response.data[0].MondayOfAppointmentsWeek = DateTimeUtils.GetMondayOfWeekAndYear(week, year);

            return PartialView(zermeloRoosterModel);
        }
    }
}