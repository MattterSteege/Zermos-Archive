using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using ChartJSCore.Models;
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
    [Authorize]
    public class MicrosoftController : BaseController
    {

        public MicrosoftController(Users user, ILogger<BaseController> logger) : base(user, logger) { }
        
        
        private readonly HttpClient _microsoftHttpClientRefresh = new()
        {
            BaseAddress = new Uri("https://login.microsoftonline.com/organizations/oauth2/v2.0/token"),
            DefaultRequestHeaders =
            {
                {"origin", "https://developer.microsoft.com"},
                {"accept", "application/json"}
            }
        };
        
        private readonly HttpClient _microsoftHttpClient = new()
        {
            BaseAddress = new Uri("https://graph.microsoft.com"),
            DefaultRequestHeaders =
            {
                {"accept", "application/json"},
            }
        };

        [NonAction]
        private async Task<string> GetAccessToken()
        {
            string access_token = ZermosUser.teams_access_token;
            if (access_token != null && TokenUtils.CheckToken(access_token))
                return access_token;
            
            string refresh_token = ZermosUser.teams_refresh_token;
            if (refresh_token == null)
                return null;
            
            var collection = new List<KeyValuePair<string, string>>();
            collection.Add(new("client_id", "5e3ce6c0-2b1f-4285-8d4b-75ee78787346"));
            collection.Add(new("scope", "https://graph.microsoft.com//.default openid profile offline_access"));
            collection.Add(new("grant_type", "refresh_token"));
            collection.Add(new("refresh_token", refresh_token));
            var content = new FormUrlEncodedContent(collection);
            
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                Content = content,
            };
            
            var response = await _microsoftHttpClientRefresh.SendAsync(request);
            
            if (!response.IsSuccessStatusCode)
                return null;
            
            
            var microsoftAuthentication = JsonConvert.DeserializeObject<MicrosoftAuthenticationModel>(await response.Content.ReadAsStringAsync());

            ZermosUser = new user
            {
                teams_access_token = microsoftAuthentication.access_token,
                teams_refresh_token = microsoftAuthentication.refresh_token
            };
            
            return microsoftAuthentication.access_token;
        }

        [ZermosPage]
        [Route("/Microsoft/Onedrive")]
        public async Task<IActionResult> Onderive()
        {
            return PartialView();
        }

        [Route("/Microsoft/Onedrive/{folderId}")]
        public async Task<IActionResult> OnedriveFolder(string folderId = "root")
        {
            //if folderId == root
            //  https://graph.microsoft.com/v1.0/me/drive/root/children/?$select=name,webUrl,size,folder,id
            //else
            //  https://graph.microsoft.com/v1.0/me/drive/items/[FOLDER_ID]/children?$select=name,webUrl,size,folder,id
            
            var accessToken = await GetAccessToken();
            if (accessToken == null)
                return Unauthorized();
            
            _microsoftHttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            
            var response = await _microsoftHttpClient.GetAsync(folderId == "root" ? "/v1.0/me/drive/root/children/?$select=name,webUrl,size,folder,id,fileSystemInfo" : $"/v1.0/me/drive/items/{folderId}/children?$select=name,webUrl,size,folder,id,fileSystemInfo");

            if (!response.IsSuccessStatusCode)
                return Unauthorized();
            
            var result = await response.Content.ReadAsStringAsync();
            
            return PartialView(JsonConvert.DeserializeObject<MicrosoftOnedriveFileModel>(result));
        }
    }
}