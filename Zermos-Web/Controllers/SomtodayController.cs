using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
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
using Zermos_Web.APIs;
using Zermos_Web.Models;
using Zermos_Web.Models.Requirements;
using Zermos_Web.Models.SomtodayLeermiddelen;
using Zermos_Web.Models.SomtodayGradesModel;
using Zermos_Web.Models.somtodayHomeworkModel;
using Zermos_Web.Models.SomtodayPlaatsingen;
using Zermos_Web.Models.SomtodayVakgemiddeldenModel;
using Zermos_Web.Models.SortedSomtodayGradesModel;
using Zermos_Web.Utilities;
using Data = ChartJSCore.Models.Data;
using Item = Zermos_Web.Models.SomtodayGradesModel.Item;

namespace Zermos_Web.Controllers
{
    public class SomtodayController : BaseController
    {
        public SomtodayController(Users user, Shares share, ILogger<BaseController> logger) : base(user, share, logger) { }
        
        private readonly HttpClient _httpClient = new();
        SomtodayAPI somtodayApi = new(new HttpClient());
        
        #region Cijfers
        [Authorize]
        [ZermosPage]
        [SomtodayRequirement]
        [HttpGet("Somtoday/Cijfers")]
        public async Task<IActionResult> Cijfers(int leerjaar)
        {
            if (Request.Cookies.ContainsKey("cached-somtoday-grades"))
            {
                return PartialView(JsonConvert.DeserializeObject<SortedSomtodayGradesModel>(ZermosUser.cached_somtoday_grades ?? "{}"));
            }
            
            var user = ZermosUser;
            
            if (TokenUtils.CheckToken(user.somtoday_access_token) == false)
            {
                user.somtoday_access_token = await RefreshToken(user.somtoday_refresh_token);
            }
            
            SomtodayPlaatsingenModel plaatsingen = (user.cached_somtoday_plaatsingen == null) ? await somtodayApi.GetPlaatsingen(user) : JsonConvert.DeserializeObject<SomtodayPlaatsingenModel>(user.cached_somtoday_plaatsingen);
            
            var grades = await somtodayApi.GetCurrentGradesAndVakgemiddelden(user, plaatsingen);
            
            ZermosUser = new user
            {
                cached_somtoday_grades = JsonConvert.SerializeObject(grades),
                cached_somtoday_plaatsingen = JsonConvert.SerializeObject(plaatsingen)
            };
            
            Response.Cookies.Append("cached-somtoday-grades", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), new CookieOptions {Expires = DateTime.Now.AddMinutes(10)});

            return PartialView(grades);
        }

        //[ZermosPage]
        [HttpGet("Somtoday/Cijfers/{vak}")]
        public async Task<IActionResult> CijferVoorVak(string leerjaar, string vak)
        {
            if (Request.Cookies.ContainsKey("cached-somtoday-grades"))
            {
                var _grades = JsonConvert.DeserializeObject<SortedSomtodayGradesModel>(ZermosUser.cached_somtoday_grades ?? "{}");
                _grades.items = _grades.items.FindAll(x => x.vakAfkorting == vak);
                _grades.lastGrades = null;
                _grades.voortGangsdossierGemiddelde = null;
                return PartialView(_grades);
            }
            
            var user = ZermosUser;
            
            if (TokenUtils.CheckToken(user.somtoday_access_token) == false)
            {
                user.somtoday_access_token = await RefreshToken(user.somtoday_refresh_token);
            }
            
            SomtodayPlaatsingenModel plaatsingen = (user.cached_somtoday_plaatsingen == null) ? await somtodayApi.GetPlaatsingen(user) : JsonConvert.DeserializeObject<SomtodayPlaatsingenModel>(user.cached_somtoday_plaatsingen);
            
            var grades = await somtodayApi.GetCurrentGradesAndVakgemiddelden(user, plaatsingen);
            
            ZermosUser = new user
            {
                cached_somtoday_grades = JsonConvert.SerializeObject(grades),
                cached_somtoday_plaatsingen = JsonConvert.SerializeObject(plaatsingen)
            };
            
            Response.Cookies.Append("cached-somtoday-grades", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), new CookieOptions {Expires = DateTime.Now.AddMinutes(10)});

