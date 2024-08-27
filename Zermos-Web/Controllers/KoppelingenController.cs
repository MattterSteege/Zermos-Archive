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
using Zermos_Web.APIs;
using Zermos_Web.Models;
using Zermos_Web.Models.Requirements;
using Zermos_Web.Models.Somtoday;
using Zermos_Web.Models.zermeloUserModel;
using Zermos_Web.Utilities;

namespace Zermos_Web.Controllers
{
    public class KoppelingenController : BaseController
    {
        public KoppelingenController(Users user, Shares share, ILogger<BaseController> logger) : base(user, share, logger) { }
        
        SomtodayAPI somtodayApi = new(new HttpClient());
        InfowijsApi InfowijsApi = new(new HttpClient());

        private readonly HttpClient _infowijsHttpClient = new()
        {
            DefaultRequestHeaders =
            {
                {"accept", "application/vnd.infowijs.v1+json"},
                {"x-infowijs-client", "nl.infowijs.hoy.android"}
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
        [Authorize]
        [ZermosPage]
        public IActionResult Index()
        {
            return PartialView();
        }

        #region ontkoppelen

        [Authorize]
        [HttpPost("/Koppelingen/Ontkoppel/{app}")]
        public IActionResult Ontkoppel(string app)
        {
            //return BadRequest("Temporary disabled");

            switch (app)
            {
                case "infowijs":
                    ZermosUser = new user {infowijs_access_token = string.Empty};
                    return Ok("success");
            
                case "somtoday":
                    ZermosUser = new user
                    {
                        somtoday_access_token = string.Empty,
                        somtoday_refresh_token = string.Empty,
                        somtoday_student_id = string.Empty
                    };
                    return Ok("success");
            
                case "zermelo":
                    ZermosUser = new user
                    {
                        zermelo_access_token = string.Empty,
                        zermelo_access_token_expires_at = DateTime.MinValue
                    };
                    return Ok("success");
            
                default:
                    return Ok("failed");
            }
        }

        #endregion

        #region infowijs

        [HttpGet]
        [Authorize]
        [ZermosPage]
        [Route("/Koppelingen/Infowijs/Ongekoppeld")]
        public IActionResult InfowijsNietGekoppeld()
        {
            return PartialView();
        }

        [HttpGet]
        [Authorize]
        [ZermosPage]
        public IActionResult Infowijs()
        {
            return PartialView();
        }

        [HttpGet]
        [Authorize]
        [ZermosPage]
        [Route("Koppelingen/Infowijs/Email")]
        public IActionResult InfowijsWithEmail(string email, bool retry = false)
        {
            ViewData["retry"] = false;
            return PartialView(model: "");
        }

        [HttpPost]
        [Authorize]
        [Route("Koppelingen/Infowijs/Email")]
        public async Task<IActionResult> InfowijsWithEmail(string email, string customer_product_id, string user_id, string id)
        {
            if (email != null && (customer_product_id == null || user_id == null || id == null))
            {
                InfowijsCustomerProductsModel productsModel = await InfowijsApi.GetCustomerProductsAsync(email);
                
                if (productsModel == null || productsModel.data.Count == 0)
                    return Ok("failed");
                
                var url1 = "https://api.infowijs.nl/sessions";
                var response1 = _infowijsHttpClient.PostAsync(url1, new StringContent(JsonConvert.SerializeObject(new
                {
                    username = email,
                    communityName = productsModel.data[0].name,
                }), Encoding.UTF8, "application/json")).Result;

                var result1 = response1.Content.ReadAsStringAsync().Result;
                var antoniusAppAuthenticatieModelData =
                    JsonConvert.DeserializeObject<AntoniusAppAuthenticatieModel>(result1);
                
                // return Ok(email + ":customer_product_id=" +
                //           antoniusAppAuthenticatieModelData.data.customer_product_id + "&user_id=" +
                //           antoniusAppAuthenticatieModelData.data.user_id + "&id=" +
                //           antoniusAppAuthenticatieModelData.data.id);
                
                return Json(new {email, antoniusAppAuthenticatieModelData.data.customer_product_id, antoniusAppAuthenticatieModelData.data.user_id, antoniusAppAuthenticatieModelData.data.id});
            }

            var response2 = await _infowijsHttpClient.PostAsync(
                "https://api.infowijs.nl/sessions/" + id + "/" + customer_product_id + "/" + user_id, null);
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
        [Authorize]
        [ZermosPage]
        [Route("Koppelingen/Infowijs/Qr")]
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
        [Authorize]
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
        [Authorize]
        [ZermosPage]
        [Route("/Koppelingen/Zermelo/Ongekoppeld")]
        public IActionResult ZermeloNietGekoppeld()
        {
            return PartialView();
        }

        [HttpGet]
        [Authorize]
        [ZermosPage]
        public IActionResult Zermelo()
        {
            return PartialView();
        }

        [HttpPost]
        [Authorize]
        [Route("/Koppelingen/Zermelo/Wachtwoord")]
        public async Task<IActionResult> ZermeloWithPassword(string username, string password, string portal = "ccg")
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

            var response = await _zermeloHttpClient.PostAsync($"https://{portal}.zportal.nl/api/v3/oauth",
                new FormUrlEncodedContent(form));
            var responseString = await response.Content.ReadAsStringAsync();

            var accessToken = Regex.Matches(responseString, "[a-zA-Z0-9]{20}")[0].Value;

            form = new Dictionary<string, string>();
            form.Add("code", accessToken);
            form.Add("client_id", "ZermeloPortal");
            form.Add("client_secret", "42");
            form.Add("grant_type", "authorization_code");
            form.Add("rememberMe", "true");

            response = await _zermeloHttpClient.PostAsync($"https://{portal}.zportal.nl/api/v3/oauth/token",
                new FormUrlEncodedContent(form));
            responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) return Ok("failed");

            var zermeloAuthentication = JsonConvert.DeserializeObject<ZermeloAuthenticatieModel>(responseString);

            var zermeloUser = await GetZermeloUser(zermeloAuthentication.access_token, portal);

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
        [Authorize]
        [ZermosPage]
        [Route("/Koppelingen/Zermelo/Qr")]
        public IActionResult ZermeloWithQr()
        {
            return PartialView();
        }

        [HttpGet]
        [Authorize]
        [ZermosPage]
        [Route("/Koppelingen/Zermelo/Code")]
        public IActionResult ZermeloWithCode()
        {
            return PartialView();
        }

        [HttpPost]
        [Authorize]
        [Route("/Koppelingen/Zermelo/Code")]
        public async Task<IActionResult> ZermeloWithCode(string code, string institution)
        {
            //POST /oauth/token?grant_type=authorization_code&code=
            
            if (code.IsNullOrEmpty() || institution.IsNullOrEmpty())
                return Ok("invalid");
            
            
            var url =
                $"https://{institution}.zportal.nl/api/v3/oauth/token?grant_type=authorization_code&code={code.Replace(" ", "")}";
            var response = await _zermeloHttpClient.PostAsync(url, null);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return Ok("expired");
            
            var zermeloAuthentication = JsonConvert.DeserializeObject<ZermeloAuthenticatieModel>(responseString);

            var zermeloUser = await GetZermeloUser(zermeloAuthentication.access_token, institution);

            ZermosUser = new user
            {
                zermelo_access_token = zermeloAuthentication.access_token,
                school_id = zermeloUser.response.data[0].code,
                zermelo_school_abbr = institution,
                name = zermeloUser.response.data[0].displayName ?? zermeloUser.response.data[0].firstName + " " + zermeloUser.response.data[0].prefix + " " +
                       zermeloUser.response.data[0].lastName,
                zermelo_access_token_expires_at = DateTime.Now.AddMonths(2)
            };


            return Ok("success");
        }

        private async Task<ZermeloUserModel> GetZermeloUser(string access_token, string institution)
        {
            var url = $"https://{institution}.zportal.nl/api/v3/users/~me?access_token=" + access_token;
            var response = await _zermeloHttpClient.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ZermeloUserModel>(responseString);
        }

        #endregion

        #region Somtoday

        [HttpGet]
        [Authorize]
        [ZermosPage]
        [Route("/Koppelingen/Somtoday/Ongekoppeld")]
        public IActionResult SomtodayNietGekoppeld()
        {
            return PartialView();
        }

        [HttpGet]
        [Authorize]
        [ZermosPage]
        [Route("/Koppelingen/Somtoday")]
        public IActionResult Somtoday()
        {
            return PartialView();
        }
        
        [HttpGet]
        [Authorize]
        [ZermosPage]
        [Route("/Koppelingen/Somtoday/HackerMan")]
        public IActionResult SomtodayHackerman()
        {
            return PartialView();
        }
        
        [HttpGet]
        [Authorize]
        [ZermosPage]
        [Route("/Koppelingen/Somtoday/Callback")]
        public async Task<IActionResult> SomtodayCallback()
        {
            //get the code behind the ? in the url
            if (Request.QueryString.Value != null)
            {
                var refresh_token = Request.QueryString.Value.Remove(0, 1).Split("&")[0];
                
                var somtoday = await somtodayApi.RefreshTokenAsync(refresh_token);
            
                if (somtoday != null)
                {
                    var user = await GetSomtodayStudent(somtoday.access_token);
                    
                    ZermosUser = new user
                    {
                        somtoday_access_token = somtoday.access_token,
                        somtoday_refresh_token = somtoday.refresh_token,
                        somtoday_student_id = user.somtoday_student_id,
                    };
                    return PartialView(model: "success");
                }
            }

            return PartialView(model: "failed");
        }

        [HttpGet]
        [Authorize]
        [ZermosPage]
        [Route("/Koppelingen/Somtoday/Inloggegevens")]
        public IActionResult SomtodayWithCreds()
        {
            return PartialView();
        }
        
        [HttpPost]
        [Authorize]
        [Route("/Koppelingen/Somtoday/Inloggegevens")]
        public async Task<IActionResult> SomtodayCreds(string username, string password, string school)
        {
            string production_authenticator_stickiness = "";
            string jsessionid = "";
            
            if (username.IsNullOrEmpty() || password.IsNullOrEmpty() || school.IsNullOrEmpty())
                return Ok("failed, one or more fields are empty");
            
            username = stringUtils.DecodeBase64Url(username);
            password = stringUtils.DecodeBase64Url(password);

            var tokens = GenerateTokens();
            //0 = code verifier
            //1 = code challenge

            //code challenge: __JVhs4cj-iqe8ha5750d9QSWJMpV49SXHPqBgFulkk
            //code verifier: 16BBJMtEJe8blIJY848ROvvO02F5V205l5A10x_DqFE

            var baseurl = string.Format(
                "https://inloggen.somtoday.nl/oauth2/authorize?redirect_uri=somtoday://nl.topicus.somtoday.leerling/oauth/callback&client_id=somtoday-leerling-native&response_type=code&state={0}&scope=openid&tenant_uuid={1}&session=no_session&code_challenge={2}&code_challenge_method=S256&knf_entree_notification",
                TokenUtils.RandomString(8), school,
                tokens[1]);
            
            var response = await _httpClientWithoutRedirect.GetAsync(baseurl);
            var authCode = response.Headers.Location.Query.Remove(0, 6);
            var fullLocation = response.Headers.Location.ToString();
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
            response = await _httpClientWithoutRedirect.GetAsync(fullLocation);
            //get (JSESSIONID) cookie
            foreach (var cookie in response.Headers.GetValues("Set-Cookie"))
            {
                if (cookie.Contains("JSESSIONID"))
                {
                    jsessionid = cookie.Split(";")[0];
                    break;
                }
            }
            
            //Check if user is 2 step login or 3 step login
            _somtodayHttpClientWithoutRedirect.DefaultRequestHeaders.Add("Cookie", production_authenticator_stickiness);
            _somtodayHttpClientWithoutRedirect.DefaultRequestHeaders.Add("Cookie", jsessionid);
            var formInputs = Regex.Matches((await (await _httpClientWithoutRedirect.GetAsync(response.Headers.Location)).Content.ReadAsStringAsync()), "form--input", RegexOptions.Multiline).Count;
            if (formInputs != 2 && formInputs != 3)
                return Ok("failed, input field count is not 2 or 3");
            
            
            
            if (jsessionid == "")
                return Ok("failed, no JSESSIONID cookie found");
            
            string finalAuthCode = null;

            if (formInputs == 2)
                finalAuthCode = await TripleLogin(username, password, jsessionid, authCode);
            else
                finalAuthCode = await DoubleLogin(username, password, jsessionid, authCode);

            if (finalAuthCode == null)
                return Ok("failed, no finalAuthCode found");
            
            
            baseurl =
                "https://inloggen.somtoday.nl/oauth2/token?grant_type=authorization_code&session=no_session&scope=openid&client_id=somtoday-leerling-native&tenant_uuid=c23fbb99-be4b-4c11-bbf5-57e7fc4f4388&code=" +
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

        private async Task<string> DoubleLogin(string username, string password, string jsessionid, string authCode)
        {
            _somtodayHttpClientWithoutRedirect.DefaultRequestHeaders.Add("Cookie", jsessionid);

            string baseurl = "https://inloggen.somtoday.nl/?0-1.-panel-signInForm&auth=" + authCode;
            
            var Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"loginLink", "x"},
                {"usernameFieldPanel:usernameFieldPanel_body:usernameField", username},
                {"passwordFieldPanel:passwordFieldPanel_body:passwordField", password}
            });
            
            var response = await _somtodayHttpClientWithoutRedirect.PostAsync(baseurl, Content);
            
            return HTMLUtils.ParseQuery(response.Headers.Location.Query)["code"];
        }
        
