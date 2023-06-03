using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly HttpClient _httpClient;

        public SomtodayController(ILogger<SomtodayController> logger, Users users)
        {
            _logger = logger;
            _users = users;
            _httpClient = new HttpClient();
        }

        public async Task<IActionResult> Cijfers()
        {
            ViewData["add_css"] = "somtoday";
            //the request was by ajax, so return the partial view
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                user user = await _users.GetUserAsync("8f3e7598-615f-4b43-9705-ba301c6e2fcd");

                string baseUrl =
                    $"https://api.somtoday.nl/rest/v1/resultaten/huidigVoorLeerling/{user.somtoday_student_id}?begintNaOfOp={DateTime.Now:yyyy}-01-01";

                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + user.somtoday_access_token);
                _httpClient.DefaultRequestHeaders.Add("Range", "items=0-99");
                _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                var response = await _httpClient.GetAsync(baseUrl);

                var grades =
                    JsonConvert.DeserializeObject<SomtodayGradesModel>(await response.Content.ReadAsStringAsync());

                if (response.IsSuccessStatusCode == false)
                {
                    return NotFound("Er is iets fout gegaan bij het ophalen van de cijfers, het is mogelijk dat je SOMtoday token verlopen is.");
                }
                
                if (int.TryParse(response.Content.Headers.GetValues("Content-Range").First().Split('/')[1],
                        out int total))
                {
                    int requests = (total / 100) * 100;

                    for (int i = 100; i < requests; i += 100)
                    {
                        _httpClient.DefaultRequestHeaders.Clear();
                        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + user.somtoday_access_token);
                        _httpClient.DefaultRequestHeaders.Add("Range", $"items={i}-{i + 99}");
                        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                        response = await _httpClient.GetAsync(baseUrl);
                        var _grades =
                            JsonConvert.DeserializeObject<SomtodayGradesModel>(
                                await response.Content.ReadAsStringAsync());
                        grades.items.AddRange(_grades.items);
                    }
                }

                return PartialView(Sort(grades));
            }

            //the request was by a legitimate user, so return the loading view
            ViewData["url"] = "/" + ControllerContext.RouteData.Values["controller"] + "/" +
                              ControllerContext.RouteData.Values["action"];
            return View("_Loading");
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