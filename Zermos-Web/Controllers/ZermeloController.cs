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
using Zermos_Web.APIs;
using Zermos_Web.Models.Requirements;
using Zermos_Web.Models.zermelo;
using Zermos_Web.Utilities;

namespace Zermos_Web.Controllers
{
    public class ZermeloController : BaseController
    {
        public ZermeloController(Users user, Shares share, ILogger<BaseController> logger) : base(user, share, logger) { }
        ZermeloApi zermeloApi = new(new HttpClient());

        [Authorize]
        [ZermosPage]
        [ZermeloRequirement]
        public async Task<IActionResult> Rooster(string year, string week, bool compact = false)
        {
            year ??= DateTime.Now.Year.ToString();
            week ??= DateTime.Now.GetWeekNumber().ToString();
            week = week.ToCharArray().Length == 1 ? "0" + week : week;
            
            //if request contains cookie old_zermelo_schedule, redirect to /smartwatch
            if (Request.Cookies["preview"]!.Contains("zermelo_smartwatch"))
                return RedirectToAction("SmartwatchRooster");

            return compact ? PartialView("Rooster-week", await zermeloApi.GetRoosterAsync(ZermosUser, year, week)) : PartialView(await zermeloApi.GetRoosterAsync(ZermosUser, year, week));
        }
        
        [Authorize]
        [ZermosPage]
        [Route("/Zermelo/Smartwatch")]
        [ZermeloRequirement]
        public async Task<IActionResult> SmartwatchRooster()
        {
            var week = DateTime.Now.GetWeekNumber().ToString();
            week = week.ToCharArray().Length == 1 ? "0" + week : week;
            var year = DateTime.Now.Year.ToString();
            
            return PartialView(await zermeloApi.GetRoosterAsync(ZermosUser, year, week));
        }

        [Authorize]
        [ZermeloRequirement]
        public async Task<IActionResult> GenereerToken(string year, string week, DateTime? expires_at, int max_uses = int.MaxValue)
        {
            if (string.IsNullOrEmpty(year) || string.IsNullOrEmpty(week))
                return BadRequest("Year or week is null or empty");
            
            expires_at ??= DateTime.Now.AddDays(7);
            week = week.ToCharArray().Length == 1 ? "0" + week : week;
            
            var zermeloRoosterModel = await zermeloApi.GetRoosterAsync(ZermosUser, year, week);
            
            string url = (await AddShare(new share
            {
                key = TokenUtils.RandomString(20),
                email = ZermosEmail,
                value = zermeloRoosterModel.ObjectToBase64String(),
                page = "/Zermelo/Rooster/Gedeeld",
                expires_at = (DateTime) expires_at,
                max_uses = max_uses
            })).url;
            
            return Ok(url);
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