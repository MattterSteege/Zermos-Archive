using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Zermos_Web.Models;
using Zermos_Web.Models.Requirements;
using Zermos_Web.Models.zermeloUserModel;
using Zermos_Web.Utilities;

namespace Zermos_Web.Controllers
{
    [Authorize]
    public class KoppelingenController : Controller
    {
        private readonly ILogger<KoppelingenController> _logger;
        private readonly HttpClient _infowijsHttpClient;
        private readonly HttpClient _zermeloHttpClient;
        private readonly HttpClient _somtodayHttpClient;
        private readonly HttpClient _somtodayHttpClientWithoutRedirect;
        private readonly HttpClient _httpClientWithoutRedirect;
        private readonly Users _users;
        private readonly Random _random;

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
                    {"accept", "application/json"}
                }
            };
            _somtodayHttpClientWithoutRedirect = new HttpClient(new HttpClientHandler {AllowAutoRedirect = false})
            {
                DefaultRequestHeaders =
                {
                    {"origin", "https://inloggen.somtoday.nl"},
                }
            };
            _httpClientWithoutRedirect = new HttpClient(new HttpClientHandler {AllowAutoRedirect = false});
            _random = new Random();
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["add_css"] = "koppelingen";
            return View();
        }

        #region ontkoppelen

        [HttpPost("ontkoppel/{app}")]
        public async Task<IActionResult> Ontkoppel(string app)
        {
            switch (app)
            {
                case "infowijs":
                    await _users.UpdateUserAsync(User.FindFirstValue("email"),
                        new user {infowijs_access_token = string.Empty});
                    return Redirect("/account");

                case "somtoday":
                    await _users.UpdateUserAsync(User.FindFirstValue("email"),
                        new user
                        {
                            somtoday_access_token = string.Empty, somtoday_refresh_token = string.Empty,
                            somtoday_student_id = string.Empty
                        });
                    return Redirect("/account");

                case "zermelo":
                    await _users.UpdateUserAsync(User.FindFirstValue("email"),
                        new user
                        {
                            zermelo_access_token = string.Empty, zermelo_access_token_expires_at = DateTime.MinValue
                        });
                    return Redirect("/account");

                default:
                    return Redirect("/account");
            }
        }

        #endregion

        #region infowijs
        [HttpGet]
        [Route("Koppelingen/Infowijs/Qr")]
        public async Task<IActionResult> InfowijsQr(string uuid, bool retry = false)
        {
            if (uuid != null)
            {
                ViewData["qr_text"] = "hoy_scan://v1/login/" + uuid;
                ViewData["uuid"] = uuid;
                ViewData["retry"] = retry;
                return View(model: "");
            }
            
            ViewData["add_css"] = "koppelingen";
            
            string url1 = "https://api.infowijs.nl/sessions/transfer";
            var response1 = await _infowijsHttpClient.PostAsync(url1, null);
            var result1 = await response1.Content.ReadAsStringAsync();

            uuid = JsonConvert.DeserializeObject<AntoniusAppAuthenticatieModelAuthSuccess>(result1).data;

            ViewData["qr_text"] = "hoy_scan://v1/login/" + uuid;
            ViewData["uuid"] = uuid;
            ViewData["retry"] = retry;

            return View(model: "");
        }

        [HttpPost]
        [Route("Koppelingen/Infowijs/Qr")]
        public async Task<IActionResult> InfowijsQr(string uuid)
        {
            if (uuid == null)
            {
                return Redirect("/koppelingen/infowijs/qr");
            }
            
            ViewData["add_css"] = "koppelingen";
            
            string url2 = "https://api.infowijs.nl/sessions/transfer/" + uuid;
            var response2 = await _infowijsHttpClient.GetAsync(url2);
            var result2 = await response2.Content.ReadAsStringAsync();

            if (response2.StatusCode != HttpStatusCode.OK)
            {
                ViewData["qr_text"] = "hoy_scan://v1/login/" + uuid;
                ViewData["uuid"] = uuid;
                ViewData["retry"] = true;
                return View(model: "");
            }
            
            var jwt = JsonConvert.DeserializeObject<AntoniusAppAuthenticatieModelAuthSuccess>(result2).data;

            var email = User.FindFirstValue("email");
            await _users.UpdateUserAsync(email, new user {infowijs_access_token = jwt});

            return RedirectToAction("ShowAccount", "Account");
        }

        #endregion

        #region Zermelo

        [HttpGet]
        public IActionResult Zermelo()
        {
            ViewData["add_css"] = "koppelingen";

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

            var zermeloUser = await GetZermeloUser(zermeloAuthentication.access_token);

            var user = new user
            {
                zermelo_access_token = zermeloAuthentication.access_token,
                school_id = zermeloUser.response.data[0].code,
                name = zermeloUser.response.data[0].firstName + " " + zermeloUser.response.data[0].prefix + " " +
                       zermeloUser.response.data[0].lastName,
                zermelo_access_token_expires_at = DateTime.Now.AddMonths(2)
            };

            await _users.UpdateUserAsync(User.FindFirstValue("email"), user);

            return RedirectToAction("ShowAccount", "Account");
        }

        [HttpGet]
        [Route("/Koppelingen/Zermelo/Qr")]
        public IActionResult ZermeloWithQr()
        {
            ViewData["add_css"] = "koppelingen";
            return View();
        }

        [HttpGet]
        [Route("/Koppelingen/Zermelo/Code")]
        public IActionResult ZermeloWithCode()
        {
            ViewData["add_css"] = "koppelingen";
            return View();
        }

        [HttpPost]
        [Route("/Koppelingen/Zermelo/Code")]
        public async Task<IActionResult> ZermeloWithCode(string code, string from = "code")
        {
            //POST /oauth/token?grant_type=authorization_code&code=
            string url =
                $"https://ccg.zportal.nl/api/v3/oauth/token?grant_type=authorization_code&code={code.Replace(" ", "")}";
            var response = await _zermeloHttpClient.PostAsync(url, null);
            string responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                HttpContext.AddNotification("Niet geldig",
                    "Deze code is mogelijk niet geldig, refresh zermelo en probeer het opnieuw",
                    NotificationCenter.NotificationType.ERROR);

                if (from == "code")
                    return RedirectToAction("ZermeloWithCode", "Koppelingen");

                return RedirectToAction("ZermeloWithQr", "Koppelingen");
            }

            var zermeloAuthentication = JsonConvert.DeserializeObject<ZermeloAuthenticatieModel>(responseString);

            var zermeloUser = await GetZermeloUser(zermeloAuthentication.access_token);

            var user = new user
            {
                zermelo_access_token = zermeloAuthentication.access_token,
                school_id = zermeloUser.response.data[0].code,
                name = zermeloUser.response.data[0].firstName + " " + zermeloUser.response.data[0].prefix + " " +
                       zermeloUser.response.data[0].lastName,
                zermelo_access_token_expires_at = DateTime.Now.AddMonths(2)
            };
            await _users.UpdateUserAsync(User.FindFirstValue("email"), user);

            return RedirectToAction("ShowAccount", "Account");
        }

        private async Task<ZermeloUserModel> GetZermeloUser(string access_token)
        {
            string url = "https://ccg.zportal.nl/api/v3/users/~me?access_token=" + access_token;
            var response = await _zermeloHttpClient.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ZermeloUserModel>(responseString);
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
            string[] tokens = GenerateTokens();
            //0 = code verifier
            //1 = code challenge

            //code challenge: __JVhs4cj-iqe8ha5750d9QSWJMpV49SXHPqBgFulkk
            //code verifier: 16BBJMtEJe8blIJY848ROvvO02F5V205l5A10x_DqFE

            var baseurl = string.Format(
                "https://inloggen.somtoday.nl/oauth2/authorize?redirect_uri=somtodayleerling://oauth/callback&client_id=D50E0C06-32D1-4B41-A137-A9A850C892C2&response_type=code&state={0}&scope=openid&tenant_uuid={1}&session=no_session&code_challenge={2}&code_challenge_method=S256",
                TokenUtils.RandomString(8), "c23fbb99-be4b-4c11-bbf5-57e7fc4f4388",
                tokens[1]);

            var response = await _httpClientWithoutRedirect.GetAsync(baseurl);
            var authCode = response.Headers.Location.Query.Remove(0, 6);


            //_somtodayHttpClientWithoutRedirect.DefaultRequestHeaders.Add("origin", "https://inloggen.somtoday.nl");


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


            baseurl =
                "https://inloggen.somtoday.nl/oauth2/token?grant_type=authorization_code&session=no_session&scope=openid&client_id=D50E0C06-32D1-4B41-A137-A9A850C892C2&tenant_uuid=c23fbb99-be4b-4c11-bbf5-57e7fc4f4388&code=" +
                finalAuthCode + "&code_verifier=" + tokens[0];

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
            return RedirectToAction("ShowAccount", "Account");
        }


        private async Task<user> GetSomtodayStudent(string auth_token)
        {
            //GET: https://api.somtoday.nl/rest/v1/leerlingen?additional=pasfoto
            var baseurl = "https://api.somtoday.nl/rest/v1/leerlingen";

            _somtodayHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", auth_token);

            var response = await _somtodayHttpClient.GetAsync(baseurl);
            string responseString = await response.Content.ReadAsStringAsync();
            var somtodayStudent =
                JsonConvert.DeserializeObject<SomtodayStudentModel>(responseString);
            return new user
            {
                somtoday_student_id = somtodayStudent.items[0].links[0].id.ToString(),
            };
        }

        public string[] GenerateTokens()
        {
            string[] tokens = new string[2];
            tokens[0] = GenerateNonce();
            tokens[1] = GenerateCodeChallenge(tokens[0]);
            return tokens;
        }

        private string GenerateNonce()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz123456789";
            var nonce = new char[128];
            for (int i = 0; i < nonce.Length; i++)
                nonce[i] = chars[_random.Next(0, chars.Length)];

            return new string(nonce);
        }

        private string GenerateCodeChallenge(string codeVerifier)
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
            var b64Hash = Convert.ToBase64String(hash);
            var code = Regex.Replace(b64Hash, "\\+", "-");
            code = Regex.Replace(code, "\\/", "_");
            code = Regex.Replace(code, "=+$", "");
            return code;
        }

        #endregion
        
        #region Teams
        [NotImplementedYet]
        public IActionResult Teams()
        {
#if DEBUG
            string redirectUrl = "https://localhost:5001/Koppelingen/Teams/Callback";
#elif RELEASE
            string redirectUrl = "https://zermos.kronk.tech/Koppelingen/Teams/Callback";
#endif
            redirectUrl = "https://login.microsoftonline.com/organizations/oauth2/v2.0/authorize?client_id=REDACTED_MS_CLIENT_ID&response_type=code&redirect_uri=" + redirectUrl + "&response_mode=query&scope=User.Read offline_access&state=" + TokenUtils.RandomString();

            return Redirect(redirectUrl);
        }
        
        [NotImplementedYet]
        [Route("/Koppelingen/Teams/Callback")]
        public async Task<IActionResult> TeamsCallback(string code, string state, string session_state)
        {
#if DEBUG
            string redirectUrl = "https://localhost:5001/Koppelingen/Teams/Callback";
#elif RELEASE
            string redirectUrl = "https://zermos.kronk.tech/Koppelingen/Teams/Callback";
#endif
            
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://login.microsoftonline.com/organizations/oauth2/v2.0/token");
            var collection = new List<KeyValuePair<string, string>>();
            collection.Add(new("client_id", "REDACTED_MS_CLIENT_ID"));
            collection.Add(new("scope", "User.Read"));
            //collection.Add(new("scope", "User.Read EduAssignments.Read"));
            collection.Add(new("code", code));
            collection.Add(new("redirect_uri", redirectUrl));
            collection.Add(new("grant_type", "authorization_code"));
            collection.Add(new("client_secret", "lcV8Q~GbQjBv45fivMgN3ARP~UHPNSuV259gQcU7"));
            var content = new FormUrlEncodedContent(collection);
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();
            
            TeamsAuthenticationModel auth = JsonConvert.DeserializeObject<TeamsAuthenticationModel>(responseString);

            request = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", "Bearer " + auth.access_token);
            
            response = await client.SendAsync(request);
            
            TeamsUserModel user = JsonConvert.DeserializeObject<TeamsUserModel>(await response.Content.ReadAsStringAsync());
            
            return Ok(user);
        }
        #endregion
    }
}