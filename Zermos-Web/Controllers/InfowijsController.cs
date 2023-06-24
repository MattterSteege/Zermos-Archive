using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Entities;
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
            var infowijsMessage = JsonConvert.DeserializeObject<InfowijsMessagesModel>(response.Content.ReadAsStringAsync().Result, Converter.Settings).Data.Messages
                .Where(x => x.Type != 12).Reverse().GroupBy(x => x.GroupId).ToList();

            return View(infowijsMessage);
        }

        [Authorize]
        [InfowijsRequirement]
        public async Task<IActionResult> SchoolKalender()
        {
            ViewData["add_css"] = "infowijs";
            
            //https://antonius.hoyapp.nl/hoy/v1/events
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", await GetSessionToken());
            
            var response = await _httpClient.GetAsync("https://antonius.hoyapp.nl/hoy/v1/events");
            return View(JsonConvert.DeserializeObject<InfowijsEventsModel>(await response.Content.ReadAsStringAsync(), Converter.Settings).data);
        }

        [NonAction]
        private async Task<string> GetSessionToken()
        {
            string mainAccessToken = (await _users.GetUserAsync(User.FindFirstValue("email"))).infowijs_access_token;

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.infowijs.nl/sessions/access_token");
            request.Headers.Add("Authorization", "Bearer " + mainAccessToken);
            request.Headers.Add("Accept", "application/vnd.infowijs.v1+json");
            request.Headers.Add("x-infowijs-client", $"nl.infowijs.hoy.android/nl.infowijs.client.antonius");
            var response = await _httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
            var accessToken = JsonConvert.DeserializeObject<InfowijsAccessTokenModel>(responseString);
            return accessToken.data;
        }
    }
}