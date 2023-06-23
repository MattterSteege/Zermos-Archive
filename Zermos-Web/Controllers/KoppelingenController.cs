using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Zermos_Web.Models;
using Zermos_Web.Utilities;

namespace Zermos_Web.Controllers
{
    public class KoppelingenController : Controller
    {
        private readonly ILogger<KoppelingenController> _logger;
        private readonly HttpClient _infowijsHttpClient;
        private readonly HttpClient _zermeloHttpClient;
        private readonly HttpClient _somtodayHttpClient;
        private readonly HttpClient _somtodayHttpClientWithoutRedirect;
        private readonly Users _users;

        public KoppelingenController(ILogger<KoppelingenController> logger, Users users)
        {
            _logger = logger;
            _users = users;
            _infowijsHttpClient = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    {"accept", "application/vnd.infowijs.v1+json"},
                    {"x-infowijs-client", "nl.infowijs.hoy.android/nl.infowijs.client.antonius"}
                }
            };
            _zermeloHttpClient = new HttpClient()
            {
                DefaultRequestHeaders =
                {
                    {"User-Agent", "Zermos-Web"}
                }
            };
            _somtodayHttpClient = new HttpClient()
            {
                DefaultRequestHeaders =
                {
                    {"origin", "https://inloggen.somtoday.nl"},
                }
            };
            _somtodayHttpClientWithoutRedirect = new HttpClient(new HttpClientHandler {AllowAutoRedirect = false})
            {
                DefaultRequestHeaders =
                {
                    {"origin", "https://inloggen.somtoday.nl"}
                }
            };
        }

        #region infowijs

        [HttpGet]
        public IActionResult Infowijs()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Infowijs(string username, string id, string customer_product_id,
            string user_id)
        {
            if (username != null)
            {
                string url = "https://api.infowijs.nl/sessions";
                string json = JsonConvert.SerializeObject(new
                    {customerProductId = "77584871-d26b-11ea-8b2e-060ffde8896c", username});
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _infowijsHttpClient.PostAsync(url, data);
                var result = await response.Content.ReadAsStringAsync();

                return View(JsonConvert.DeserializeObject<AntoniusAppAuthenticatieModel>(result));
            }

            if (id != null && customer_product_id != null && user_id != null)
            {
                string url = $"https://api.infowijs.nl/sessions/{id}/{customer_product_id}/{user_id}";

                var response = await _infowijsHttpClient.PostAsync(url, null);
                var result = await response.Content.ReadAsStringAsync();

                if (result.StartsWith("{\"data\":\""))
                {
                    string token = result.Substring(9, result.Length - 11);

                    var user = new user {infowijs_access_token = token};
                    await _users.UpdateUserAsync(User.FindFirstValue("email"), user);

                    return RedirectToAction("Index", "Hoofdmenu");
                }

                return View(JsonConvert.DeserializeObject<AntoniusAppAuthenticatieModel>(result));
            }

            return RedirectToAction(nameof(Infowijs));
        }

        #endregion

        #region Zermelo

        [HttpGet]
        public IActionResult Zermelo()
        {
            ViewData["add_css"] = "zermelo";

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Zermelo(string username, string password)
        {
            var form = new Dictionary<string, string>();
            form.Add("username", username);
            form.Add("password", password);
            form.Add("client_id", "OAuthPage");
            form.Add("redirect_uri", "/main/");
            form.Add("scope", "");
            form.Add("state", TokenUtils.RandomString());
            form.Add("response_type", "code");
            form.Add("tenant", "ccg");

            var response = await _zermeloHttpClient.PostAsync("https://ccg.zportal.nl/api/v3/oauth",
                new FormUrlEncodedContent(form));
            string responseString = await response.Content.ReadAsStringAsync();

            var accessToken = Regex.Matches(responseString, "[a-zA-Z0-9]{20}")[0].Value;

            form = new Dictionary<string, string>();
            form.Add("code", accessToken);
            form.Add("client_id", "ZermeloPortal");
            form.Add("client_secret", "42");
            form.Add("grant_type", "authorization_code");
            form.Add("rememberMe", "true");

            response = await _zermeloHttpClient.PostAsync("https://ccg.zportal.nl/api/v3/oauth/token",
                new FormUrlEncodedContent(form));
            responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) return RedirectToAction(nameof(Zermelo));

            var zermeloAuthentication = JsonConvert.DeserializeObject<ZermeloAuthenticatieModel>(responseString);

            var user = new user {zermelo_access_token = zermeloAuthentication.access_token, school_id = username};
            await _users.UpdateUserAsync(User.FindFirstValue("email"), user);

            return RedirectToAction("Rooster", "Zermelo");
        }

        #endregion

        #region Somtoday
        [HttpGet]
        public IActionResult Somtoday()
        {
            ViewData["add_css"] = "somtoday";

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Somtoday(string username, string password)
        {
            //code challenge: __JVhs4cj-iqe8ha5750d9QSWJMpV49SXHPqBgFulkk
            //code verifier: 16BBJMtEJe8blIJY848ROvvO02F5V205l5A10x_DqFE

            var baseurl = string.Format(
                "https://inloggen.somtoday.nl/oauth2/authorize?redirect_uri=somtodayleerling://oauth/callback&client_id=D50E0C06-32D1-4B41-A137-A9A850C892C2&response_type=code&state={0}&scope=openid&tenant_uuid={1}&session=no_session&code_challenge={2}&code_challenge_method=S256",
                TokenUtils.RandomString(8), "c23fbb99-be4b-4c11-bbf5-57e7fc4f4388",
                "__JVhs4cj-iqe8ha5750d9QSWJMpV49SXHPqBgFulkk");

            var response = await _somtodayHttpClientWithoutRedirect.GetAsync(baseurl);
            var authCode = response.Headers.Location.Query.Remove(0, 6);
            


            baseurl = "https://inloggen.somtoday.nl/?-1.-panel-signInForm&auth=" + authCode;

            var Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"loginLink", "x"},
                {"usernameFieldPanel:usernameFieldPanel_body:usernameField", username}
            });

            await _somtodayHttpClientWithoutRedirect.PostAsync(baseurl, Content);

            baseurl = "https://inloggen.somtoday.nl/login?1-1.-passwordForm&auth=" + authCode;

            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"passwordFieldPanel:passwordFieldPanel_body:passwordField", password},
                {"loginLink", "x"}
            });

            response = await _somtodayHttpClientWithoutRedirect.PostAsync(baseurl, Content);

            var finalAuthCode = HTMLUtils.ParseQuery(response.Headers.Location.Query)["code"];


            baseurl = "https://inloggen.somtoday.nl/oauth2/token?grant_type=authorization_code&session=no_session&scope=openid&client_id=D50E0C06-32D1-4B41-A137-A9A850C892C2&tenant_uuid=c23fbb99-be4b-4c11-bbf5-57e7fc4f4388&code=" +
                      finalAuthCode + "&code_verifier=16BBJMtEJe8blIJY848ROvvO02F5V205l5A10x_DqFE";

            response = await _somtodayHttpClientWithoutRedirect.PostAsync(baseurl,
                new FormUrlEncodedContent(new Dictionary<string, string> {{"", ""}}));

            var somtodayAuthentication =
                JsonConvert.DeserializeObject<SomtodayAuthenticatieModel>(response.Content.ReadAsStringAsync()
                    .Result);

            if (somtodayAuthentication.access_token == null) return View();

            var user = await GetSomtodayStudent(somtodayAuthentication.access_token);
            user.somtoday_access_token = somtodayAuthentication.access_token;
            user.somtoday_refresh_token = somtodayAuthentication.refresh_token;

            await _users.UpdateUserAsync(User.FindFirstValue("email"), user);
            return RedirectToAction("Index", "Hoofdmenu");
        }

        private async Task<user> GetSomtodayStudent(string auth_token)
        {
            //GET: https://api.somtoday.nl/rest/v1/leerlingen?additional=pasfoto
            var baseurl = "https://api.somtoday.nl/rest/v1/leerlingen?additional=pasfoto";
            
            _somtodayHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth_token);

            var response = await _somtodayHttpClient.GetAsync(baseurl);
            var somtodayStudent =
                JsonConvert.DeserializeObject<SomtodayStudentModel>(await response.Content.ReadAsStringAsync());
            return new user
            {
                somtoday_student_id = somtodayStudent.items[0].links[0].id.ToString(),
                somtoday_student_profile_picture = somtodayStudent.items[0].additionalObjects.pasfoto.datauri
            };
        }

        #endregion
    }
}