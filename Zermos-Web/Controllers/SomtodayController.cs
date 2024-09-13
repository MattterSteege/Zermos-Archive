using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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
using Zermos_Web.Models.Somtoday;
using Zermos_Web.Models.SomtodayAfwezigheidModel;
using Zermos_Web.Models.SomtodayLeermiddelen;
using Zermos_Web.Models.somtodayHomeworkModel;
using Zermos_Web.Models.SomtodayPlaatsingen;
using Zermos_Web.Models.SortedSomtodayGradesModel;
using Zermos_Web.Models.zermelo;
using Zermos_Web.Utilities;

namespace Zermos_Web.Controllers
{
    public class SomtodayController : BaseController
    {
        public SomtodayController(Users user, Shares share, CustomAppointments customCustomAppointment, ILogger<BaseController> logger) : base(user, share, customCustomAppointment, logger) { }
        
        private readonly HttpClient _httpClient = new();
        SomtodayAPI somtodayApi = new(new HttpClient());
        
        #region Cijfers
        [Authorize]
        [ZermosPage]
        [SomtodayRequirement]
        [HttpGet("Somtoday/Cijfers")]
        public async Task<IActionResult> Cijfers(string leerjaar = "0")
        {
            if (Request.Cookies.ContainsKey("cached-somtoday-grades") && leerjaar == "0")
            {
                return PartialView(JsonConvert.DeserializeObject<SortedSomtodayGradesModel>(ZermosUser.cached_somtoday_grades ?? "{}"));
            }
            
            var user = ZermosUser;
            
            if (TokenUtils.CheckToken(user.somtoday_access_token) == false)
            {
                string access_token = await RefreshToken(user.somtoday_refresh_token);
                if (access_token == "re-auth")
                    return RedirectToAction("SomtodayNietGekoppeld", "Koppelingen");
                
                user.somtoday_access_token = access_token;
            }

            SomtodayPlaatsingenModel plaatsingen;
            
            if (Request.Cookies.ContainsKey("cached-somtoday-plaatsingen"))
            {
                plaatsingen = JsonConvert.DeserializeObject<SomtodayPlaatsingenModel>(ZermosUser.cached_somtoday_plaatsingen ?? "{}"); //object reference is not set to an instance of an object
                if (plaatsingen == null || plaatsingen.items == null || plaatsingen.items.Count == 0)
                {
                    plaatsingen = await somtodayApi.GetPlaatsingen(user);
                    Response.Cookies.Append("cached-somtoday-plaatsingen", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), new CookieOptions {Expires = DateTime.Now.AddMonths(1)});
                }
            }
            else
            {
                plaatsingen = await somtodayApi.GetPlaatsingen(user);
                Response.Cookies.Append("cached-somtoday-plaatsingen", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), new CookieOptions {Expires = DateTime.Now.AddMonths(1)});
            }

            SortedSomtodayGradesModel grades = new();
            
            if (leerjaar == "0" || leerjaar == plaatsingen.items.FirstOrDefault(x => x.huidig)!?.stamgroepnaam)
                grades = await somtodayApi.GetCurrentGradesAndVakgemiddelden(user, plaatsingen);
            else
                grades = await somtodayApi.GetGradesAndVakgemiddelden(user, plaatsingen, leerjaar);
            
