using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Zermos_Web.Models;
using Zermos_Web.Models.Requirements;

namespace Zermos_Web.Controllers
{
    public class InfowijsController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<InfowijsController> _logger;
        private readonly Users _users;
        private readonly HttpClient _httpClient;

        public InfowijsController(ILogger<InfowijsController> logger, IConfiguration config, Users users)
        {
            _logger = logger;
            _config = config;
            _users = users;
            _httpClient = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    {"accept", "application/vnd.infowijs.v1+json"},
                    {"x-infowijs-client", "nl.infowijs.hoy.android/nl.infowijs.client.antonius"}
                }
            };
        }

        [Authorize]
        [InfowijsRequirement]
        [AddLoadingScreen("De laatste nieuwtjes worden geladen")]
        public IActionResult SchoolNieuws()
        {
            ViewData["add_css"] = "infowijs";

            //GET https://antonius.hoyapp.nl/hoy/v3/messages?include_archived=0&since=4000000
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", GetSessionToken().Result);
            var response = _httpClient
                .GetAsync("https://antonius.hoyapp.nl/hoy/v3/messages?include_archived=0&since=3500000").Result;

            /*
                type catalog:
                1: means message contents
                2: means that that is an attached file (bijlage)     
                3: means that it contains an foto
                
                12: probably means nothing, but is a divider between messages
                
                30: contains information about sender/reader and the title of the post 
            */

            //remove all messages that have type 12, then reverse the list so that the newest messages are on top, then group all the messages by groupid
            var infowijsMessage = JsonConvert
                .DeserializeObject<InfowijsMessagesModel>(response.Content.ReadAsStringAsync().Result,
                    Converter.Settings).Data.Messages
                .Where(x => x.Type != 12).Reverse().GroupBy(x => x.GroupId).ToList();

            return View(infowijsMessage);
        }


        [Authorize]
        [InfowijsRequirement]
        [AddLoadingScreen("De kalender wordt geladen")]
        public async Task<IActionResult> SchoolKalender()
        {
            ViewData["add_css"] = "infowijs";
            
            if (System.IO.File.Exists("infowijs_kalender.json"))
            {
                var lastModified = System.IO.File.GetLastWriteTime("infowijs_kalender.json");
                // if last modified was in 0:00 - 11:59, and it is 13.00, then update the file.
                // if last modified was in 12:00 - 23:59, and it is 00.00, then update the file.
                if ((lastModified.Hour < 12 && DateTime.Now.Hour >= 13) || (lastModified.Hour >= 12 && DateTime.Now.Hour <= 24))
                {
                    return View(JsonConvert
                        .DeserializeObject<InfowijsEventsModel>(await System.IO.File.ReadAllTextAsync("infowijs_kalender.json"),
                            Converter.Settings).data);
                }
            }

            //https://antonius.hoyapp.nl/hoy/v1/events
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", await GetSessionToken());

            var response = await _httpClient.GetAsync("https://antonius.hoyapp.nl/hoy/v1/events");
            
            //cache the output for 1 day in a json file
            await System.IO.File.WriteAllTextAsync("infowijs_kalender.json", await response.Content.ReadAsStringAsync());
            
            return View(JsonConvert
                .DeserializeObject<InfowijsEventsModel>(await response.Content.ReadAsStringAsync(),
                    Converter.Settings).data);
        }

        [NonAction]
        private async Task<string> GetSessionToken()
        {
            string mainAccessToken =
                (await _users.GetUserAsync(User.FindFirstValue("email"))).infowijs_access_token;

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.infowijs.nl/sessions/access_token");
            request.Headers.Add("Authorization", "Bearer " + mainAccessToken);
            request.Headers.Add("Accept", "application/vnd.infowijs.v1+json");
            request.Headers.Add("x-infowijs-client", $"nl.infowijs.hoy.android/nl.infowijs.client.antonius");
            var response = await _httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
            var accessToken = JsonConvert.DeserializeObject<InfowijsAccessTokenModel>(responseString);
            return accessToken.data;
        }

        public IActionResult SchoolWiki(string query)
        {
            //curl --location 'https://aboarc8x9f-dsn.algolia.net/1/indexes/*/queries?x-algolia-application-id=ABOARC8X9F&x-algolia-api-key=1c110b29cea05e83dce945e2c5594f2f' --header 'Content-Type: text/plain' --data '{"requests":[{"indexName":"schoolwiki.113-prod.185f99fe-1aea-4110-9d14-6c76533a352c","params":"query=PTA&hitsPerPage=100"}]}'
            
            string body =
                "{\"requests\":[{\"indexName\":\"schoolwiki.113-prod.185f99fe-1aea-4110-9d14-6c76533a352c\",\"params\":\"query=" +
                query +
                "&hitsPerPage=100\"}]}";
            var response = _httpClient
                .PostAsync(
                    "https://aboarc8x9f-dsn.algolia.net/1/indexes/*/queries?x-algolia-application-id=ABOARC8X9F&x-algolia-api-key=1c110b29cea05e83dce945e2c5594f2f",
                    new StringContent(body, Encoding.UTF8, "application/json")).Result;
            
            var schoolWikiModel = JsonConvert.DeserializeObject<SchoolWikiModel>(response.Content.ReadAsStringAsync().Result);
            
            return Ok(schoolWikiModel);
        }
    }
}