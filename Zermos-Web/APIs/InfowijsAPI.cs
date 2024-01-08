using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Infrastructure.Entities;
using Newtonsoft.Json;
using Zermos_Web.Models;
using Zermos_Web.Utilities;

namespace Zermos_Web.APIs
{
    public class InfowijsApi
    {
        private readonly HttpClient _httpClient;

        public InfowijsApi(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.DefaultRequestHeaders.Add("accept", "application/vnd.infowijs.v1+json");
            _httpClient.DefaultRequestHeaders.Add("x-infowijs-client", "nl.infowijs.hoy.android/nl.infowijs.client.antonius");
        }
        
        public async Task<string> GetSessionTokenAsync(user user)
        {
            if (user.infowijs_access_token.IsNullOrEmpty()) throw new ArgumentNullException(nameof(user.infowijs_access_token));
            
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.infowijs.nl/sessions/access_token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.infowijs_access_token);
            var response = await _httpClient.SendAsync(request);
            return JsonConvert.DeserializeObject<InfowijsAccessTokenModel>(await response.Content.ReadAsStringAsync()).data;
        }
        
        public async Task<InfowijsMessagesModel> GetSchoolNieuwsAsync(user user)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetSessionTokenAsync(user));
            var response = await _httpClient.GetAsync("https://antonius.hoyapp.nl/hoy/v3/messages?include_archived=0&since=4500000");
            return JsonConvert.DeserializeObject<InfowijsMessagesModel>(await response.Content.ReadAsStringAsync(), Converter.Settings);
            
            /*
                type catalog:
                1: means message contents
                2: means that that is an attached file (bijlage)
                3: means that it contains an foto

                12: probably means nothing, but is a divider between messages

                30: contains information about sender/reader and the title of the post
            */
        }
        
        public async Task<InfowijsEventsModel> GetSchoolKalenderAsync(user user)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetSessionTokenAsync(user));
            var response = await _httpClient.GetAsync("https://antonius.hoyapp.nl/hoy/v1/events");
            return JsonConvert.DeserializeObject<InfowijsEventsModel>(await response.Content.ReadAsStringAsync(), Converter.Settings);
        }
        
        //Als je het nog gaat gebruiken dan kan je hier de endpoint voor de schoolwiki vinden.
        public async Task<InfowijsMessagesModel> GetSchoolWikiAsync(string query)
        {            
            var body = "{\"requests\":[{\"indexName\":\"schoolwiki.113-prod.185f99fe-1aea-4110-9d14-6c76533a352c\",\"params\":\"query=" + query + "&hitsPerPage=100\"}]}";
            var response = await _httpClient.PostAsync("https://aboarc8x9f-dsn.algolia.net/1/indexes/*/queries?x-algolia-application-id=ABOARC8X9F&x-algolia-api-key=1c110b29cea05e83dce945e2c5594f2f", new StringContent(body, System.Text.Encoding.UTF8, "application/json"));
            return JsonConvert.DeserializeObject<InfowijsMessagesModel>(await response.Content.ReadAsStringAsync(), Converter.Settings);
        }
    }
}