            if (leerjaar == "0")
            {
                ZermosUser = new user
                {
                    cached_somtoday_grades = JsonConvert.SerializeObject(grades),
                    cached_somtoday_plaatsingen = JsonConvert.SerializeObject(plaatsingen)
                };
                
                Response.Cookies.Append("cached-somtoday-grades", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), new CookieOptions {Expires = DateTime.Now.AddMinutes(10)});
            }

            return PartialView(grades);
        }

        [ZermosPage]
        [HttpGet("Somtoday/Cijfers/{vak}/{vakUUID}/{lichtingUUID}/{plaatsingUuid}")]
        public async Task<IActionResult> CijferVoorVak(string vakUUID, string lichtingUUID, string plaatsingUuid)
        {
            vakUUID = vakUUID.simpleDecodeUUID();
            lichtingUUID = lichtingUUID.simpleDecodeUUID();
            plaatsingUuid = plaatsingUuid.simpleDecodeUUID();
            
            var user = ZermosUser;
            
            if (TokenUtils.CheckToken(user.somtoday_access_token) == false)
            {
                string access_token = await RefreshToken(user.somtoday_refresh_token);
                if (access_token == "re-auth")
                    return RedirectToAction("SomtodayNietGekoppeld", "Koppelingen");
                
                user.somtoday_access_token = access_token;
            }

            SomtodayPlaatsingenModel plaatsingen;
            
            if (Request.Cookies.ContainsKey("cached-somtoday-plaatsingen"))
            {
                plaatsingen = JsonConvert.DeserializeObject<SomtodayPlaatsingenModel>(ZermosUser.cached_somtoday_plaatsingen ?? "{}");
                if (plaatsingen.items.Count == 0)
                {
                    plaatsingen = await somtodayApi.GetPlaatsingen(user);
                    Response.Cookies.Append("cached-somtoday-plaatsingen", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), new CookieOptions {Expires = DateTime.Now.AddMonths(1)});
                }
            }
            else
            {
                plaatsingen = await somtodayApi.GetPlaatsingen(user);
                Response.Cookies.Append("cached-somtoday-plaatsingen", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), new CookieOptions {Expires = DateTime.Now.AddMonths(1)});
            }

            SortedSomtodayGradesModel grades;

            if (vakUUID == null || lichtingUUID == null || plaatsingUuid == null)
            {
                grades = await somtodayApi.GetCurrentGradesAndVakgemiddelden(user, plaatsingen);
                ZermosUser = new user
                {
                    cached_somtoday_grades = JsonConvert.SerializeObject(grades),
                    cached_somtoday_plaatsingen = JsonConvert.SerializeObject(plaatsingen)
                };
            }
            else
            {
                grades = await somtodayApi.GetGradesFromUUID(user, vakUUID, lichtingUUID, plaatsingUuid);
                ZermosUser = new user
                {
                    cached_somtoday_plaatsingen = JsonConvert.SerializeObject(plaatsingen)
                };
            }

            return PartialView(grades);
        }

        [ZermosPage]
        [HttpGet("Somtoday/Cijfers/{vak}/{vakUUID}/{lichtingUUID}/{plaatsingUuid}/Statistieken")]
        public async Task<IActionResult> CijferStatistieken(string vakUUID, string lichtingUUID, string plaatsingUuid)
        {
            vakUUID = vakUUID.simpleDecodeUUID();
            lichtingUUID = lichtingUUID.simpleDecodeUUID();
            plaatsingUuid = plaatsingUuid.simpleDecodeUUID();
            
            var user = ZermosUser;
            
            if (TokenUtils.CheckToken(user.somtoday_access_token) == false)
            {
                string access_token = await RefreshToken(user.somtoday_refresh_token);
                if (access_token == "re-auth")
                    return RedirectToAction("SomtodayNietGekoppeld", "Koppelingen");
                
                user.somtoday_access_token = access_token;
            }

            SomtodayPlaatsingenModel plaatsingen;
            
            if (Request.Cookies.ContainsKey("cached-somtoday-plaatsingen"))
            {
                plaatsingen = JsonConvert.DeserializeObject<SomtodayPlaatsingenModel>(ZermosUser.cached_somtoday_plaatsingen ?? "{}");
                if (plaatsingen.items.Count == 0)
                {
                    plaatsingen = await somtodayApi.GetPlaatsingen(user);
                    Response.Cookies.Append("cached-somtoday-plaatsingen", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), new CookieOptions {Expires = DateTime.Now.AddMonths(1)});
                }
            }
            else
            {
                plaatsingen = await somtodayApi.GetPlaatsingen(user);
                Response.Cookies.Append("cached-somtoday-plaatsingen", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), new CookieOptions {Expires = DateTime.Now.AddMonths(1)});
            }

            SortedSomtodayGradesModel somtodayGradesModel;

            if (vakUUID == null || lichtingUUID == null || plaatsingUuid == null)
            {
                somtodayGradesModel = await somtodayApi.GetCurrentGradesAndVakgemiddelden(user, plaatsingen);
                ZermosUser = new user
                {
                    cached_somtoday_grades = JsonConvert.SerializeObject(somtodayGradesModel),
                    cached_somtoday_plaatsingen = JsonConvert.SerializeObject(plaatsingen)
                };
            }
            else
            {
                somtodayGradesModel = await somtodayApi.GetGradesFromUUID(user, vakUUID, lichtingUUID, plaatsingUuid);
                ZermosUser = new user
                {
                    cached_somtoday_plaatsingen = JsonConvert.SerializeObject(plaatsingen)
                };
            }
            
            //calc the stats:
            //circle chart: voldoendes/onvoldoendes
            var allgrades = somtodayGradesModel.items[0].cijfers;
            allgrades.AddRange(somtodayGradesModel.items[0].cijfersSE);
            allgrades = allgrades.FindAll(x => x.isCijfer);
            
            int voldoendes = 0;
            int onvoldoendes = 0;
            int weging = 0;
            double som = 0;
            int wegingSE = 0;
            double somSE = 0;
            foreach (var item in allgrades)
            {
                if (item.isLabel) continue;
                
                if (item.cijfer >= 5.45)
                    voldoendes++;
                else
                    onvoldoendes++;
                
                if (item.isVoortgang)
                {
                    weging += item.weging;
                    som += item.cijfer * item.weging;
                }
                else
                {
                    wegingSE += item.weging;
                    somSE += item.cijfer * item.weging;
                }
            }
            
            //bar chart: most common grade
            var grades = new List<int> {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
            foreach (var item in allgrades)
            {
                if (item.isLabel) continue;
                
                var grade = (int) Math.Round(item.cijfer, 0, MidpointRounding.AwayFromZero) - 1;
                if (grade < 0)
                    grade = 0;
                if (grade > 9)
                    grade = 9;
                grades[grade]++;
            }
            
            double highest = 0;
            double lowest = 10;
            foreach (var grade in allgrades)
            {
                if (grade.isLabel) continue;
                
                if (grade.cijfer > highest)
                    highest = grade.cijfer;
                if (grade.cijfer < lowest)
                    lowest = grade.cijfer;
            }
            
            //line chart: grades over time
            
            somtodayGradesModel.items[0].cijfers = somtodayGradesModel.items[0].cijfers.FindAll(x => x.isVoortgang && x.isCijfer);
            somtodayGradesModel.items[0].cijfersSE = somtodayGradesModel.items[0].cijfersSE.FindAll(x => x.isVoortgang && x.isCijfer);
            
            SomtodayStatistiekenModel model = new()
            {
                item = somtodayGradesModel.items[0],
                voldoendes = voldoendes,
                onvoldoendes = onvoldoendes,
                mostCommonGrade = grades,
                highest = highest,
                lowest = lowest,
                weging = weging,
                som = som,
                wegingSE = wegingSE,
                somSE = somSE,
                containsSE = somtodayGradesModel.items[0].cijfersSE.Count > 0,
                containsVoortgang = somtodayGradesModel.items[0].cijfers.Count > 0,
            };
            
            
            return PartialView(model);
        }
        
        [Authorize]
        [SomtodayRequirement]
        [HttpGet("Somtoday/Cijfers/genereer-token")]
        public async Task<IActionResult> GenereerGradeToken(string leerjaar = "0", DateTime? expires_at = null, int max_uses = int.MaxValue)
        {
            expires_at ??= DateTime.Now.AddDays(7);
            
            var user = ZermosUser;
            
            if (TokenUtils.CheckToken(user.somtoday_access_token) == false)
            {
                string access_token = await RefreshToken(user.somtoday_refresh_token);
                if (access_token == "re-auth")
                    return RedirectToAction("SomtodayNietGekoppeld", "Koppelingen");
                
                user.somtoday_access_token = access_token;
            }

            SomtodayPlaatsingenModel plaatsingen;
            
            if (Request.Cookies.ContainsKey("cached-somtoday-plaatsingen"))
            {
                plaatsingen = JsonConvert.DeserializeObject<SomtodayPlaatsingenModel>(ZermosUser.cached_somtoday_plaatsingen ?? "{}");
                if (plaatsingen.items.Count == 0)
                {
                    plaatsingen = await somtodayApi.GetPlaatsingen(user);
                    Response.Cookies.Append("cached-somtoday-plaatsingen", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), new CookieOptions {Expires = DateTime.Now.AddMonths(1)});
                }
            }
            else
            {
                plaatsingen = await somtodayApi.GetPlaatsingen(user);
                Response.Cookies.Append("cached-somtoday-plaatsingen", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), new CookieOptions {Expires = DateTime.Now.AddMonths(1)});
            }

            SortedSomtodayGradesModel grades = new();
            
            if (leerjaar == "0" || leerjaar == plaatsingen.items.FirstOrDefault(x => x.huidig)!?.stamgroepnaam)
                grades = await somtodayApi.GetCurrentGradesAndVakgemiddelden(user, plaatsingen);
            else
                grades = await somtodayApi.GetGradesAndVakgemiddelden(user, plaatsingen, leerjaar);

            string url = (await AddShare(new()
            {
                key = TokenUtils.RandomString(20),
                email = ZermosEmail,
                value = grades.ObjectToBase64String(),
                page = "/Somtoday/Cijfers/Gedeeld",
                expires_at = (DateTime) expires_at,
                max_uses = max_uses
            })).key;
            
            return Ok(url);
        }
        
        [ZermosPage]
        [HttpGet("/Somtoday/Cijfers/Gedeeld")]
        public async Task<IActionResult> GedeeldCijfers(string token)
        {
            var cijfers = await GetShare(token);
            
            if (cijfers == null)
                return RedirectToAction("Verlopen", "Error");
            
            if (cijfers.expires_at < DateTime.Now)
            {
                await DeleteShare(token);
                return RedirectToAction("Verlopen", "Error");
            }
            
            return PartialView(cijfers.value.Base64StringToObject<SortedSomtodayGradesModel>());
        }
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
            
            if (TokenUtils.CheckToken(user.somtoday_access_token) == false)
            {
                string access_token = await RefreshToken(user.somtoday_refresh_token);
                if (access_token == "re-auth")
                    return RedirectToAction("SomtodayNietGekoppeld", "Koppelingen");
                
                user.somtoday_access_token = access_token;
            }

            var somtodayHomework = await somtodayApi.GetHomeworkAsync(user, dagen);
            
            somtodayHomework ??= new SomtodayHomeworkModel {items = new List<Models.somtodayHomeworkModel.Item>()};
            
            ZermosUser = new user
            {
                cached_somtoday_homework = JsonConvert.SerializeObject(somtodayHomework)
            };
            
            somtodayHomework.items.AddRange(GetRemappedCustomHuiswerk());
            
            Response.Cookies.Append("cached-somtoday-homework", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), new CookieOptions {Expires = DateTime.Now.AddMinutes(15)});
            
            return PartialView(Sort(somtodayHomework));
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
        public IActionResult NieuwHuiswerkPOST()
        {
            var customHuiswerkModel = new CustomHuiswerkModel
            {
                deadline = DateTime.Parse(Request.Form["deadline"]),
                titel = Request.Form["titel"],
                omschrijving = Request.Form["omschrijving"]
            };
            
            if (customHuiswerkModel.titel.IsNullOrEmpty() || customHuiswerkModel.omschrijving.IsNullOrEmpty() || customHuiswerkModel.deadline == DateTime.MinValue || customHuiswerkModel.deadline < DateTime.Now)
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
            {
                string access_token = await RefreshToken(user.somtoday_refresh_token);
                if (access_token == "re-auth")
                {
                    SomtodayAfwezigheidModel model = ModelUtils.ReturnEmptyModel<SomtodayAfwezigheidModel>();
                    return Json(model);
                }
                user.somtoday_access_token = access_token;
            }
            
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
            {
                string access_token = await RefreshToken(user.somtoday_refresh_token);
                if (access_token == "re-auth")
                    return RedirectToAction("SomtodayNietGekoppeld", "Koppelingen");
                
                user.somtoday_access_token = access_token;
            }

            var somtodayLeermiddelen = await somtodayApi.GetStudiemateriaal(user);
            var customLeermiddelen = JsonConvert.DeserializeObject<SomtodayLeermiddelenModel>(user.custom_leermiddelen ?? "{\"items\": []}") ?? new SomtodayLeermiddelenModel { items = new List<Models.SomtodayLeermiddelen.Item>() };
            somtodayLeermiddelen.items.AddRange(customLeermiddelen.items ?? new List<Models.SomtodayLeermiddelen.Item>());
            
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

        #region rooster
        [Authorize]
        [ZermosPage]
        [SomtodayRequirement]
        [HttpGet("/Somtoday/Rooster")]
        public async Task<IActionResult> Rooster(string year, string week, bool compact = false)
        {
            year ??= DateTime.Now.Year.ToString();
            week ??= DateTime.Now.GetWeekNumber().ToString();
            week = week.ToCharArray().Length == 1 ? "0" + week : week;
            
            var user = ZermosUser;
            
            if (TokenUtils.CheckToken(user.somtoday_access_token) == false)
            {
                string access_token = await RefreshToken(user.somtoday_refresh_token);
                if (access_token == "re-auth")
                    return RedirectToAction("SomtodayNietGekoppeld", "Koppelingen");
                
                user.somtoday_access_token = access_token;
            }
            
            var somtodayRooster = await somtodayApi.GetRoosterAsync(user, year, week);
            ZermeloRoosterModel zermeloRooster = somtodayRooster.TransformToZermeloRoosterModel();
            
            ZermosUser = new user();
            
            //use the /Zermelo/Rooster-week.cshtml view and /Zermelo/Rooster PartialViews
            return compact ? PartialView("~/Views/Zermelo/Rooster-week.cshtml", zermeloRooster) 
                          : PartialView("~/Views/Zermelo/Rooster.cshtml", zermeloRooster);
        }
        
        [Authorize]
        [SomtodayRequirement]
        [Route("/Somtoday/Rooster/Genereer-Token")]
        public async Task<IActionResult> GenereerToken(string year, string week, DateTime? expires_at, int max_uses = int.MaxValue)
        {
            if (string.IsNullOrEmpty(year) || string.IsNullOrEmpty(week))
                return BadRequest("Year or week is null or empty");
            
            expires_at ??= DateTime.Now.AddDays(7);
            week = week.ToCharArray().Length == 1 ? "0" + week : week;
            
            var somtodayRooster = await somtodayApi.GetRoosterAsync(ZermosUser, year, week);
            ZermeloRoosterModel zermeloRooster = somtodayRooster.TransformToZermeloRoosterModel();
            
            string url = (await AddShare(new share
            {
                key = TokenUtils.RandomString(20),
                email = ZermosEmail,
                value = zermeloRooster.ObjectToBase64String(),
                page = "/Somtoday/Rooster/Gedeeld",
                expires_at = (DateTime) expires_at,
                max_uses = max_uses
            })).key;
            
            return Ok(url);
        }
        
        [ZermosPage]
        [HttpGet("/Somtoday/Rooster/Gedeeld")]
        public async Task<IActionResult> GedeeldRooster(string token)
        {
            var rooster = await GetShare(token);
            
            if (rooster == null)
                return RedirectToAction("Verlopen", "Error");
            
            if (rooster.expires_at < DateTime.Now)
            {
                await DeleteShare(token);
                return RedirectToAction("Verlopen", "Error");
            }
            
            return PartialView("~/Views/Zermelo/GedeeldRooster.cshtml", rooster.value.Base64StringToObject<ZermeloRoosterModel>());
        }
        #endregion
        
        #region refresh token
        public async Task<string> RefreshToken(string token = null)
        {
            if (token == null) return null;
            
            var somtoday = await somtodayApi.RefreshTokenAsync(token);

            if (somtoday == null)
            {
                Console.WriteLine(" - triggered by: " + ZermosEmail);
                ZermosUser = new user
                {
                    somtoday_access_token = string.Empty,
                    somtoday_refresh_token = string.Empty,
                    somtoday_student_id = string.Empty
                };
                return "re-auth";
            }
            
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