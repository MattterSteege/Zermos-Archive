using System;
using System.Diagnostics;
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
            _httpClient.DefaultRequestHeaders.Add("x-infowijs-client", "nl.infowijs.hoy.android");
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
#if DEBUG
            var watch = Stopwatch.StartNew();
#endif
            //y=0.0508x−81148563 (approxSince = 0.0508 * DateTime.Now.ToUnixTimeSeconds() - 81148563)
            var approxSince = (int) (0.0489 * DateTime.Now.ToUnixTime() - 78164184) - 750000;
            
            var json = TokenUtils.DecodeJwt(user.infowijs_access_token);
            var client = json.payload.customerProduct.name;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetSessionTokenAsync(user));
            var response = await _httpClient.GetAsync($"https://{client}.hoyapp.nl/hoy/v3/messages?include_archived=1&since={approxSince}");
            
#if DEBUG
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds + "ms");
#endif
            
            return JsonConvert.DeserializeObject<InfowijsMessagesModel>(await response.Content.ReadAsStringAsync(), Converter.Settings);
            
            // var json = TokenUtils.DecodeJwt(user.infowijs_access_token);
            // var client = json.payload.customerProduct.name;
            // _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetSessionTokenAsync(user));
            // var response = await _httpClient.GetAsync($"https://{client}.hoyapp.nl/hoy/v3/messages?include_archived=1&since=0");
            //
            // InfowijsMessagesModel model = JsonConvert.DeserializeObject<InfowijsMessagesModel>(await response.Content.ReadAsStringAsync(), Converter.Settings);
            //
            // long since = model.Data.Since;
            //
            // bool shouldContinue = true;
            //
            // while (shouldContinue)
            // {
            //     //https://{client}.hoyapp.nl/hoy/v3/messages?include_archived=1&{since}
            //     response = await _httpClient.GetAsync($"https://{client}.hoyapp.nl/hoy/v3/messages?include_archived=1&since={since}");
            //     InfowijsMessagesModel newModel = JsonConvert.DeserializeObject<InfowijsMessagesModel>(await response.Content.ReadAsStringAsync(), Converter.Settings);
            //     model.Data.Messages.AddRange(newModel.Data.Messages);
            //     since = newModel.Data.Since;
            //     shouldContinue = newModel.Data.HasMore;
            // }
            
            //return model;
            
            //return JsonConvert.DeserializeObject<InfowijsMessagesModel>(await response.Content.ReadAsStringAsync(), Converter.Settings);
            
            /*
                type catalog:
                1: means message contents
                2: means that that is an attached file (bijlage)
                3: means that it contains an foto

                12: probably means nothing, but is a divider between messages

                30: contains information about sender/reader and the title of the post
            */
        }
        
        public async Task<InfowijsCustomerProductsModel> GetCustomerProductsAsync(string email)
        {
            //curl 'https://api.infowijs.nl/sessions/customer-products' --data-raw '{"username":"58373@ccg-leerling.nl"}' POST
            var body = "{\"username\":\"" + email + "\"}";
            var response = await _httpClient.PostAsync("https://api.infowijs.nl/sessions/customer-products", new StringContent(body, System.Text.Encoding.UTF8, "application/json"));
            return JsonConvert.DeserializeObject<InfowijsCustomerProductsModel>(await response.Content.ReadAsStringAsync(), Converter.Settings);
        }
        
        public async Task<InfowijsEventsModel> GetSchoolKalenderAsync(user user)
        {
            var json = TokenUtils.DecodeJwt(user.infowijs_access_token);
            var client = json.payload.customerProduct.name;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetSessionTokenAsync(user));
            var response = await _httpClient.GetAsync($"https://{client}.hoyapp.nl/hoy/v1/events");
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