        private async Task<string> TripleLogin(string username, string password, string jsessionid, string authCode)
        {
            //set the cookie to the _somtodayHttpClientWithoutRedirect
            _somtodayHttpClientWithoutRedirect.DefaultRequestHeaders.Add("Cookie", jsessionid);


            //_somtodayHttpClientWithoutRedirect.DefaultRequestHeaders.Add("origin", "https://inloggen.somtoday.nl");
            
            
            string baseurl = "https://inloggen.somtoday.nl/?0-1.-panel-signInForm&auth=" + authCode;
            
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
            
            var response = await _somtodayHttpClientWithoutRedirect.PostAsync(baseurl, Content);
            
            return HTMLUtils.ParseQuery(response.Headers.Location.Query)["code"];
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
        
        [HttpGet]
        [Authorize]
        [ZermosPage]
        [Route("/Koppelingen/Somtoday/STAP")]
        public async Task<IActionResult> SomtodayWithSTAP()
        {
            SomtodayPreAuthenticationModel model = new()
            {
                user = ZermosEmail,
#if (RELEASE && !BETA)
                callbackUrl = "https://zermos.kronk.tech/Koppelingen/Somtoday/STAP"
#elif (RELEASE && BETA)
                callbackUrl = "https://zermos-beta.kronk.tech/Koppelingen/Somtoday/STAP"
#else
                callbackUrl = "https://test-server.kronk.tech/Koppelingen/Somtoday/STAP"
#endif
            };
            
            //request url at https://somtoday.kronk.tech/requestUrl?user=ZermosEmail&callbackUrl=callbackUrl
            var requestUrl = "https://somtoday.kronk.tech/requestUrl?user=" + model.user + "&callbackUrl=" + model.callbackUrl;
            var response = _normalHttpClient.GetAsync(requestUrl).Result;
            model = JsonConvert.DeserializeObject<SomtodayPreAuthenticationModel>(await response.Content.ReadAsStringAsync());
            
            return PartialView(model);
        }
        
        [HttpGet]
        [Authorize]
        [Route("/Koppelingen/Somtoday/STAP/isCompleted")]
        public IActionResult SomtodayWithSTAPCompleted()
        {
            var user = ZermosUser;
            if (user.somtoday_access_token.IsNullOrEmpty() || user.somtoday_refresh_token.IsNullOrEmpty() || user.somtoday_student_id.IsNullOrEmpty() || !TokenUtils.CheckToken(user.somtoday_refresh_token))
                return Ok(false);
            
            return Ok(true);
        }

        [HttpPost]
        [Route("/Koppelingen/Somtoday/STAP")]
        public async Task<IActionResult> LegitAuth([FromBody] SomtodayPreAuthenticationModel model)
        {
            if (model == null)
                return Ok("failed");
            
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://inloggen.somtoday.nl/oauth2/token");
            
            request.Headers.Add("accept", "application/json, text/plain, */*");
            request.Headers.Add("origin", "https://leerling.somtoday.nl");
            
            var collection = new List<KeyValuePair<string, string>>();
            collection.Add(new("grant_type", "authorization_code"));
            collection.Add(new("code", model.code.UrlDecode()));
            collection.Add(new("redirect_uri", "somtoday://nl.topicus.somtoday.leerling/oauth/callback"));
            collection.Add(new("code_verifier", model.code_verifier.UrlDecode()));
            collection.Add(new("client_id", "somtoday-leerling-native"));
            collection.Add(new("claims", "{\"id_token\":{\"given_name\":null, \"leerlingen\":null, \"orgname\": null, \"affiliation\":{\"values\":[\"student\",\"parent/guardian\"]} }}"));
            var content = new FormUrlEncodedContent(collection);
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            
            
            var responseString = await response.Content.ReadAsStringAsync();
            var somtodayAuthentication = JsonConvert.DeserializeObject<SomtodayAuthenticatieModel>(responseString);
            
            var user = await GetSomtodayStudent(somtodayAuthentication.access_token);
            user.somtoday_access_token = somtodayAuthentication.access_token;
            user.somtoday_refresh_token = somtodayAuthentication.refresh_token;
            
            await _user.UpdateUserAsync(model.user, user);
            return Ok("Fuck off");
        }
        #endregion
    }
}