            grades.items = grades.items.FindAll(x => x.vakAfkorting == vak);
            grades.lastGrades = null;
            grades.voortGangsdossierGemiddelde = null;
            
            return PartialView(grades);
        }

        // [ZermosPage]
        // [HttpGet("Somtoday/Cijfers/{vak}/Statestieken")]
        // public async Task<IActionResult> CijferStatestieken(string vak, bool asPFD = false)
        // {
        //     SomtodayGradesModel somtodayGradesModel;
        //     
        //     if (Request.Cookies.ContainsKey("cached-somtoday-grades"))
        //     {
        //         somtodayGradesModel = JsonConvert.DeserializeObject<SomtodayGradesModel>(ZermosUser.cached_somtoday_grades ?? string.Empty);
        //     }
        //     else
        //     {
        //         var user = ZermosUser;
        //     
        //         if (TokenUtils.CheckToken(user.somtoday_access_token) == false)
        //         {
        //             user.somtoday_access_token = await RefreshToken(user.somtoday_refresh_token);
        //         }
        //
        //         somtodayGradesModel = await somtodayApi.GetGrades(user);
        //
        //         ZermosUser = new user
        //         {
        //             cached_somtoday_grades = JsonConvert.SerializeObject(somtodayGradesModel)
        //         };
        //     
        //         Response.Cookies.Append("cached-somtoday-grades", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), new CookieOptions {Expires = DateTime.Now.AddMinutes(10)});
        //     }
        //     
        //     var sortedGrades = new List<sortedGrades>();
        //     foreach (var grade in somtodayGradesModel.items)
        //     {
        //         var vakWithGrades = sortedGrades.Find(x => x.vak.naam == grade.vak.naam);
        //         if (vakWithGrades == null)
        //         {
        //             vakWithGrades = new sortedGrades();
        //             vakWithGrades.vak = grade.vak;
        //             vakWithGrades.grades = new List<Item>();
        //             sortedGrades.Add(vakWithGrades);
        //         }
        //         vakWithGrades.grades.Add(grade);
        //     }
        //
        //     var grades = sortedGrades.Find(x => string.Equals(x.vak.afkorting, vak, StringComparison.CurrentCultureIgnoreCase));
        //
        //     ViewData["stats"] = new Dictionary<string, string>();
        //     (ViewData["stats"] as Dictionary<string, string>)?.Add("hoogste", grades.grades.Max(x => x.geldendResultaat).ToString());
        //     (ViewData["stats"] as Dictionary<string, string>)?.Add("laagste", grades.grades.Min(x => x.geldendResultaat).ToString());
        //
        //     var som = 0f;
        //     var weging = 0;
        //     foreach (Item grade in grades.grades)
        //     {
        //         if (grade.isExamendossierResultaat || grade.type == "DeeltoetsKolom") continue;
        //         
        //         som += NumberUtils.ParseFloat(grade.geldendResultaat) * (grade.weging == 0 ? grade.examenWeging : grade.weging);
        //         weging += grade.weging == 0 ? grade.examenWeging : grade.weging;
        //     }
        //
        //     (ViewData["stats"] as Dictionary<string, string>)?.Add("som", som.ToString("0.0000000000", CultureInfo.InvariantCulture));
        //     (ViewData["stats"] as Dictionary<string, string>)?.Add("weging", weging.ToString());
        //     
        //     var charts = new List<Chart>();
        //     charts.Add(GetVoldoendeOndervoldoendeRatio(grades));
        //     charts.Add(GenerateGradeOverTimeAndGradeAverage(grades));
        //     charts.Add(GetMostCommonGrade(grades));
        //
        //     dynamic model = new
        //     {
        //         charts = charts,
        //         stats = ViewData["stats"] as Dictionary<string, string>,
        //         grades = grades
        //     };
        //     
        //     return PartialView(model);
        // }
        //
        // private Chart CreateChart(string title, bool showHeight = true)
        // {
        //     var chart = new Chart
        //     {
        //         Options = new Options
        //         {
        //             Scales = new Dictionary<string, Scale>
        //             {
        //                 {
        //                     "y",
        //                     new CartesianLinearScale
        //                     {
        //                         BeginAtZero = true, Ticks = new CartesianLinearTick {StepSize = 1}, Display = showHeight
        //                     }
        //                 },
        //                 {"x", new Scale {Grid = new Grid {Offset = true}, Display = showHeight}}
        //             },
        //             Plugins = new Plugins
        //             {
        //                 Legend = new Legend {Display = false}
        //             }
        //         }
        //     };
        //
        //     chart.Options.Layout = new Layout
        //     {
        //         Padding = new Padding
        //         {
        //             PaddingObject = new PaddingObject
        //             {
        //                 Left = 10,
        //                 Right = 12
        //             }
        //         }
        //     };
        //
        //     chart.Options.Plugins.Title = new ChartJSCore.Models.Title {Text = new List<string> {title}, Display = false};
        //
        //     return chart;
        // }
        //
        // private Chart GetMostCommonGrade(sortedGrades grades)
        // {
        //     var chart = CreateChart("Meest voorkomende cijfer");
        //     chart.Type = Enums.ChartType.Bar;
        //
        //     var data = new Data();
        //     data.Labels = new List<string>();
        //
        //     var dataset = new BarDataset
        //     {
        //         Data = new List<double?>(),
        //         BackgroundColor = new List<ChartColor> {ChartColor.FromRgb(75, 192, 192)},
        //         BorderColor = new List<ChartColor> {ChartColor.FromRgb(75, 192, 192)},
        //         BorderWidth = new List<int> {1},
        //         BarPercentage = 0.5,
        //         BarThickness = 6,
        //         MaxBarThickness = 8,
        //         MinBarLength = 2
        //     };
        //
        //     data.Datasets = new List<Dataset> {dataset};
        //     chart.Data = data;
        //
        //     var doubleList = new List<double?> {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        //     var stringList = new List<string> {"1", "2", "3", "4", "5", "6", "7", "8", "9", "10"};
        //
        //     foreach (var grade in grades.grades)
        //     {
        //         var index = (int) Math.Round(NumberUtils.ParseFloat(grade.geldendResultaat), 0,
        //             MidpointRounding.AwayFromZero) - 1;
        //
        //         if (doubleList[index] == null)
        //             doubleList[index] = 1;
        //         else
        //             doubleList[index]++;
        //     }
        //
        //     for (var i = 0; i < doubleList.Count; i++)
        //         if (doubleList[i] == 0)
        //         {
        //             doubleList.RemoveAt(i);
        //             stringList.RemoveAt(i);
        //             i--;
        //         }
        //
        //     dataset.Data = doubleList;
        //     data.Labels = stringList;
        //
        //     return chart;
        // }
        //
        // private Chart GetVoldoendeOndervoldoendeRatio(sortedGrades grades)
        // {
        //     var chart = CreateChart("Percentage voldoende", false);
        //     chart.Type = Enums.ChartType.Pie;
        //
        //     var data = new Data();
        //     data.Labels = new List<string> {"Voldoende", "Onvoldoende"};
        //
        //     var dataset = new PieDataset
        //     {
        //         BackgroundColor = new List<ChartColor>
        //             {ChartColor.FromHexString("#00ff00"), ChartColor.FromHexString("#ff0000")},
        //         HoverBackgroundColor = new List<ChartColor>
        //             {ChartColor.FromHexString("#00ff00"), ChartColor.FromHexString("#ff0000")},
        //         Data = new List<double?>()
        //     };
        //
        //     var voldoende = 0;
        //     var onvoldoende = 0;
        //
        //     foreach (var grade in grades.grades)
        //         if (NumberUtils.ParseFloat(grade.geldendResultaat) >= 5.5)
        //             voldoende++;
        //         else
        //             onvoldoende++;
        //
        //     dataset.Data.Add(voldoende);
        //     dataset.Data.Add(onvoldoende);
        //
        //     data.Datasets = new List<Dataset> {dataset};
        //     chart.Data = data;
        //
        //     (ViewData["stats"] as Dictionary<string, string>)?.Add("voldoendes", voldoende.ToString());
        //     (ViewData["stats"] as Dictionary<string, string>)?.Add("onvoldoendes", onvoldoende.ToString());
        //     
        //     return chart;
        // }
        //
        // private Chart GenerateGradeOverTimeAndGradeAverage(sortedGrades grades)
        // {
        //     var chart = CreateChart("cijfers en gemiddelde over tijd");
        //     chart.Type = Enums.ChartType.Line;
        //     chart.Options.Scales["y"] = new Scale {Min = 0, Max = 10};
        //
        //     var dataset = new LineDataset
        //     {
        //         Fill = "false",
        //         Data = new List<double?>(),
        //         Tension = 0.2,
        //         BackgroundColor = new List<ChartColor> {ChartColor.FromRgba(75, 192, 192, 0.4)},
        //         BorderColor = new List<ChartColor> {ChartColor.FromRgb(75, 192, 192)},
        //         BorderCapStyle = "butt",
        //         BorderDash = new List<int>(),
        //         BorderDashOffset = 0.0,
        //         BorderJoinStyle = "miter",
        //         PointBorderColor = new List<ChartColor> {ChartColor.FromRgb(75, 192, 192)},
        //         PointBackgroundColor = new List<ChartColor> {ChartColor.FromHexString("#ffffff")},
        //         PointBorderWidth = new List<int> {1},
        //         PointHoverRadius = new List<int> {5},
        //         PointHoverBackgroundColor = new List<ChartColor> {ChartColor.FromRgb(75, 192, 192)},
        //         PointHoverBorderColor = new List<ChartColor> {ChartColor.FromRgb(220, 220, 220)},
        //         PointHoverBorderWidth = new List<int> {2},
        //         PointRadius = new List<int> {1},
        //         PointHitRadius = new List<int> {10},
        //         SpanGaps = false
        //     };
        //
        //     var data = new Data
        //     {
        //         Labels = new List<string>()
        //     };
        //
        //     data.Datasets = new List<Dataset> {dataset};
        //     chart.Data = data;
        //
        //     var gradesArray = new float[grades.grades.Count];
        //     var gradesWeight = new int[grades.grades.Count];
        //
        //     foreach (var grade in grades.grades)
        //     {
        //         chart.Data.Datasets[0].Data.Add(NumberUtils.ParseFloat(grade.geldendResultaat));
        //         data.Labels.Add(grade.datumInvoer.ToString("dd-MM"));
        //         gradesArray[grades.grades.IndexOf(grade)] = NumberUtils.ParseFloat(grade.geldendResultaat);
        //         gradesWeight[grades.grades.IndexOf(grade)] = grade.weging == 0 ? grade.examenWeging : grade.weging;
        //     }
        //     
        //     chart.Data.Datasets.Add(new LineDataset
        //     {
        //         Fill = "false",
        //         Data = NumberUtils.CalculateStandardDeviationSnapshots(gradesArray, gradesWeight),
        //         Tension = 0.2,
        //         BackgroundColor = new List<ChartColor> {ChartColor.FromHexString("#F2542D")},
        //         BorderColor = new List<ChartColor> {ChartColor.FromHexString("#F2542D")},
        //         BorderCapStyle = "butt",
        //         BorderDash = new List<int>(),
        //         BorderDashOffset = 0.0,
        //         BorderJoinStyle = "miter",
        //         PointBorderColor = new List<ChartColor> {ChartColor.FromHexString("#F2542D")},
        //         PointBackgroundColor = new List<ChartColor> {ChartColor.FromHexString("#ffffff")},
        //         PointBorderWidth = new List<int> {1},
        //         PointHoverRadius = new List<int> {5},
        //         PointHoverBackgroundColor = new List<ChartColor> {ChartColor.FromHexString("#F2542D")},
        //         PointHoverBorderColor = new List<ChartColor> {ChartColor.FromRgb(220, 220, 220)},
        //         PointHoverBorderWidth = new List<int> {2},
        //         PointRadius = new List<int> {1},
        //         PointHitRadius = new List<int> {10},
        //         SpanGaps = false
        //     });
        //     
        //     (ViewData["stats"] as Dictionary<string, string>)?.Add("gemiddelde", chart.Data.Datasets[1].Data[^1]?.ToString("0.0000000000", CultureInfo.InvariantCulture));
        //
        //     
        //     return chart;
        // }
        //
        // [Authorize]
        // [HttpGet("/Somtoday/Cijfers/GenereerToken")]
        // [SomtodayRequirement]
        // public async Task<IActionResult> GenereerToken(string vakken, bool show_individual_grades, DateTime? expires_at, int max_uses = int.MaxValue)
        // {
        //     expires_at ??= DateTime.Now.AddDays(7);
        //     
        //     var user = ZermosUser;
        //     
        //     if (TokenUtils.CheckToken(user.somtoday_access_token) == false)
        //     {
        //        user.somtoday_access_token = await RefreshToken(user.somtoday_refresh_token);
        //     }
        //
        //     var grades = await somtodayApi.GetGrades(ZermosUser);
        //     
        //     ZermosUser = new user
        //     {
        //         cached_somtoday_grades = JsonConvert.SerializeObject(grades)
        //     };
        //     
        //     Response.Cookies.Append("cached-somtoday-grades", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), new CookieOptions {Expires = DateTime.Now.AddMinutes(10)});
        //
        //     string[] vakkenArray = vakken.Split(',');
        //     
        //     var sortedGrades = new List<sortedGrades>();
        //     
        //     foreach (var grade in grades.items)
        //     {
        //         var vakWithGrades = sortedGrades.Find(x => x.vak.naam == grade.vak.naam);
        //         if (vakWithGrades == null)
        //         {
        //             vakWithGrades = new sortedGrades();
        //             vakWithGrades.vak = grade.vak;
        //             vakWithGrades.grades = new List<Item>();
        //             sortedGrades.Add(vakWithGrades);
        //         }
        //         vakWithGrades.grades.Add(grade);
        //     }
        //     
        //     var gradesToShare = new List<Item>();
        //     foreach (var vak in vakkenArray)
        //     {
        //         var vakWithGrades = sortedGrades.Find(x => string.Equals(x.vak.afkorting, vak, StringComparison.CurrentCultureIgnoreCase));
        //         if (vakWithGrades == null) continue;
        //         gradesToShare.AddRange(vakWithGrades.grades);
        //     }
        //     
        //     share share = new share
        //     {
        //         key = TokenUtils.RandomString(20),
        //         email = ZermosEmail,
        //         value = (show_individual_grades ? 1 : 0) + gradesToShare.ObjectToBase64String(),
        //         page = "/Somtoday/Cijfers/Gedeeld",
        //         expires_at = (DateTime) expires_at,
        //         max_uses = max_uses
        //     };
        //     
        //     await AddShare(share);
        //     
        //     return Ok(share.url);
        // }
        //
        // [ZermosPage]
        // [HttpGet("/Somtoday/Cijfers/Gedeeld")]
        // public async Task<IActionResult> GedeeldeCijfers(string token)
        // {
        //     var grades = await GetShare(token);
        //     
        //     if (grades == null)
        //         return NotFound();
        //     
        //     if (grades.expires_at < DateTime.Now)
        //     {
        //         await DeleteShare(token);
        //         return NotFound();
        //     }
        //     
        //     dynamic model = new
        //     {
        //         grades = grades.value.Substring(1).Base64StringToObject<List<Item>>(),
        //         show_individual_grades = grades.value[0] == '1'
        //     };
        //     
        //     return PartialView(model);
        // }
        #endregion

        #region huiswerk
        [Authorize]
        [SomtodayRequirement]
        [ZermosPage]
        [HttpGet("/Somtoday/Huiswerk")]
        public async Task<IActionResult> Huiswerk(int dagen = 21)
        {
            ViewData["add_css"] = "somtoday";
            
            if (Request.Cookies.ContainsKey("cached-somtoday-homework"))
            {
                var cache = ZermosUser.cached_somtoday_homework;
                var homework = JsonConvert.DeserializeObject<SomtodayHomeworkModel>(cache);
                homework.items.AddRange(GetRemappedCustomHuiswerk());
                return PartialView(Sort(homework));
            }

            var user = ZermosUser;

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
                return PartialView(Sort(new SomtodayHomeworkModel() {items = GetRemappedCustomHuiswerk()}));
            }

            var somtodayHuiswerk =
                JsonConvert.DeserializeObject<SomtodayHomeworkModel>(
                    await response.Content.ReadAsStringAsync());
            
            somtodayHuiswerk.items.AddRange(await GetWeekAndDayHomework(access_token, dagen));

            ZermosUser = new user
            {
                cached_somtoday_homework = JsonConvert.SerializeObject(somtodayHuiswerk)
            };
            
            somtodayHuiswerk.items.AddRange(GetRemappedCustomHuiswerk());

            somtodayHuiswerk = Sort(somtodayHuiswerk);
            
            Response.Cookies.Append("cached-somtoday-homework", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), new CookieOptions {Expires = DateTime.Now.AddHours(1)});
            
            return PartialView(somtodayHuiswerk);
        }

        private async Task<List<Models.somtodayHomeworkModel.Item>> GetWeekAndDayHomework(string access_token, int dagen)
        {
            
            var _startDate = DateTime.Now.AddDays(-dagen).ToString("yyyy-MM-dd");
            var baseurl = $"https://api.somtoday.nl/rest/v1/studiewijzeritemdagtoekenningen?schooljaar=&begintNaOfOp={_startDate}&additional=swigemaaktVinkjes";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("authorization", "Bearer " + access_token);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _httpClient.DefaultRequestHeaders.Add("Range", $"items=0-99");
            
            var response = await _httpClient.GetAsync(baseurl);
            
            if (response.IsSuccessStatusCode == false)
            {
                HttpContext.AddNotification("Oops, er is iets fout gegaan", "Je \"dag huiswerk\" op somtoday kon niet opgevraagd worden", NotificationCenter.NotificationType.WARNING);
                return new List<Models.somtodayHomeworkModel.Item>();
            }
            
            var somtodayHuiswerk = JsonConvert.DeserializeObject<SomtodayHomeworkModel>(await response.Content.ReadAsStringAsync());

            baseurl = $"https://api.somtoday.nl/rest/v1/studiewijzeritemweektoekenningen?schooljaar=&begintNaOfOp={_startDate}&additional=swigemaaktVinkjes";
            
            response = await _httpClient.GetAsync(baseurl);
            
            if (response.IsSuccessStatusCode == false)
            {
                HttpContext.AddNotification("Oops, er is iets fout gegaan", "Je \"week huiswerk\" op somtoday kon niet opgevraagd worden, je \"dag huiswerk\" is wel opgevraagd", NotificationCenter.NotificationType.WARNING);
                return somtodayHuiswerk.items;
            }
            
            somtodayHuiswerk.items.AddRange(JsonConvert.DeserializeObject<SomtodayHomeworkModel>(await response.Content.ReadAsStringAsync()).items);
            
            return somtodayHuiswerk.items;
        }

        [NonAction]
        private List<Models.somtodayHomeworkModel.Item> GetRemappedCustomHuiswerk()
        {
            var customHomeworkItems = JsonConvert.DeserializeObject<List<CustomHuiswerkModel>>(ZermosUser.custom_huiswerk ?? "[]") ?? new List<CustomHuiswerkModel>();
            var remapedHomework = new List<Models.somtodayHomeworkModel.Item>(capacity:  customHomeworkItems.Count);

            foreach (var customHomeworkItem in customHomeworkItems)
            {
                var item = new Models.somtodayHomeworkModel.Item
                {
                    DatumTijd = customHomeworkItem.deadline,
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
        [HttpPost("Somtoday/Huiswerk/Nieuw")]
        public IActionResult NieuwHuiswerkPOST([FromBody] CustomHuiswerkModel customHuiswerkModel)
        {
            if (customHuiswerkModel == null)
                return BadRequest();
            
            var homework = JsonConvert.DeserializeObject<List<CustomHuiswerkModel>>(ZermosUser.custom_huiswerk ?? "[]") ?? new List<CustomHuiswerkModel>();

            customHuiswerkModel.id = homework.Count + 1;
            customHuiswerkModel.gemaakt = false;
            
            homework.Add(customHuiswerkModel);
            
            ZermosUser = new user
            {
                custom_huiswerk = JsonConvert.SerializeObject(homework)
            };
            
            return Ok();
        }
        
        [Authorize]
        [SomtodayRequirement]
        [HttpDelete("Somtoday/Huiswerk/Nieuw")]
        public IActionResult NieuwHuiswerk(int id)
        {
            var user = ZermosUser;
            
            var homework = JsonConvert.DeserializeObject<List<CustomHuiswerkModel>>(user.custom_huiswerk) ?? new List<CustomHuiswerkModel>();
            
            homework.RemoveAll(x => x.id == id);
            
            user.custom_huiswerk = JsonConvert.SerializeObject(homework);
            
            ZermosUser = user;
            
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
        [HttpGet("/Account/Afwezigheid")]
        [HttpGet("/Somtoday/Afwezigheid")]
        public async Task<IActionResult> Afwezigheid()
        {
            var user = ZermosUser;
            
            if (TokenUtils.CheckToken(user.somtoday_access_token) == false)
                user.somtoday_access_token = await RefreshToken(user.somtoday_refresh_token);
            var somtodayAfwezigheid = await somtodayApi.GetAfwezigheidAsync(user);
            if (somtodayAfwezigheid == null) return NoContent();
            return Json(somtodayAfwezigheid);
        }
        #endregion
        
        #region leermiddelen
        [Authorize]
        [ZermosPage]
        [SomtodayRequirement]
        [HttpGet("/Somtoday/Leermiddelen")]
        public async Task<IActionResult> Leermiddelen()
        {
            if (Request.Cookies.ContainsKey("cached-somtoday-leermiddelen") && DateTime.TryParse(Request.Cookies["cached-somtoday-leermiddelen"], out var date) && date.AddDays(60) > DateTime.Now)
            {
                var _somtodayLeermiddelen = JsonConvert.DeserializeObject<SomtodayLeermiddelenModel>(ZermosUser.cached_somtoday_leermiddelen ?? "{\"items\": []}");
                var _customLeermiddelen = JsonConvert.DeserializeObject<SomtodayLeermiddelenModel>(ZermosUser.custom_leermiddelen ?? "{\"items\": []}");
                _somtodayLeermiddelen.items.AddRange(_customLeermiddelen.items);
                return PartialView(_somtodayLeermiddelen);
            }
            
            var user = ZermosUser;
            
            if (TokenUtils.CheckToken(user.somtoday_access_token) == false)
                user.somtoday_access_token = await RefreshToken(user.somtoday_refresh_token);

            var somtodayLeermiddelen = await somtodayApi.GetStudiemateriaal(user);
            var customLeermiddelen = JsonConvert.DeserializeObject<SomtodayLeermiddelenModel>(user.custom_leermiddelen ?? "{\"items\": []}");
            somtodayLeermiddelen.items.AddRange(customLeermiddelen.items);
            
            if (somtodayLeermiddelen.items.Count == 0)
                return NoContent();
            
            string json = JsonConvert.SerializeObject(somtodayLeermiddelen);
            
            ZermosUser = new user
            {
                cached_somtoday_leermiddelen = json
            };
            
            Response.Cookies.Append("cached-somtoday-leermiddelen", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), new CookieOptions {Expires = DateTime.Now.AddMinutes(15)});
            
            return PartialView(somtodayLeermiddelen);
        }
        
        [Authorize]
        [SomtodayRequirement]
        [HttpPost("/Somtoday/Leermiddelen")]
        public IActionResult LeermiddelenPost(string url, string title, string methode, string uitgever)
        {
            var leermiddelen = JsonConvert.DeserializeObject<SomtodayLeermiddelenModel>(ZermosUser.custom_leermiddelen ?? "{\"items\": []}");
            
            leermiddelen.items.Add(new Models.SomtodayLeermiddelen.Item
            {
                product = new Product
                {
                    title = title,
                    url = url,
                    methodeInformatie = new MethodeInformatie
                    {
                        dashboardMethodeNaam = title,
                        methode = methode,
                        uitgever = uitgever
                    }
                },
                isCustom = true
            });
            
            ZermosUser = new user
            {
                custom_leermiddelen = JsonConvert.SerializeObject(leermiddelen)
            };
            
            return Ok(leermiddelen);
        }
        #endregion
        
        #region refresh token
        public async Task<string> RefreshToken(string token = null)
        {
            if (token == null) return null;
            
            var somtoday = await somtodayApi.RefreshTokenAsync(token);
            
            ZermosUser = new user
            {
                somtoday_access_token = somtoday.access_token,
                somtoday_refresh_token = somtoday.refresh_token
            };
            
            return somtoday.access_token;
        }
        #endregion
    }
}