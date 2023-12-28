using System;
using System.Collections.Generic;
using System.Net.Http;
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
        public ZermeloController(Users user, Shares share, ILogger<BaseController> logger) : base(user, share, logger) { }

        [Authorize]
        [ZermosPage]
        [ZermeloRequirement]
        public async Task<IActionResult> Rooster(string year, string week, bool compact = false)
        {
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
                // if (week == DateTime.Now.GetWeekNumber().ToString() && year == DateTime.Now.Year.ToString())
                // {
                //     ZermosUser = new user {cached_zermelo_schedule = "{\"response\":{\"data\":[{\"Items\":[{\"appointments\":[]}]}]}}"};
                // }
                
                HttpContext.AddNotification("Oops, er is iets fout gegaan", "Je rooster kon niet worden geladen, waarschijnlijk is je Zermelo token verlopen", NotificationCenter.NotificationType.ERROR);
                return compact ? PartialView("Rooster-week", new ZermeloRoosterModel{ response = new Response { data = new List<Items> { new() { appointments = new List<Appointment>(), MondayOfAppointmentsWeek = DateTimeUtils.GetMondayOfWeekAndYear(week, year)}}}}) : PartialView(new ZermeloRoosterModel{ response = new Response { data = new List<Items> { new() { appointments = new List<Appointment>(), MondayOfAppointmentsWeek = DateTimeUtils.GetMondayOfWeekAndYear(week, year)}}}});
            }

            // if (week == DateTime.Now.GetWeekNumber().ToString() && year == DateTime.Now.Year.ToString())
            // {
            //     ZermosUser = new user {cached_zermelo_schedule = await response.Content.ReadAsStringAsync()};
            // }
            
            var zermeloRoosterModel = JsonConvert.DeserializeObject<ZermeloRoosterModel>(await response.Content.ReadAsStringAsync());
            zermeloRoosterModel.response.data[0].MondayOfAppointmentsWeek = DateTimeUtils.GetMondayOfWeekAndYear(week, year);

            return compact ? PartialView("Rooster-week", zermeloRoosterModel) : PartialView(zermeloRoosterModel);
        }

        [Authorize]
        [ZermeloRequirement]
        public async Task<IActionResult> GenereerToken(string year, string week, DateTime? expires_at, int max_uses = int.MaxValue)
        {
            if (string.IsNullOrEmpty(year) || string.IsNullOrEmpty(week))
            {
                return BadRequest("Year or week is null or empty");
            }
            
            expires_at ??= DateTime.Now.AddDays(7);

            var date = year + (week.ToCharArray().Length == 1 ? "0" + week : week);

            var user = ZermosUser;

            var baseURL = "https://ccg.zportal.nl/api/v3/liveschedule" +
                          $"?access_token={user.zermelo_access_token}" +
                          $"&student={user.school_id}" +
                          $"&week={date}";

            ZermeloRoosterModel zermeloRoosterModel;
            
            //GET request
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(baseURL);
            
            if (response.IsSuccessStatusCode == false)
            {   
                return BadRequest("Fetching Zermelo schedule failed");
            }
            
            zermeloRoosterModel = JsonConvert.DeserializeObject<ZermeloRoosterModel>(await response.Content.ReadAsStringAsync());
            zermeloRoosterModel.response.data[0].MondayOfAppointmentsWeek = DateTimeUtils.GetMondayOfWeekAndYear(week, year);
            
            var token = TokenUtils.RandomString(20,
                TokenUtils.RandomStringType.Numbers & TokenUtils.RandomStringType.LowerCase &
                TokenUtils.RandomStringType.UpperCase);
            
            share share = new share
            {
                key = token,
                email = ZermosEmail,
                value = zermeloRoosterModel.ObjectToBase64String(),
                page = "/Zermelo/Rooster/Gedeeld",
                expires_at = (DateTime) expires_at,
                max_uses = max_uses
            };
            
            await AddShare(share);
            
            return Ok(share.url);
        }

        [ZermosPage]
        [HttpGet("/Zermelo/Rooster/Gedeeld")]
        public async Task<IActionResult> GedeeldRooster(string token)
        {
            var rooster = await GetShare(token);
            
            if (rooster == null)
                return NotFound();
            
            if (rooster.expires_at < DateTime.Now)
            {
                await DeleteShare(token);
                return NotFound();
            }
            
            return PartialView(rooster.value.Base64StringToObject<ZermeloRoosterModel>());
        }
    }
}