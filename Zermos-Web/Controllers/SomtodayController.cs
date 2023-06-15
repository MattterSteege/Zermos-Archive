using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Zermos_Web.Models;
using Zermos_Web.Models.SomtodayGradesModel;
using Zermos_Web.Models.somtodayHomeworkModel;
using Zermos_Web.Utilities;
using Item = Zermos_Web.Models.SomtodayGradesModel.Item;

namespace Zermos_Web.Controllers
{
    [Authorize]
    public class SomtodayController : Controller
    {
        private readonly ILogger<SomtodayController> _logger;
        private readonly Users _users;
        private readonly HttpClient _httpClient;
        private readonly HttpClient _httpClientWithoutRedirect;

        public SomtodayController(ILogger<SomtodayController> logger, Users users)
        {
            _logger = logger;
            _users = users;
            _httpClient = new HttpClient();
            _httpClientWithoutRedirect = new HttpClient(new HttpClientHandler {AllowAutoRedirect = false});
        }

        #region Cijfers

        public async Task<IActionResult> Cijfers(bool refresh_token = false)
        {
            ViewData["add_css"] = "somtoday";
            //the request was by ajax, so return the partial view
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                user user = await _users.GetUserAsync(User.FindFirstValue("email"));

                if (user.somtoday_access_token == null)
                {
                    return RedirectToAction("Inloggen", "Somtoday");
                }

                if (TokenUtils.CheckToken(user.somtoday_access_token) == false && refresh_token == false)
                {
                    ViewData["redirected_from_loadingpage"] = "true";
                    ViewData["laad_tekst"] = "Cijfers worden geladen nadat je SOMtoday token is ververst";
                    ViewData["url"] = "/" + ControllerContext.RouteData.Values["controller"] + "/" +
                                      ControllerContext.RouteData.Values["action"] + "?refresh_token=true";
                    return View("_Loading");
                }

                if (refresh_token)
                {
                    await RefreshToken(user.somtoday_refresh_token);
                    ViewData["redirected_from_loadingpage"] = "true";
                    ViewData["laad_tekst"] = "SOMtoday token is ververst, cijfers worden geladen";
                    ViewData["url"] = "/" + ControllerContext.RouteData.Values["controller"] + "/" +
                                      ControllerContext.RouteData.Values["action"];
                    return View("_Loading");
                }


                string baseUrl =
                    $"https://api.somtoday.nl/rest/v1/resultaten/huidigVoorLeerling/{user.somtoday_student_id}?begintNaOfOp={DateTime.Now:yyyy}-01-01";

                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + user.somtoday_access_token);
                _httpClient.DefaultRequestHeaders.Add("Range", "items=0-99");
                _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                var response = await _httpClient.GetAsync(baseUrl);

                var grades =
                    JsonConvert.DeserializeObject<SomtodayGradesModel>(await response.Content.ReadAsStringAsync());

                if (response.IsSuccessStatusCode == false)
                {
                    return NotFound(
                        "Er is iets fout gegaan bij het ophalen van de cijfers, het is mogelijk dat je SOMtoday token verlopen is.");
                }

                if (int.TryParse(response.Content.Headers.GetValues("Content-Range").First().Split('/')[1],
                        out int total))
                {
                    int requests = (total / 100) * 100;

                    for (int i = 100; i < requests; i += 100)
                    {
                        _httpClient.DefaultRequestHeaders.Clear();
                        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + user.somtoday_access_token);
                        _httpClient.DefaultRequestHeaders.Add("Range", $"items={i}-{i + 99}");
                        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                        response = await _httpClient.GetAsync(baseUrl);
                        var _grades =
                            JsonConvert.DeserializeObject<SomtodayGradesModel>(
                                await response.Content.ReadAsStringAsync());
                        grades.items.AddRange(_grades.items);
                    }
                }

