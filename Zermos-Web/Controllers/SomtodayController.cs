using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using ChartJSCore.Helpers;
using ChartJSCore.Models;
using Infrastructure;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Zermos_Web.Models;
using Zermos_Web.Models.Requirements;
using Zermos_Web.Models.SomtodayAfwezigheidModel;
using Zermos_Web.Models.SomtodayGradesModel;
using Zermos_Web.Models.somtodayHomeworkModel;
using Zermos_Web.Utilities;
using Data = ChartJSCore.Models.Data;
using Item = Zermos_Web.Models.SomtodayGradesModel.Item;

namespace Zermos_Web.Controllers
{
    public class SomtodayController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SomtodayController> _logger;
        private readonly Users _users;

        public SomtodayController(ILogger<SomtodayController> logger, Users users)
        {
            _logger = logger;
            _users = users;
            _httpClient = new HttpClient();
        }

        #region Cijfers

        [Authorize]
        [SomtodayRequirement]
        [ZermosPage]
        [HttpGet("Somtoday/Cijfers")]
        public async Task<IActionResult> Cijfers()
        {
            ViewData["add_css"] = "somtoday";
            
            if (Request.Cookies.ContainsKey("cached-somtoday-grades"))
            {
                return PartialView(JsonConvert.DeserializeObject<SomtodayGradesModel>(_users.GetUserAsync(User.FindFirstValue("email")).Result.cached_somtoday_grades ?? string.Empty));
            }

            var user = await _users.GetUserAsync(User.FindFirstValue("email"));

            var access_token = user.somtoday_access_token;
            
            if (TokenUtils.CheckToken(user.somtoday_access_token) == false)
            {
                access_token = await RefreshToken(user.somtoday_refresh_token);
            }

            var grades = await fetchGrades(access_token, user.somtoday_student_id);
            
            await _users.UpdateUserAsync(User.FindFirstValue("email"), new user
            {
                cached_somtoday_grades = JsonConvert.SerializeObject(grades)
            });
            
            Response.Cookies.Append("cached-somtoday-grades", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), new CookieOptions {Expires = DateTime.Now.AddMinutes(10)});

