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
    public class KoppelingenController : BaseController
    {
        public KoppelingenController(Users user, Shares share, ILogger<BaseController> logger) : base(user, share,
            logger)
        {
        }

        private readonly HttpClient _infowijsHttpClient = new()
        {
            DefaultRequestHeaders =
            {
                {"accept", "application/vnd.infowijs.v1+json"},
                {"x-infowijs-client", "nl.infowijs.hoy.android/nl.infowijs.client.antonius"}
            }
        };

        private readonly HttpClient _zermeloHttpClient = new()
        {
            DefaultRequestHeaders =
            {
                {"User-Agent", "Zermos-Web"}
            }
        };

        private readonly HttpClient _somtodayHttpClient = new()
        {
            DefaultRequestHeaders =
            {
                {"origin", "https://inloggen.somtoday.nl"},
                {"accept", "application/json"}
            }
        };

        private readonly HttpClient _somtodayHttpClientWithoutRedirect =
            new(new HttpClientHandler {AllowAutoRedirect = false})
            {
                DefaultRequestHeaders =
                {
                    {"origin", "https://inloggen.somtoday.nl"},
                }
            };

        private readonly HttpClient _httpClientWithoutRedirect = new(new HttpClientHandler {AllowAutoRedirect = false});
        private readonly HttpClient _normalHttpClient = new();

        [HttpGet]
        [ZermosPage]
        public IActionResult Index()
        {
            return PartialView();
        }

        #region ontkoppelen

        [HttpPost("ontkoppel/{app}")]
        public IActionResult Ontkoppel(string app)
        {
            //return BadRequest("Temporary disabled");

            switch (app)
            {
                case "infowijs":
                    ZermosUser = new user {infowijs_access_token = string.Empty};
                    return Redirect("/account");
            
                case "somtoday":
                    ZermosUser = new user
                    {
                        somtoday_access_token = string.Empty,
                        somtoday_refresh_token = string.Empty,
                        somtoday_student_id = string.Empty
                    };
                    return Redirect("/account");
            
                case "zermelo":
                    ZermosUser = new user
                    {
                        zermelo_access_token = string.Empty,
                        zermelo_access_token_expires_at = DateTime.MinValue
                    };
                    return Redirect("/account");
            
                default:
                    return Redirect("/account");
            }
        }

        #endregion

        #region infowijs

        [HttpGet]
        [ZermosPage]
        [Route("/Koppelingen/Infowijs/Ongekoppeld")]
        public IActionResult InfowijsNietGekoppeld()
        {
            return PartialView();
        }

        [HttpGet]
        [ZermosPage]
        public IActionResult Infowijs()
        {
            return PartialView();
        }

        [HttpGet]
        [Route("Koppelingen/Infowijs/Email")]
        [ZermosPage]
        public IActionResult InfowijsWithEmail(string email, bool retry = false)
        {
            ViewData["retry"] = false;
            return PartialView(model: "");
        }

        [HttpPost]
        [Route("Koppelingen/Infowijs/Email")]
        public async Task<IActionResult> InfowijsWithEmail(string email, string customer_product_id, string user_id,
            string id)
        {
            if (email != null && (customer_product_id == null || user_id == null || id == null))
            {
                var url1 = "https://api.infowijs.nl/sessions";
                var response1 = _infowijsHttpClient.PostAsync(url1, new StringContent(JsonConvert.SerializeObject(new
                {
                    username = email,
                    communityName = "antonius"
                }), Encoding.UTF8, "application/json")).Result;

                var result1 = response1.Content.ReadAsStringAsync().Result;
                var antoniusAppAuthenticatieModelData =
                    JsonConvert.DeserializeObject<AntoniusAppAuthenticatieModel>(result1);

                // ViewData["email"] = email;
                // ViewData["customer_product_id"] = antoniusAppAuthenticatieModelData.data.customer_product_id;
                // ViewData["user_id"] = antoniusAppAuthenticatieModelData.data.user_id;
                // ViewData["id"] = antoniusAppAuthenticatieModelData.data.id;
                // ViewData["retry"] = false;
                return Ok("?" + email + "?customer_product_id=" +
                          antoniusAppAuthenticatieModelData.data.customer_product_id + "&user_id=" +
                          antoniusAppAuthenticatieModelData.data.user_id + "&id=" +
                          antoniusAppAuthenticatieModelData.data.id);
            }

            var response2 = await _infowijsHttpClient.PostAsync(
                "https://api.infowijs.nl/sessions/" + id + "/77584871-d26b-11ea-8b2e-060ffde8896c/" + user_id, null);
            var result2 = await response2.Content.ReadAsStringAsync();

            try
            {
                var jwt = JsonConvert.DeserializeObject<AntoniusAppAuthenticatieModelAuthSuccess>(result2).data;

                ZermosUser = new user {infowijs_access_token = jwt};

                return Ok("success");
            }
            catch
            {
                return Ok("failed");
            }
        }

        [HttpGet]
        [Route("Koppelingen/Infowijs/Qr")]
        [ZermosPage]
        public async Task<IActionResult> InfowijsQr(string uuid, bool retry = false)
        {
            if (uuid != null)
            {
                ViewData["qr_text"] = "hoy_scan://v1/login/" + uuid;
                ViewData["uuid"] = uuid;
                ViewData["retry"] = retry;
                return PartialView(model: "");
            }

            var url1 = "https://api.infowijs.nl/sessions/transfer";
            var response1 = await _infowijsHttpClient.PostAsync(url1, null);
            var result1 = await response1.Content.ReadAsStringAsync();

            uuid = JsonConvert.DeserializeObject<AntoniusAppAuthenticatieModelAuthSuccess>(result1).data;

            ViewData["qr_text"] = "hoy_scan://v1/login/" + uuid;
            ViewData["uuid"] = uuid;
            ViewData["retry"] = retry;

            return PartialView(model: "");
        }

        [HttpPost]
        [Route("Koppelingen/Infowijs/Qr")]
        public async Task<IActionResult> InfowijsQr(string uuid)
        {
            if (uuid == null)
            {
                return Ok("failed");
            }

            var url = "https://api.infowijs.nl/sessions/transfer/" + uuid;
            var response = await _infowijsHttpClient.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                ViewData["qr_text"] = "hoy_scan://v1/login/" + uuid;
                ViewData["uuid"] = uuid;
                ViewData["retry"] = true;
                return Ok("failed");
            }

            var jwt = JsonConvert.DeserializeObject<AntoniusAppAuthenticatieModelAuthSuccess>(result).data;

            var email = User.FindFirstValue("email");
            ZermosUser = new user {infowijs_access_token = jwt};

            return Ok("success");
        }

        #endregion

        #region Zermelo

        [HttpGet]
        [ZermosPage]
        [Route("/Koppelingen/Zermelo/Ongekoppeld")]
        public IActionResult ZermeloNietGekoppeld()
        {
            return PartialView();
        }

        [HttpGet]
        [ZermosPage]
        public IActionResult Zermelo()
        {
            return PartialView();
        }

        [HttpPost]
        [Route("/Koppelingen/Zermelo/Wachtwoord")]
        public async Task<IActionResult> ZermeloWithPassword(string username, string password)
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
            var responseString = await response.Content.ReadAsStringAsync();

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

            if (!response.IsSuccessStatusCode) return Ok("failed");

            var zermeloAuthentication = JsonConvert.DeserializeObject<ZermeloAuthenticatieModel>(responseString);

            var zermeloUser = await GetZermeloUser(zermeloAuthentication.access_token);

            ZermosUser = new user
            {
                zermelo_access_token = zermeloAuthentication.access_token,
                school_id = zermeloUser.response.data[0].code,
                name = zermeloUser.response.data[0].firstName + " " + zermeloUser.response.data[0].prefix + " " +
                       zermeloUser.response.data[0].lastName,
                zermelo_access_token_expires_at = DateTime.Now.AddMonths(2)
            };

            return Ok("success");
        }

        [HttpGet]
        [ZermosPage]
        [Route("/Koppelingen/Zermelo/Qr")]
        public IActionResult ZermeloWithQr()
        {
            return PartialView();
        }

        [HttpGet]
        [ZermosPage]
        [Route("/Koppelingen/Zermelo/Code")]
        public IActionResult ZermeloWithCode()
        {
            return PartialView();
        }

        [HttpPost]
        [Route("/Koppelingen/Zermelo/Code")]
        public async Task<IActionResult> ZermeloWithCode(string code)
        {
            //POST /oauth/token?grant_type=authorization_code&code=
            var url =
                $"https://ccg.zportal.nl/api/v3/oauth/token?grant_type=authorization_code&code={code.Replace(" ", "")}";
            var response = await _zermeloHttpClient.PostAsync(url, null);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                HttpContext.AddNotification("Niet geldig",
                    "Deze code is mogelijk niet geldig, refresh zermelo en probeer het opnieuw",
                    NotificationCenter.NotificationType.ERROR);

                return Ok("failed");
            }

            var zermeloAuthentication = JsonConvert.DeserializeObject<ZermeloAuthenticatieModel>(responseString);

            var zermeloUser = await GetZermeloUser(zermeloAuthentication.access_token);

            ZermosUser = new user
            {
                zermelo_access_token = zermeloAuthentication.access_token,
                school_id = zermeloUser.response.data[0].code,
                name = zermeloUser.response.data[0].firstName + " " + zermeloUser.response.data[0].prefix + " " +
                       zermeloUser.response.data[0].lastName,
                zermelo_access_token_expires_at = DateTime.Now.AddMonths(2)
            };


            return Ok("success");
        }

        private async Task<ZermeloUserModel> GetZermeloUser(string access_token)
        {
            var url = "https://ccg.zportal.nl/api/v3/users/~me?access_token=" + access_token;
            var response = await _zermeloHttpClient.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ZermeloUserModel>(responseString);
        }

        #endregion

        #region Somtoday

        [HttpGet]
        [ZermosPage]
        [Route("/Koppelingen/Somtoday/Ongekoppeld")]
        public IActionResult SomtodayNietGekoppeld()
        {
            return PartialView();
        }

        [HttpGet]
        [ZermosPage]
        public IActionResult Somtoday()
        {
            return PartialView();
        }


        [HttpPost]
        public async Task<IActionResult> Somtoday(string username, string password)
        {
            string production_authenticator_stickiness = "";
            string jsessionid = "";
            
            username = stringUtils.DecodeBase64Url(username);
            password = stringUtils.DecodeBase64Url(password);

            var tokens = GenerateTokens();
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
            //get the cookies (production-authenticator-stickiness)
            foreach (var cookie in response.Headers.GetValues("Set-Cookie"))
            {
                if (cookie.Contains("production-authenticator-stickiness"))
                {
                    production_authenticator_stickiness = cookie.Split(";")[0];
                    break;
                }
            }
            
            if (production_authenticator_stickiness == "")
                return Ok("failed, no production-authenticator-stickiness cookie found");

            
            
                //set the cookie to the _somtodayHttpClientWithoutRedirect
            _somtodayHttpClientWithoutRedirect.DefaultRequestHeaders.Add("Cookie", production_authenticator_stickiness);
            
            //send http request to to location
            baseurl = response.Headers.Location.ToString();
            response = await _httpClientWithoutRedirect.GetAsync(baseurl);
            //get (JSESSIONID) cookie
            foreach (var cookie in response.Headers.GetValues("Set-Cookie"))
            {
                if (cookie.Contains("JSESSIONID"))
                {
                    jsessionid = cookie.Split(";")[0];
                    break;
                }
            }
            
            if (jsessionid == "")
                return Ok("failed, no JSESSIONID cookie found");
            
            
            //set the cookie to the _somtodayHttpClientWithoutRedirect
            _somtodayHttpClientWithoutRedirect.DefaultRequestHeaders.Add("Cookie", jsessionid);


            //_somtodayHttpClientWithoutRedirect.DefaultRequestHeaders.Add("origin", "https://inloggen.somtoday.nl");
            
            
            baseurl = "https://inloggen.somtoday.nl/?0-1.-panel-signInForm&auth=" + authCode;
            
            var Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"loginLink", "x"},
                {"usernameFieldPanel:usernameFieldPanel_body:usernameField", username}
            });
            
            await _somtodayHttpClientWithoutRedirect.PostAsync(baseurl, Content);
            
            baseurl = "https://inloggen.somtoday.nl/login?2-1.-passwordForm";
            
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"passwordFieldPanel:passwordFieldPanel_body:passwordField", password},
                {"loginLink", "x"}
            });
            
            response = await _somtodayHttpClientWithoutRedirect.PostAsync(baseurl, Content);
            
            //redirectUri=https://inloggen.somtoday.nl/%3Fauth%3DeyJ4NXQjUzI1NiI6IkdpZ295b0kyZXcxQS00TDUweGoyWGlPdXIxdE9BMFo3M05mYmZuQXFkU3ciLCJraWQiOiJpcmlkaXVtaWRwLTE2NjgzMzc3ODYzMTA4ODY0NzQwNTkwOTk4NzcyNDAzMjI1MTM0NSIsInR5cCI6IkpXVCIsImFsZyI6IlJTMjU2In0.eyJyZW1lbWJlciI6ZmFsc2UsImFsbENsaWVudHMiOlsiRDUwRTBDMDYtMzJEMS00QjQxLUExMzctQTlBODUwQzg5MkMyIl0sInByb3QiOiJPQVVUSDIiLCJtZXRob2QiOiJwd2QiLCJub1Nlc3Npb24iOnRydWUsImNsaWVudCI6IkQ1MEUwQzA2LTMyRDEtNEI0MS1BMTM3LUE5QTg1MEM4OTJDMiIsImF1dGhUaW1lIjoiMjAyNC0wNi0wMVQxNzoxNDoxMS40NjM2MDMwNzRaIiwiZXhwIjoxNzE3MjY1NjUxLCJwYXJhbWV0ZXJzIjp7InRlbmFudF91dWlkIjpbImMyM2ZiYjk5LWJlNGItNGMxMS1iYmY1LTU3ZTdmYzRmNDM4OCJdLCJzZXNzaW9uIjpbIm5vX3Nlc3Npb24iXSwic2NvcGUiOlsib3BlbmlkIl0sInJlc3BvbnNlX3R5cGUiOlsiY29kZSJdLCJyZWRpcmVjdF91cmkiOlsic29tdG9kYXlsZWVybGluZzovL29hdXRoL2NhbGxiYWNrIl0sInN0YXRlIjpbIkprVXZ0bXI5Il0sImNvZGVfY2hhbGxlbmdlX21ldGhvZCI6WyJTMjU2Il0sIm9yaWdpbmFsX2p3dGlkIjpbIiJdLCJjbGllbnRfaWQiOlsiRDUwRTBDMDYtMzJEMS00QjQxLUExMzctQTlBODUwQzg5MkMyIl0sImNvZGVfY2hhbGxlbmdlIjpbImZ1WjlrWmNQYlZ6VlpHOW1nMkdtY0JMUENMSUhOZC1jNG8tVTlQaWJwLTAiXSwia25mX2VudHJlZV9ub3RpZmljYXRpb24iOiJrbmZfZW50cmVlX25vdGlmaWNhdGlvbiJ9fQ.rf9oI3KaNDdYTazxbFa3MimkSwaoEuP-CXrrrFLHCeSfDIRv1Mwrzx55Noh71kzz7qUevFAcRb68XVf5BmP2CgfRgBBYC0DKg_9nXnEld9wN_LoZgeXyijLkMzWf9_MtlrL4VMcfG86W-thKQv371eSn6cv810WbHcmaxO5rrDIjwXPC6ul7_OcfBT1D4hhuFq8pdykQ0nBCQAs-Gno9FUqkxZuTVQi_CComLToMFSaM9IGlMI-o3HC1t5ctkje6UoZe9IlT-MrK_KQcmQVaUGzvXlPug9-5JBNdza97uee1TflDi8ZscVnjB_QBWMX5UeZq2assxG7WUcZbVVmKbQ
            //deze moet je vinden en urldecoden en dan de ?auth= eruit halen
            //dan moet je https://inloggen.somtoday.nl/?0-1.-panel-signInForm&auth= invullen met data zoals hierboven
            //dan moet je https://inloggen.somtoday.nl/login?2-1.-passwordForm invullen met data zoals hierboven
            //dan krijg je de somtodayleerling://oauth/callback?code= en die moet je hieroner invullen
            
            
            //HIERONDER IS BROKEN!!
            var finalAuthCode = HTMLUtils.ParseQuery(response.Headers.Location.Query)["code"];

            if (finalAuthCode == null)
                return Ok("failed, no finalAuthCode found");
            
            
            baseurl =
                "https://inloggen.somtoday.nl/oauth2/token?grant_type=authorization_code&session=no_session&scope=openid&client_id=D50E0C06-32D1-4B41-A137-A9A850C892C2&tenant_uuid=c23fbb99-be4b-4c11-bbf5-57e7fc4f4388&code=" +
                finalAuthCode + "&code_verifier=" + tokens[0];
            
            response = await _somtodayHttpClientWithoutRedirect.PostAsync(baseurl,
                new FormUrlEncodedContent(new Dictionary<string, string> {{"", ""}}));
            
            var somtodayAuthentication =
                JsonConvert.DeserializeObject<SomtodayAuthenticatieModel>(response.Content.ReadAsStringAsync()
                    .Result);

            var user = await GetSomtodayStudent(somtodayAuthentication.access_token);
            user.somtoday_access_token = somtodayAuthentication.access_token;
            user.somtoday_refresh_token = somtodayAuthentication.refresh_token;

            ZermosUser = user;
            return Ok("success");
        }


        private async Task<user> GetSomtodayStudent(string auth_token)
        {
            //GET: https://api.somtoday.nl/rest/v1/leerlingen?additional=pasfoto
            var baseurl = "https://api.somtoday.nl/rest/v1/leerlingen";

            _somtodayHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", auth_token);

            var response = await _somtodayHttpClient.GetAsync(baseurl);
            var responseString = await response.Content.ReadAsStringAsync();
            var somtodayStudent =
                JsonConvert.DeserializeObject<SomtodayStudentModel>(responseString);
            return new user
            {
                somtoday_student_id = somtodayStudent.items[0].links[0].id.ToString(),
            };
        }

        public string[] GenerateTokens()
        {
            var tokens = new string[2];
            tokens[0] = GenerateNonce();
            tokens[1] = GenerateCodeChallenge(tokens[0]);
            return tokens;
        }

        private string GenerateNonce()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz123456789";
            var nonce = new char[128];
            for (var i = 0; i < nonce.Length; i++)
                nonce[i] = chars[new Random().Next(0, chars.Length)];

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
    }
}