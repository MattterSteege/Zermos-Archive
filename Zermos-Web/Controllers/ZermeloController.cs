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
        [AddLoadingScreen("Je rooster wordt geladen")]
        public async Task<IActionResult> Rooster(string year, string week, bool asPartial = false)
        {
            ViewData["add_css"] = "zermelo";

            year ??= DateTime.Now.Year.ToString();
            week ??= DateTime.Now.GetWeekNumber().ToString();

            var date = year + (week.ToCharArray().Length == 1 ? "0" + week : week);

            var user = await _users.GetUserAsync(User.FindFirstValue("email"));

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
                    await _users.UpdateUserAsync(User.FindFirstValue("email"), new user {cached_zermelo_schedule = "{\"response\":{\"data\":[{\"Items\":[{\"appointments\":[]}]}]}}"});
                }
                
                HttpContext.AddNotification("Oops, er is iets fout gegaan", "Je rooster kon niet worden geladen, waarschijnlijk is je Zermelo token verlopen", NotificationCenter.NotificationType.ERROR);
                if (asPartial)
                    return PartialView(new ZermeloRoosterModel{ response = new Response { data = new List<Items> { new() { appointments = new List<Appointment>(), MondayOfAppointmentsWeek = DateTimeUtils.GetMondayOfWeekAndYear(week, year)}}}});
                return PartialView(new ZermeloRoosterModel{ response = new Response { data = new List<Items> { new() { appointments = new List<Appointment>(), MondayOfAppointmentsWeek = DateTimeUtils.GetMondayOfWeekAndYear(week, year)}}}});
            }

            if (week == DateTime.Now.GetWeekNumber().ToString() && year == DateTime.Now.Year.ToString())
            {
                await _users.UpdateUserAsync(User.FindFirstValue("email"), new user {cached_zermelo_schedule = await response.Content.ReadAsStringAsync()});
            }
            
            var zermeloRoosterModel = JsonConvert.DeserializeObject<ZermeloRoosterModel>(await response.Content.ReadAsStringAsync());
            zermeloRoosterModel.response.data[0].MondayOfAppointmentsWeek = DateTimeUtils.GetMondayOfWeekAndYear(week, year);
            
            if (asPartial)
                return PartialView(zermeloRoosterModel);
            return PartialView(zermeloRoosterModel);
        }
    }
}