            return PartialView(grades);
        }

        public async Task<string> RefreshToken(string token = null)
        {
            if (token == null) return null;

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
            
            if (response.IsSuccessStatusCode == false)
            {
                HttpContext.AddNotification("Oops, er is iets fout gegaan", "Je Somtoday refresh token lijkt verlopen te zijn, als dit probleem zich voor blijft doen, koppel Somtoday dan opnieuw", NotificationCenter.NotificationType.ERROR);
                return null;
            }
            
            var somtodayAuthentication =
                JsonConvert.DeserializeObject<SomtodayAuthenticatieModel>(response.Content.ReadAsStringAsync().Result);

            var user = new user
            {
                somtoday_access_token = somtodayAuthentication.access_token,
                somtoday_refresh_token = somtodayAuthentication.refresh_token
            };
            await _users.UpdateUserAsync(User.FindFirstValue("email"), user);
            return somtodayAuthentication.access_token;
        }

        [ZermosPage]
        [HttpGet("Somtoday/Cijfers/{vak}")]
        public async Task<IActionResult> Cijfer(string vak)
        {
            SomtodayGradesModel grades;
            
            if (Request.Cookies.ContainsKey("cached-somtoday-grades"))
            {
                grades = JsonConvert.DeserializeObject<SomtodayGradesModel>(_users.GetUserAsync(User.FindFirstValue("email")).Result.cached_somtoday_grades ?? string.Empty);
            }
            else
            {
                var user = await _users.GetUserAsync(User.FindFirstValue("email"));

                var access_token = user.somtoday_access_token;
            
                if (TokenUtils.CheckToken(user.somtoday_access_token) == false)
                {
                    access_token = await RefreshToken(user.somtoday_refresh_token);
                }

                grades = await fetchGrades(access_token, user.somtoday_student_id);
            
                await _users.UpdateUserAsync(User.FindFirstValue("email"), new user
                {
                    cached_somtoday_grades = JsonConvert.SerializeObject(grades)
                });
            
                Response.Cookies.Append("cached-somtoday-grades", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), new CookieOptions {Expires = DateTime.Now.AddMinutes(10)});
            }
            
            var sortedGrades = new List<sortedGrades>();
            foreach (var grade in grades.items)
            {
                var vakWithGrades = sortedGrades.Find(x => x.vak.naam == grade.vak.naam);
                if (vakWithGrades == null)
                {
                    vakWithGrades = new sortedGrades();
                    vakWithGrades.vak = grade.vak;
                    vakWithGrades.grades = new List<Item>();
                    sortedGrades.Add(vakWithGrades);
                }
                vakWithGrades.grades.Add(grade);
            }

            return PartialView(sortedGrades.Find(x => string.Equals(x.vak.afkorting, vak, StringComparison.CurrentCultureIgnoreCase)));
        }

        [ZermosPage]
        [HttpGet("Somtoday/Cijfers/{vak}/{id}")]
        public async Task<IActionResult> CijferData(string id)
        {
            SomtodayGradesModel grades;
            
            if (Request.Cookies.ContainsKey("cached-somtoday-grades"))
            {
                grades = JsonConvert.DeserializeObject<SomtodayGradesModel>(_users.GetUserAsync(User.FindFirstValue("email")).Result.cached_somtoday_grades ?? string.Empty);
            }
            else
            {
                var user = await _users.GetUserAsync(User.FindFirstValue("email"));

                var access_token = user.somtoday_access_token;
            
                if (TokenUtils.CheckToken(user.somtoday_access_token) == false)
                {
                    access_token = await RefreshToken(user.somtoday_refresh_token);
                }

                grades = await fetchGrades(access_token, user.somtoday_student_id);
            
                await _users.UpdateUserAsync(User.FindFirstValue("email"), new user
                {
                    cached_somtoday_grades = JsonConvert.SerializeObject(grades)
                });
            
                Response.Cookies.Append("cached-somtoday-grades", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), new CookieOptions {Expires = DateTime.Now.AddMinutes(10)});
            }

            return PartialView(grades.items.Find(x => x.links[0].id == id));
        }

        [ZermosPage]
        [HttpGet("Somtoday/Cijfers/{vak}/Statestieken")]
        public async Task<IActionResult> CijferStatestieken(string vak, bool asPFD = false)
        {
            SomtodayGradesModel somtodayGradesModel;
            
            if (Request.Cookies.ContainsKey("cached-somtoday-grades"))
            {
                somtodayGradesModel = JsonConvert.DeserializeObject<SomtodayGradesModel>(_users.GetUserAsync(User.FindFirstValue("email")).Result.cached_somtoday_grades ?? string.Empty);
            }
            else
            {
                var user = await _users.GetUserAsync(User.FindFirstValue("email"));

                var access_token = user.somtoday_access_token;
            
                if (TokenUtils.CheckToken(user.somtoday_access_token) == false)
                {
                    access_token = await RefreshToken(user.somtoday_refresh_token);
                }

                somtodayGradesModel = await fetchGrades(access_token, user.somtoday_student_id);
            
                await _users.UpdateUserAsync(User.FindFirstValue("email"), new user
                {
                    cached_somtoday_grades = JsonConvert.SerializeObject(somtodayGradesModel)
                });
            
                Response.Cookies.Append("cached-somtoday-grades", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), new CookieOptions {Expires = DateTime.Now.AddMinutes(10)});
            }
            
            var sortedGrades = new List<sortedGrades>();
            foreach (var grade in somtodayGradesModel.items)
            {
                var vakWithGrades = sortedGrades.Find(x => x.vak.naam == grade.vak.naam);
                if (vakWithGrades == null)
                {
                    vakWithGrades = new sortedGrades();
                    vakWithGrades.vak = grade.vak;
                    vakWithGrades.grades = new List<Item>();
                    sortedGrades.Add(vakWithGrades);
                }
                vakWithGrades.grades.Add(grade);
            }

            var grades = sortedGrades.Find(x => string.Equals(x.vak.afkorting, vak, StringComparison.CurrentCultureIgnoreCase));

            ViewData["stats"] = new Dictionary<string, string>();
            (ViewData["stats"] as Dictionary<string, string>)?.Add("vak", grades.vak.naam);
            (ViewData["stats"] as Dictionary<string, string>)?.Add("hoogste", grades.grades.Max(x => x.geldendResultaat).ToString());
            (ViewData["stats"] as Dictionary<string, string>)?.Add("laagste", grades.grades.Min(x => x.geldendResultaat).ToString());
            (ViewData["stats"] as Dictionary<string, string>)?.Add("weging", grades.grades.Sum(x => x.weging == 0 ? x.examenWeging : x.weging).ToString()); 
            // (ViewData["stats"] as Dictionary<string, string>)?.Add("som", the grades resultaat times it weight);
            (ViewData["stats"] as Dictionary<string, string>)?.Add("som", grades.grades.Sum(x => NumberUtils.ParseFloat(x.geldendResultaat) * x.weging == 0 ? x.examenWeging : x.weging).ToString("0.0000000000", System.Globalization.CultureInfo.InvariantCulture));

            var charts = new List<Chart>();
            charts.Add(GenerateGradeOverTimeAndGradeAverage(grades));
            charts.Add(GetVoldoendeOndervoldoendeRatio(grades));
            charts.Add(GetMostCommonGrade(grades));
            //charts.Add(GetStandardDeviationChart(grades));


            return PartialView(charts);
        }

        private Chart CreateChart(string title, bool showHeight = true)
        {
            var chart = new Chart
            {
                Options = new Options
                {
                    Scales = new Dictionary<string, Scale>
                    {
                        {
                            "y",
                            new CartesianLinearScale
                            {
                                BeginAtZero = true, Ticks = new CartesianLinearTick {StepSize = 1}, Display = showHeight
                            }
                        },
                        {"x", new Scale {Grid = new Grid {Offset = true}, Display = showHeight}}
                    },
                    Plugins = new Plugins
                    {
                        Legend = new Legend {Display = false}
                    }
                }
            };

            chart.Options.Layout = new Layout
            {
                Padding = new Padding
                {
                    PaddingObject = new PaddingObject
                    {
                        Left = 10,
                        Right = 12
                    }
                }
            };

            chart.Options.Plugins.Title = new ChartJSCore.Models.Title {Text = new List<string> {title}, Display = false};

            return chart;
        }

        private Chart GetMostCommonGrade(sortedGrades grades)
        {
            var chart = CreateChart("Meest voorkomende cijfer");
            chart.Type = Enums.ChartType.Bar;

            var data = new Data();
            data.Labels = new List<string>();

            var dataset = new BarDataset
            {
                Data = new List<double?>(),
                BackgroundColor = new List<ChartColor> {ChartColor.FromRgb(75, 192, 192)},
                BorderColor = new List<ChartColor> {ChartColor.FromRgb(75, 192, 192)},
                BorderWidth = new List<int> {1},
                BarPercentage = 0.5,
                BarThickness = 6,
                MaxBarThickness = 8,
                MinBarLength = 2
            };

            data.Datasets = new List<Dataset> {dataset};
            chart.Data = data;

            var doubleList = new List<double?> {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
            var stringList = new List<string> {"1", "2", "3", "4", "5", "6", "7", "8", "9", "10"};

            foreach (var grade in grades.grades)
            {
                var index = (int) Math.Round(NumberUtils.ParseFloat(grade.geldendResultaat), 0,
                    MidpointRounding.AwayFromZero) - 1;

                if (doubleList[index] == null)
                    doubleList[index] = 1;
                else
                    doubleList[index]++;
            }

            for (var i = 0; i < doubleList.Count; i++)
                if (doubleList[i] == 0)
                {
                    doubleList.RemoveAt(i);
                    stringList.RemoveAt(i);
                    i--;
                }

            dataset.Data = doubleList;
            data.Labels = stringList;

            return chart;
        }

        private Chart GetVoldoendeOndervoldoendeRatio(sortedGrades grades)
        {
            var chart = CreateChart("Voldoende/Ondervoldoende Ratio", false);
            chart.Type = Enums.ChartType.Pie;

            var data = new Data();
            data.Labels = new List<string> {"Voldoende", "Onvoldoende"};

            var dataset = new PieDataset
            {
                BackgroundColor = new List<ChartColor>
                    {ChartColor.FromHexString("#00ff00"), ChartColor.FromHexString("#ff0000")},
                HoverBackgroundColor = new List<ChartColor>
                    {ChartColor.FromHexString("#00ff00"), ChartColor.FromHexString("#ff0000")},
                Data = new List<double?>()
            };

            var voldoende = 0;
            var onvoldoende = 0;

            foreach (var grade in grades.grades)
                if (NumberUtils.ParseFloat(grade.geldendResultaat) >= 5.5)
                    voldoende++;
                else
                    onvoldoende++;

            dataset.Data.Add(voldoende);
            dataset.Data.Add(onvoldoende);

            data.Datasets = new List<Dataset> {dataset};
            chart.Data = data;

            return chart;
        }

        private Chart GenerateGradeOverTimeAndGradeAverage(sortedGrades grades)
        {
            var chart = CreateChart("Cijfer en gemiddelde over tijd");
            chart.Type = Enums.ChartType.Line;
            chart.Options.Scales["y"] = new Scale {Min = 0, Max = 10};

            var dataset = new LineDataset
            {
                Fill = "false",
                Data = new List<double?>(),
                Tension = 0.2,
                BackgroundColor = new List<ChartColor> {ChartColor.FromRgba(75, 192, 192, 0.4)},
                BorderColor = new List<ChartColor> {ChartColor.FromRgb(75, 192, 192)},
                BorderCapStyle = "butt",
                BorderDash = new List<int>(),
                BorderDashOffset = 0.0,
                BorderJoinStyle = "miter",
                PointBorderColor = new List<ChartColor> {ChartColor.FromRgb(75, 192, 192)},
                PointBackgroundColor = new List<ChartColor> {ChartColor.FromHexString("#ffffff")},
                PointBorderWidth = new List<int> {1},
                PointHoverRadius = new List<int> {5},
                PointHoverBackgroundColor = new List<ChartColor> {ChartColor.FromRgb(75, 192, 192)},
                PointHoverBorderColor = new List<ChartColor> {ChartColor.FromRgb(220, 220, 220)},
                PointHoverBorderWidth = new List<int> {2},
                PointRadius = new List<int> {1},
                PointHitRadius = new List<int> {10},
                SpanGaps = false
            };

            var data = new Data
            {
                Labels = new List<string>()
            };

            data.Datasets = new List<Dataset> {dataset};
            chart.Data = data;

            var gradesArray = new float[grades.grades.Count];
            var gradesWeight = new int[grades.grades.Count];

            foreach (var grade in grades.grades)
            {
                chart.Data.Datasets[0].Data.Add(NumberUtils.ParseFloat(grade.geldendResultaat));
                data.Labels.Add(grade.datumInvoer.ToString("dd-MM"));
                gradesArray[grades.grades.IndexOf(grade)] = NumberUtils.ParseFloat(grade.geldendResultaat);
                gradesWeight[grades.grades.IndexOf(grade)] = grade.weging == 0 ? grade.examenWeging : grade.weging;
            }

            chart.Data.Datasets.Add(new LineDataset
            {
                Fill = "false",
                Data = NumberUtils.CalculateWeightedAverageSnapshots(gradesArray, gradesWeight),
                Tension = 0.2,
                BackgroundColor = new List<ChartColor> {ChartColor.FromHexString("#F2542D")},
                BorderColor = new List<ChartColor> {ChartColor.FromHexString("#F2542D")},
                BorderCapStyle = "butt",
                BorderDash = new List<int>(),
                BorderDashOffset = 0.0,
                BorderJoinStyle = "miter",
                PointBorderColor = new List<ChartColor> {ChartColor.FromHexString("#F2542D")},
                PointBackgroundColor = new List<ChartColor> {ChartColor.FromHexString("#ffffff")},
                PointBorderWidth = new List<int> {1},
                PointHoverRadius = new List<int> {5},
                PointHoverBackgroundColor = new List<ChartColor> {ChartColor.FromHexString("#F2542D")},
                PointHoverBorderColor = new List<ChartColor> {ChartColor.FromRgb(220, 220, 220)},
                PointHoverBorderWidth = new List<int> {2},
                PointRadius = new List<int> {1},
                PointHitRadius = new List<int> {10},
                SpanGaps = false
            });

            (ViewData["stats"] as Dictionary<string, string>)?.Add("gemiddelde", chart.Data.Datasets[1].Data[^1]?.ToString("0.0000000000", System.Globalization.CultureInfo.InvariantCulture));

            return chart;
        }

        private Chart GetStandardDeviationChart(sortedGrades grades)
        {
            return null;
        }

        public SomtodayGradesModel Sort(SomtodayGradesModel grades)
        {
            if (grades == null) return new SomtodayGradesModel() {items = new List<Item>()};
            
            grades.items = grades.items.OrderBy(x => x.datumInvoer).ToList();

            foreach (var item in grades.items.Where(x => x.resultaatLabelAfkorting == "V"))
                item.geldendResultaat = "7";
            
            foreach (var item in grades.items.Where(x => x.resultaatLabelAfkorting == "G"))
                item.geldendResultaat = "8";


            grades.items = grades.items
                .Where(x => !(string.IsNullOrEmpty(x.omschrijving) && x.weging == 0))
                .Where(x => x.type != "DeeltoetsKolom")
                .Where(x => x.geldendResultaat != null)
                .ToList();

            return grades;
        }

        public async Task<SomtodayGradesModel> fetchGrades(string access_token, string somtoday_student_id)
        {
                        var baseUrl =
                $"https://api.somtoday.nl/rest/v1/resultaten/huidigVoorLeerling/{somtoday_student_id}?additional=samengesteldeToetskolomId";

            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + access_token);
            _httpClient.DefaultRequestHeaders.Add("Range", "items=0-99");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var response = await _httpClient.GetAsync(baseUrl);

            var grades =
                JsonConvert.DeserializeObject<SomtodayGradesModel>(await response.Content.ReadAsStringAsync());

            if (response.IsSuccessStatusCode == false)
            {
                HttpContext.AddNotification("Oops, er is iets fout gegaan", "Je cijfers konden niet worden opgehaald, mogelijk is je Somtoday token verlopen, als dit probleem zich blijft voordoen koppel dan je Somtoday account opnieuw", NotificationCenter.NotificationType.ERROR);
                return new SomtodayGradesModel {items = new List<Item>()};
            }

            if (int.TryParse(response.Content.Headers.GetValues("Content-Range").First().Split('/')[1],
                    out var total))
            {
                
                
                var requests = total / 100 * 100;

                for (var i = 100; i < requests; i += 100)
                {
                    _httpClient.DefaultRequestHeaders.Clear();
                    _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + access_token);
                    _httpClient.DefaultRequestHeaders.Add("Range", $"items={i}-{i + 99}");
                    _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                    response = await _httpClient.GetAsync(baseUrl);

                    var _grades =
                        JsonConvert.DeserializeObject<SomtodayGradesModel>(
                            await response.Content.ReadAsStringAsync());
                    grades.items.AddRange(_grades.items);
                }
            }

            return Sort(grades);
        }
        #endregion

        #region huiswerk
        [Authorize]
        [SomtodayRequirement]
        [ZermosPage]
        [HttpGet("/Somtoday/Huiswerk")]
        public async Task<IActionResult> Huiswerk(int dagen = 21, bool recache = false)
        {
            ViewData["add_css"] = "somtoday";
            
            if (Request.Cookies.ContainsKey("cached-somtoday-homework") && recache == false)
            {
                var cache = (await _users.GetUserAsync(User.FindFirstValue("email"))).cached_somtoday_homework;
                var homework = JsonConvert.DeserializeObject<SomtodayHomeworkModel>(cache);
                homework.items.AddRange(await GetRemappedCustomHuiswerk());
                return PartialView(Sort(homework));
            }

            var user = await _users.GetUserAsync(User.FindFirstValue("email"));

            var access_token = user.somtoday_access_token;
            
            if (TokenUtils.CheckToken(user.somtoday_access_token) == false)
            {
                access_token = await RefreshToken(user.somtoday_refresh_token);
            }

            var _startDate = DateTime.Now.AddDays(-dagen).ToString("yyyy-MM-dd");
            var baseurl =
                $"https://api.somtoday.nl/rest/v1/studiewijzeritemafspraaktoekenningen?begintNaOfOp={_startDate}&additional=swigemaaktVinkjes";

            var rangemin = 0;
            var rangemax = 99;


            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("authorization", "Bearer " + access_token);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _httpClient.DefaultRequestHeaders.Add("Range", $"items={rangemin}-{rangemax}");

            var response = await _httpClient.GetAsync(baseurl);
            
            if (response.IsSuccessStatusCode == false)
            {
                HttpContext.AddNotification("Oops, er is iets fout gegaan", "Je Huiswerk op Somtoday kon niet worden opgehaald, mogelijk is je Somtoday token verlopen, als dit probleem zich blijft voordoen koppel dan je Somtoday account opnieuw", NotificationCenter.NotificationType.ERROR);
                return PartialView(Sort(new SomtodayHomeworkModel() {items = await GetRemappedCustomHuiswerk()}));
            }

            var somtodayHuiswerk =
                JsonConvert.DeserializeObject<SomtodayHomeworkModel>(
                    await response.Content.ReadAsStringAsync());

            await _users.UpdateUserAsync(User.FindFirstValue("email"), new user
            {
                cached_somtoday_homework = JsonConvert.SerializeObject(somtodayHuiswerk)
            });
            
            somtodayHuiswerk.items.AddRange(await GetRemappedCustomHuiswerk());

            somtodayHuiswerk = Sort(somtodayHuiswerk);
            
            Response.Cookies.Append("cached-somtoday-homework", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), new CookieOptions {Expires = DateTime.Now.AddHours(1)});
            
            return PartialView(somtodayHuiswerk);
        }

        [NonAction]
        private async Task<List<Models.somtodayHomeworkModel.Item>> GetRemappedCustomHuiswerk()
        {
            var customHomeworkItems = JsonConvert.DeserializeObject<List<CustomHuiswerkModel>>((await _users.GetUserAsync(User.FindFirstValue("email"))).custom_huiswerk ?? "[]") ?? new List<CustomHuiswerkModel>();
            var remapedHomework = new List<Models.somtodayHomeworkModel.Item>(capacity:  customHomeworkItems.Count);

            foreach (var customHomeworkItem in customHomeworkItems)
            {
                var item = new Models.somtodayHomeworkModel.Item
                {
                    datumTijd = customHomeworkItem.deadline,
                    studiewijzerItem = new StudiewijzerItem
                    {
                        omschrijving = customHomeworkItem.omschrijving,
                        huiswerkType = "EIGEN",
                    },
                    lesgroep = new Lesgroep
                    {
                        vak = new Models.somtodayHomeworkModel.Vak
                        {
                            naam = customHomeworkItem.titel
                        }
                    },
                    additionalObjects = new Models.somtodayHomeworkModel.AdditionalObjects
                    {
                        swigemaaktVinkjes = new SwigemaaktVinkjes
                        {
                            items = new List<Models.somtodayHomeworkModel.Item>
                            {
                                new()
                                {
                                    gemaakt = customHomeworkItem.gemaakt
                                }
                            }
                        }
                    },
                    gemaakt = true,
                    UUID = customHomeworkItem.id.ToString()
                };

                remapedHomework.Add(item);
            }

            return remapedHomework;
        }


        [Authorize]
        [SomtodayRequirement]
        [ZermosPage]
        [HttpGet("Somtoday/Huiswerk/Nieuw")]
        public IActionResult NieuwHuiswerk()
        {
            ViewData["add_css"] = "somtoday";
            return PartialView();
        }
        
        [Authorize]
        [SomtodayRequirement]
        [HttpPost("Somtoday/Huiswerk/Nieuw")]
        public async Task<IActionResult> NieuwHuiswerkPOST()
        {
            //from form
            var title = Request.Form["title"];
            var description = Request.Form["description"];
            var date = DateTime.ParseExact(Request.Form["date"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
            
            
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description))
            {
                HttpContext.AddNotification("Bijna", "Je hebt of geen titel, of geen datum ingevult, beide velden zijn nodig om huiswerk aan te maken", NotificationCenter.NotificationType.INFO);
                return Forbid();
            }
            
            var userEmail = User.FindFirstValue("email");
            var user = await _users.GetUserAsync(userEmail);

            var homework = JsonConvert.DeserializeObject<List<CustomHuiswerkModel>>(user.custom_huiswerk ?? "[]") ?? new List<CustomHuiswerkModel>();

            homework.Add(new CustomHuiswerkModel(title, description, date, false, homework.Count + 1));

            user = new user
            {
                custom_huiswerk = JsonConvert.SerializeObject(homework)
            };

            await _users.UpdateUserAsync(userEmail, user);
            
            return Ok();
        }
        
        [Authorize]
        [SomtodayRequirement]
        [HttpDelete("Somtoday/Huiswerk/Nieuw")]
        public async Task<IActionResult> NieuwHuiswerk(int id)
        {
            var userEmail = User.FindFirstValue("email");
            var user = await _users.GetUserAsync(userEmail);
            
            var homework = JsonConvert.DeserializeObject<List<CustomHuiswerkModel>>(user.custom_huiswerk) ?? new List<CustomHuiswerkModel>();
            
            homework.RemoveAll(x => x.id == id);
            
            user.custom_huiswerk = JsonConvert.SerializeObject(homework);
            
            await _users.UpdateUserAsync(userEmail, user);
            
            return Ok();
        }

        public SomtodayHomeworkModel Sort(SomtodayHomeworkModel homework)
        {
            if (homework == null)
                return new SomtodayHomeworkModel {items = new List<Models.somtodayHomeworkModel.Item>()};

            homework.items = homework?.items?
                .Where(x => x.studiewijzerItem != null && x.studiewijzerItem.huiswerkType != "LESSTOF")
                .OrderBy(x => x.datumTijd)
                .ToList();

            return homework;
        }

        #endregion
        
        #region Afwezigheid
        [Authorize]
        [SomtodayRequirement]
        [ZermosPage]
        [HttpGet("/Account/Afwezigheid")]
        [HttpGet("/Somtoday/Afwezigheid")]
        public async Task<IActionResult> Afwezigheid()
        {
            if (Request.Cookies.ContainsKey("cached-somtoday-absence"))
            {
                return PartialView(JsonConvert.DeserializeObject<SomtodayAfwezigheidModel>(_users.GetUserAsync(User.FindFirstValue("email")).Result.cached_somtoday_absence ?? string.Empty));
            }

            SchooljaarUtils.Schooljaar currentSchoolyear = SchooljaarUtils.getCurrentSchooljaar();
            
            //https://api.somtoday.nl/rest/v1/waarnemingen?waarnemingSoort=Afwezig
            
            var user = await _users.GetUserAsync(User.FindFirstValue("email"));
            
            var access_token = user.somtoday_access_token;
            
            if (TokenUtils.CheckToken(user.somtoday_access_token) == false)
            {
                access_token = await RefreshToken(user.somtoday_refresh_token);
            }
            
            var baseurl = $"https://api.somtoday.nl/rest/v1/absentiemeldingen?begindatumtijd={currentSchoolyear.vanafDatumDate:yyyy-MM-dd}&einddatumtijd={currentSchoolyear.totDatumDate:yyyy-MM-dd}";
            
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("authorization", "Bearer " + access_token);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            
            var response = await _httpClient.GetAsync(baseurl);
            
            if (response.IsSuccessStatusCode == false)
            {
                HttpContext.AddNotification("Oops, er is iets fout gegaan", "Je Afwezigheid op Somtoday kon niet worden opgehaald, mogelijk is je Somtoday token verlopen, als dit probleem zich blijft voordoen koppel dan je Somtoday account opnieuw", NotificationCenter.NotificationType.ERROR);
                return PartialView(new SomtodayAfwezigheidModel {items = new List<Models.SomtodayAfwezigheidModel.Item>()});
            }
            
            var somtodayAfwezigheid =
                JsonConvert.DeserializeObject<SomtodayAfwezigheidModel>(
                    await response.Content.ReadAsStringAsync());
            
            if (currentSchoolyear.vanafDatumDate < DateTime.Now && DateTime.Now < currentSchoolyear.totDatumDate)
            {
                await _users.UpdateUserAsync(User.FindFirstValue("email"), new user
                {
                    cached_somtoday_absence = JsonConvert.SerializeObject(somtodayAfwezigheid)
                });
                
                Response.Cookies.Append("cached-somtoday-absence", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), new CookieOptions {Expires = DateTime.Now.AddHours(12)});

            }
            
            return PartialView(somtodayAfwezigheid);
        }
        #endregion
    }
}