                return View(Sort(grades));
            }

            ViewData["laad_tekst"] = "Cijfers worden geladen";
            //the request was by a legitimate user, so return the loading view
            ViewData["url"] = "/" + ControllerContext.RouteData.Values["controller"] + "/" +
                              ControllerContext.RouteData.Values["action"];
            return View("_Loading");
        }

        public async Task RefreshToken(string token = null)
        {
            if (token == null) return;

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _httpClient.DefaultRequestHeaders.Add("Origin", "https://somtoday.nl");

            var form = new Dictionary<string, string>
            {
                {"grant_type", "refresh_token"},
                {"refresh_token", token},
                {"scope", "openid"},
                {"client_id", "D50E0C06-32D1-4B41-A137-A9A850C892C2"}
            };

            var response = _httpClient
                .PostAsync("https://inloggen.somtoday.nl/oauth2/token", new FormUrlEncodedContent(form)).Result;
            var somtodayAuthentication =
                JsonConvert.DeserializeObject<SomtodayAuthenticatieModel>(response.Content.ReadAsStringAsync().Result);

            user user = new user
            {
                somtoday_access_token = somtodayAuthentication.access_token,
                somtoday_refresh_token = somtodayAuthentication.refresh_token
            };
            await _users.UpdateUserAsync(User.FindFirstValue("email"), user);
        }

        [AllowAnonymous]
        public IActionResult Cijfer(string content = null)
        {
            ViewData["add_css"] = "somtoday";
            //the request was by ajax, so return the partial view
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var a = Convert.FromBase64String(content ?? "");
                var b = System.Text.Encoding.UTF8.GetString(a);
                var c = JsonConvert.DeserializeObject<sortedGrades>(b);

                return View(c);
            }

            ViewData["laad_tekst"] = "Cijfer worden geladen";
            //the request was by a legitimate user, so return the loading view
            ViewData["url"] = "/" + ControllerContext.RouteData.Values["controller"] + "/" +
                              ControllerContext.RouteData.Values["action"] + "?content=" + content;
            return View("_Loading");
        }

        [AllowAnonymous]
        public IActionResult CijferData(string content = null)
        {
            ViewData["add_css"] = "somtoday";
            //the request was by ajax, so return the partial view
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var a = Convert.FromBase64String(content ?? "");
                var b = System.Text.Encoding.UTF8.GetString(a);
                var c = JsonConvert.DeserializeObject<Item>(b);

                return View(c);
            }

            ViewData["laad_tekst"] = "Cijfer worden geladen";
            //the request was by a legitimate user, so return the loading view
            ViewData["url"] = "/" + ControllerContext.RouteData.Values["controller"] + "/" +
                              ControllerContext.RouteData.Values["action"] + "?content=" + content;
            return View("_Loading");
        }

        public SomtodayGradesModel Sort(SomtodayGradesModel grades)
        {
            grades.items = grades.items.OrderBy(x => x.datumInvoer).ToList();
            foreach (var x in grades.items.Where(x => x.resultaatLabelAfkorting == "V"))
            {
                x.geldendResultaat = "7";
            }

            grades.items.RemoveAll(x => string.IsNullOrEmpty(x.omschrijving) && x.weging == 0);
            grades.items.RemoveAll(x => x.type == "SamengesteldeToetsKolom");
            grades.items.RemoveAll(x => x.geldendResultaat == null);

            return grades;
        }

        #endregion

        #region huiswerk

        public async Task<IActionResult> Huiswerk(bool refresh_token = false)
        {
            ViewData["add_css"] = "somtoday";
            //the request was by ajax, so return the partial view
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                user user = await _users.GetUserAsync(User.FindFirstValue("email"));

                if (string.IsNullOrEmpty(user.somtoday_access_token))
                {
                    return RedirectToAction("Inloggen", "Somtoday");
                }

                if (TokenUtils.CheckToken(user.somtoday_access_token) == false && refresh_token == false)
                {
                    ViewData["redirected_from_loadingpage"] = "true";
                    ViewData["laad_tekst"] = "Huiswerk wordt opgevraagd nadat je SOMtoday token is ververst";
                    ViewData["url"] = "/" + ControllerContext.RouteData.Values["controller"] + "/" +
                                      ControllerContext.RouteData.Values["action"] + "?refresh_token=true";
                    return View("_Loading");
                }

                if (refresh_token)
                {
                    await RefreshToken(user.somtoday_refresh_token);
                    ViewData["redirected_from_loadingpage"] = "true";
                    ViewData["laad_tekst"] = "SOMtoday token is ververst, huiswerk wordt opgevraagd";
                    ViewData["url"] = "/" + ControllerContext.RouteData.Values["controller"] + "/" +
                                      ControllerContext.RouteData.Values["action"];
                    return View("_Loading");
                }

                string _startDate = DateTime.Now.AddDays(-14).ToString("yyyy-MM-dd");
                string baseurl =
                    $"https://api.somtoday.nl/rest/v1/studiewijzeritemafspraaktoekenningen?begintNaOfOp={_startDate}&additional=swigemaaktVinkjes";

                int rangemin = 0;
                int rangemax = 99;


                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("authorization", "Bearer " + user.somtoday_access_token);
                _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                _httpClient.DefaultRequestHeaders.Add("Range", $"items={rangemin}-{rangemax}");

                var response = await _httpClient.GetAsync(baseurl);

                var somtodayHuiswerk =
                    JsonConvert.DeserializeObject<somtodayHomeworkModel>(await response.Content.ReadAsStringAsync());

                return View(Sort(somtodayHuiswerk));
            }

            ViewData["laad_tekst"] = "Huiswerk wordt opgevraagd";
            //the request was by a legitimate user, so return the loading view
            ViewData["url"] = "/" + ControllerContext.RouteData.Values["controller"] + "/" +
                              ControllerContext.RouteData.Values["action"];
            return View("_Loading");
        }

        public somtodayHomeworkModel Sort(somtodayHomeworkModel homework)
        {
            homework.items = homework.items.OrderBy(x => x.datumTijd).ToList();
            homework.items.RemoveAll(x => x.studiewijzerItem == null);
            homework.items.RemoveAll(x => x.datumTijd < DateTime.Now.AddDays(-14));
            homework.items.RemoveAll(x => x.studiewijzerItem.huiswerkType == "LESSTOF");
            return homework;
        }

        #endregion

        #region inloggen

        [HttpGet]
        public IActionResult Inloggen()
        {
            ViewData["add_css"] = "somtoday";

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Inloggen(string username, string password)
        {
            //code challenge: __JVhs4cj-iqe8ha5750d9QSWJMpV49SXHPqBgFulkk
            //code verifier: 16BBJMtEJe8blIJY848ROvvO02F5V205l5A10x_DqFE

            _httpClientWithoutRedirect.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("origin", "https://inloggen.somtoday.nl");

            string baseurl = string.Format(
                "https://inloggen.somtoday.nl/oauth2/authorize?redirect_uri=somtodayleerling://oauth/callback&client_id=D50E0C06-32D1-4B41-A137-A9A850C892C2&response_type=code&state={0}&scope=openid&tenant_uuid={1}&session=no_session&code_challenge={2}&code_challenge_method=S256",
                RandomStateString(), "c23fbb99-be4b-4c11-bbf5-57e7fc4f4388",
                "__JVhs4cj-iqe8ha5750d9QSWJMpV49SXHPqBgFulkk");

            var response = await _httpClientWithoutRedirect.GetAsync(baseurl);
            string authCode = response.Headers.Location.Query.Remove(0, 6);


            _httpClientWithoutRedirect.DefaultRequestHeaders.Add("origin", "https://inloggen.somtoday.nl");


            baseurl = "https://inloggen.somtoday.nl/?-1.-panel-signInForm&auth=" + authCode;

            var Content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                {"loginLink", "x"},
                {"usernameFieldPanel:usernameFieldPanel_body:usernameField", username}
            });

            await _httpClientWithoutRedirect.PostAsync(baseurl, Content);

            baseurl = "https://inloggen.somtoday.nl/login?1-1.-passwordForm&auth=" + authCode;

            Content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                {"passwordFieldPanel:passwordFieldPanel_body:passwordField", password},
                {"loginLink", "x"}
            });

            response = await _httpClientWithoutRedirect.PostAsync(baseurl, Content);

            string finalAuthCode = HTMLUtils.ParseQuery(response.Headers.Location.Query)["code"];


            baseurl =
                "https://inloggen.somtoday.nl/oauth2/token?grant_type=authorization_code&session=no_session&scope=openid&client_id=D50E0C06-32D1-4B41-A137-A9A850C892C2&tenant_uuid=c23fbb99-be4b-4c11-bbf5-57e7fc4f4388&code=" +
                finalAuthCode + "&code_verifier=16BBJMtEJe8blIJY848ROvvO02F5V205l5A10x_DqFE";

            response = await _httpClientWithoutRedirect.PostAsync(baseurl,
                new FormUrlEncodedContent(new Dictionary<string, string> {{"", ""}}));

            SomtodayAuthenticatieModel somtodayAuthentication =
                JsonConvert.DeserializeObject<SomtodayAuthenticatieModel>(response.Content.ReadAsStringAsync().Result);

            if (somtodayAuthentication.access_token == null)
            {
                return View("Inloggen");
            }

            user user = await GetSomtodayStudent(somtodayAuthentication.access_token);
            user.somtoday_access_token = somtodayAuthentication.access_token;
            user.somtoday_refresh_token = somtodayAuthentication.refresh_token;

            await _users.UpdateUserAsync(User.FindFirstValue("email"), user);
            return RedirectToAction("Index", "Hoofdmenu");
        }

        private async Task<user> GetSomtodayStudent(string auth_token)
        {
            //GET: https://api.somtoday.nl/rest/v1/leerlingen?additional=pasfoto
            string baseurl = "https://api.somtoday.nl/rest/v1/leerlingen?additional=pasfoto";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("authorization", "Bearer " + auth_token);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var response = await _httpClient.GetAsync(baseurl);
            var somtodayStudent =
                JsonConvert.DeserializeObject<SomtodayStudentModel>(await response.Content.ReadAsStringAsync());
            return new user
            {
                somtoday_student_id = somtodayStudent.items[0].links[0].id.ToString(),
                somtoday_student_profile_picture = somtodayStudent.items[0].additionalObjects.pasfoto.datauri
            };
        }

        Random random = new Random();

        private string RandomStateString(int length = 8)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string result = "";
            for (int i = 0; i < length; i++)
                result += chars[random.Next(0, chars.Length)];


            return result;
        }

        #endregion
    }
}