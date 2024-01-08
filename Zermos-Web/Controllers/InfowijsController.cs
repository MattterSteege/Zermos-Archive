using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Zermos_Web.APIs;
using Zermos_Web.Models;
using Zermos_Web.Models.Requirements;

namespace Zermos_Web.Controllers
{
    public class InfowijsController : BaseController
    {
        public InfowijsController(Users user, Shares share, ILogger<BaseController> logger, GlobalVariables globalVariables) : base(user, share, logger, globalVariables) { }
        
        InfowijsApi infowijsApi = new(new HttpClient());
        
        private readonly HttpClient _httpClient = new()
        {
            DefaultRequestHeaders =
            {
                {"accept", "application/vnd.infowijs.v1+json"},
                {"x-infowijs-client", "nl.infowijs.hoy.android/nl.infowijs.client.antonius"}
            }
        };
        

        [HttpGet]
        [Authorize]
        [ZermosPage]
        [InfowijsRequirement]
        public async Task<IActionResult> Schoolnieuws()
        {
            
            if (Request.Cookies.ContainsKey("cached-infowijs-news"))
            {
                var messagesModel = JsonConvert.DeserializeObject<InfowijsMessagesModel>(ZermosUser.cached_infowijs_news ?? "{\"data\":{\"messages\":[],\"since\":0,\"hasMore\":false}}");
                
                if (messagesModel.Data.Messages.Count > 0)
                {
                    var infowijsMessageX = messagesModel.Data.Messages
                        .Where(x => x.Type != 12)
                        //.Reverse()
                        .GroupBy(x => x.GroupId)
                        .ToList();
                    
                    return PartialView(infowijsMessageX);
                }
            }
            
            InfowijsMessagesModel infowijsMessagesModel = await infowijsApi.GetSchoolNieuwsAsync(ZermosUser);
            
            
            //remove all messages that have type 12, then reverse the list so that the newest messages are on top, then group all the messages by groupid
            var infowijsMessage = infowijsMessagesModel.Data.Messages
                                                 .GroupBy(x => x.GroupId)
                                                 .ToList();

            ZermosUser = new user
            {
                cached_infowijs_news = JsonConvert.SerializeObject(infowijsMessagesModel)
            };

            Response.Cookies.Append("cached-infowijs-news", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), new CookieOptions {Expires = DateTime.Now.AddMinutes(10)});

            return PartialView(infowijsMessage);
        }


        [Authorize]
        [InfowijsRequirement]
        [ZermosPage]
        public async Task<IActionResult> Schoolkalender()
        {
            if (GlobalVariables.SchoolJaarKalenderLastMod.AddDays(1) > DateTime.Now && GlobalVariables.SchoolJaarKalender != null)
                return PartialView(GlobalVariables.SchoolJaarKalender.data);
            
            InfowijsEventsModel infowijsEventsModel = await infowijsApi.GetSchoolKalenderAsync(ZermosUser);

            GlobalVariables.SchoolJaarKalenderLastMod = DateTime.Now;
            GlobalVariables.SchoolJaarKalender = infowijsEventsModel;

            return PartialView(infowijsEventsModel.data);
        }
    }
}