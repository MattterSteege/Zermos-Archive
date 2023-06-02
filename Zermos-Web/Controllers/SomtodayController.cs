using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Zermos_Web.Models;

namespace Zermos_Web.Controllers
{
    public class SomtodayController : Controller
    {
        private readonly ILogger<SomtodayController> _logger;
        private readonly Users _users;

        public SomtodayController(ILogger<SomtodayController> logger, Users users)
        {
            _logger = logger;
            _users = users;
        }

        public async Task<IActionResult> Cijfers()
        {
            ViewData["add_css"] = "somtoday";
            user user = await _users.GetUserAsync("8f3e7598-615f-4b43-9705-ba301c6e2fcd");

            string baseUrl = $"https://api.somtoday.nl/rest/v1/resultaten/huidigVoorLeerling/{user.somtoday_student_id}?begintNaOfOp={DateTime.Now:yyyy}-01-01";

            //GET request
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + user.somtoday_access_token);
            httpClient.DefaultRequestHeaders.Add("Range", "items=0-99");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            
            var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, baseUrl));
            
            var grades = JsonConvert.DeserializeObject<SomtodayGradesModel>(await response.Content.ReadAsStringAsync());
            
            int total = int.Parse(response.Content.Headers.GetValues("Content-Range").First().Split('/')[1]);

            int requests = (total / 100) * 100;

            for (int i = 100; i < requests; i += 100)
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + user.somtoday_access_token);
                httpClient.DefaultRequestHeaders.Add("Range", $"items={i}-{i + 99}");
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                string _response = await httpClient.GetStringAsync(baseUrl);
                
                var _grades = JsonConvert.DeserializeObject<SomtodayGradesModel>(_response);
                
                grades.items.AddRange(_grades.items);
            }
            
            return View(Sort(grades));
        }
        
        public SomtodayGradesModel Sort(SomtodayGradesModel grades)
        {
            grades.items.RemoveAll(x => x.geldendResultaat == null);
            grades.items.RemoveAll(x => string.IsNullOrEmpty(x.omschrijving) && x.weging == 0);
            grades.items = grades.items.OrderBy(x => x.datumInvoer).ToList();
            return grades;
        }
    }
}