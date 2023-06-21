using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Zermos_Web.Models;

namespace Zermos_Web.Controllers
{
    public class AntoniusAppController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<AntoniusAppController> _logger;
        private readonly Users _users;
        private readonly HttpClient _httpClient;

        public AntoniusAppController(ILogger<AntoniusAppController> logger, IConfiguration config, Users users)
        {
            _logger = logger;
            _config = config;
            _users = users;
            _httpClient = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    { "accept", "application/vnd.infowijs.v1+json" },
                    { "x-infowijs-client", "nl.infowijs.hoy.android/nl.infowijs.client.antonius" }
                }
            };
        }

        public IActionResult SchoolNieuws()
        {
            return Ok("SchoolNieuws");
        }

        public IActionResult SchoolKalender()
        {
            return Ok("SchoolKalender");
        }

        [HttpGet]
        public IActionResult Koppelen()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Koppelen(string username, string id, string customer_product_id, string user_id)
        {
            if (username != null)
            {
                string url = "https://api.infowijs.nl/sessions";
                string json = JsonConvert.SerializeObject(new { customerProductId = "77584871-d26b-11ea-8b2e-060ffde8896c", username });
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, data);
                var result = await response.Content.ReadAsStringAsync();

                return View(JsonConvert.DeserializeObject<AntoniusAppAuthenticatieModel>(result));
            }

            if (id != null && customer_product_id != null && user_id != null)
            {
                string url = $"https://api.infowijs.nl/sessions/{id}/{customer_product_id}/{user_id}";

                var response = await _httpClient.PostAsync(url, null);
                var result = await response.Content.ReadAsStringAsync();

                if (result.StartsWith("{\"data\":\""))
                {
                    string token = result.Substring(9, result.Length - 11);

                    var user = new user { infowijs_access_token = token };
                    await _users.UpdateUserAsync(User.FindFirstValue("email"), user);

                    return RedirectToAction(nameof(SchoolNieuws));
                }

                return View(JsonConvert.DeserializeObject<AntoniusAppAuthenticatieModel>(result));
            }

            return RedirectToAction(nameof(Koppelen));
        }
    }
}
