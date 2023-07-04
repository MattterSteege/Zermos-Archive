using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ChartJSCore.Helpers;
using ChartJSCore.Models;
using Infrastructure;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Zermos_Web.Models;
using Zermos_Web.Models.Requirements;
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
        [AddLoadingScreen("Cijfers worden opgehaald")]
        public async Task<IActionResult> Cijfers(bool refresh_token = false)
        {
            ViewData["add_css"] = "somtoday";
           
            var user = await _users.GetUserAsync(User.FindFirstValue("email"));

            if (TokenUtils.CheckToken(user.somtoday_access_token) == false)
                await RefreshToken(user.somtoday_refresh_token);

            var baseUrl =
                $"https://api.somtoday.nl/rest/v1/resultaten/huidigVoorLeerling/{user.somtoday_student_id}?begintNaOfOp={DateTime.Now:yyyy}-01-01";

            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + user.somtoday_access_token);
            _httpClient.DefaultRequestHeaders.Add("Range", "items=0-99");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var response = await _httpClient.GetAsync(baseUrl);

            var grades =
                JsonConvert.DeserializeObject<SomtodayGradesModel>(await response.Content.ReadAsStringAsync());

            if (response.IsSuccessStatusCode == false)
                return NotFound(
                    "Er is iets fout gegaan bij het ophalen van de cijfers, het is mogelijk dat je SOMtoday token verlopen is, probeer de pagina opnieuw te openen, als het dan nog.");

            if (int.TryParse(response.Content.Headers.GetValues("Content-Range").First().Split('/')[1],
                    out var total))
            {
                var requests = total / 100 * 100;

                for (var i = 100; i < requests; i += 100)
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

            var user = new user
            {
                somtoday_access_token = somtodayAuthentication.access_token,
                somtoday_refresh_token = somtodayAuthentication.refresh_token
            };
            await _users.UpdateUserAsync(User.FindFirstValue("email"), user);
        }

        [AddLoadingScreen("Cijfers worden geladen")]
        public IActionResult Cijfer(string content = null)
        {
            ViewData["add_css"] = "somtoday";

            var a = Convert.FromBase64String(content ?? "");
            var b = Encoding.UTF8.GetString(a);
            var c = JsonConvert.DeserializeObject<sortedGrades>(b);

            return View(c);
        }

        [AddLoadingScreen("Cijfers worden geladen")]
        public IActionResult CijferData(string content = null)
        {
            ViewData["add_css"] = "somtoday";

            var a = Convert.FromBase64String(content ?? "");
            var b = Encoding.UTF8.GetString(a);
            var c = JsonConvert.DeserializeObject<Item>(b);

            return View(c);
        }

        [AddLoadingScreen("Cijfers worden geladen")]
        public IActionResult CijferStatestieken(string content = null, bool asPFD = false)
        {
            if (asPFD) return View("_Loading");

            ViewData["add_css"] = "somtoday";

            var a = Convert.FromBase64String(content ?? "");
            var b = Encoding.UTF8.GetString(a);
            var grades = JsonConvert.DeserializeObject<sortedGrades>(b);

            ViewData["stats"] = new Dictionary<string, string>();
            (ViewData["stats"] as Dictionary<string, string>)?.Add("vak",
                grades.vak.naam);
            (ViewData["stats"] as Dictionary<string, string>)?.Add("hoogste",
                grades.grades.Max(x => x.geldendResultaat).ToString());
            (ViewData["stats"] as Dictionary<string, string>)?.Add("laagste",
                grades.grades.Min(x => x.geldendResultaat).ToString());


            var charts = new List<Chart>();
            charts.Add(GenerateGradeOverTimeAndGradeAverage(grades));
            charts.Add(GetVoldoendeOndervoldoendeRatio(grades));
            charts.Add(GetMostCommonGrade(grades));
            //charts.Add(GetStandardDeviationChart(grades));


            return View(charts);
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
                gradesWeight[grades.grades.IndexOf(grade)] = grade.weging;
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

            (ViewData["stats"] as Dictionary<string, string>)?.Add("gemiddelde",
                chart.Data.Datasets[1].Data[^1]?.ToString("0.000"));

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


            grades.items = grades.items
                .Where(x => !(string.IsNullOrEmpty(x.omschrijving) && x.weging == 0))
                .Where(x => x.type != "SamengesteldeToetsKolom")
                .Where(x => x.geldendResultaat != null)
                .ToList();

            return grades;
        }

        #endregion

        #region huiswerk

        [Authorize]
        [SomtodayRequirement]
        [AddLoadingScreen("Huiswerk wordt opgehaald")]
        public async Task<IActionResult> Huiswerk(int dagen, bool refresh_token = false)
        {
            ViewData["add_css"] = "somtoday";

            var user = await _users.GetUserAsync(User.FindFirstValue("email"));

            dagen = dagen == 0 ? 14 : dagen;
            dagen = dagen > 50 ? 50 : dagen;

            if (TokenUtils.CheckToken(user.somtoday_access_token) == false)
                await RefreshToken(user.somtoday_refresh_token);

            var _startDate = DateTime.Now.AddDays(-dagen).ToString("yyyy-MM-dd");
            var baseurl =
                $"https://api.somtoday.nl/rest/v1/studiewijzeritemafspraaktoekenningen?begintNaOfOp={_startDate}&additional=swigemaaktVinkjes";

            var rangemin = 0;
            var rangemax = 99;


            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("authorization", "Bearer " + user.somtoday_access_token);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _httpClient.DefaultRequestHeaders.Add("Range", $"items={rangemin}-{rangemax}");

            var response = await _httpClient.GetAsync(baseurl);

            var somtodayHuiswerk =
                JsonConvert.DeserializeObject<somtodayHomeworkModel>(
                    await response.Content.ReadAsStringAsync());

            return View(Sort(somtodayHuiswerk));
        }


        public somtodayHomeworkModel Sort(somtodayHomeworkModel homework)
        {
            if (homework == null)
                return new somtodayHomeworkModel {items = new List<Models.somtodayHomeworkModel.Item>()};

            homework.items = homework?.items?
                .Where(x => x.studiewijzerItem != null && x.studiewijzerItem.huiswerkType != "LESSTOF")
                .OrderBy(x => x.datumTijd)
                .ToList();

            return homework;
        }

        #endregion
    }
}