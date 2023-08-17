using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Infrastructure;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Zermos_Web.Models;
using Zermos_Web.Models.Requirements;
using Zermos_Web.Utilities;

namespace Zermos_Web.Controllers
{
    public class SchoolController : Controller
    {
        private readonly ILogger<SchoolController> _logger;
        private readonly Users _users;

        public SchoolController(ILogger<SchoolController> logger, Users users)
        {
            _logger = logger;
            _users = users;
        }

        [ZermosPage]
        public async Task<IActionResult> InformatieBoord()
        {
            ViewData["add_css"] = "school";

            if (Request.Cookies.ContainsKey("cached-school-informationscreen"))
            {
                var lastModified = DateTime.Parse(Request.Cookies["cached-school-informationscreen"] ?? string.Empty);
                
                if (lastModified.Hour >= 0 && lastModified.Hour < 12 && DateTime.Now.Hour >= 0 &&
                    DateTime.Now.Hour < 12 || lastModified.Hour >= 12 && lastModified.Hour < 24 &&
                    DateTime.Now.Hour >= 12 && DateTime.Now.Hour < 24)
                {

                    return PartialView(JsonConvert.DeserializeObject<List<InformatieBoordModel>>((await _users.GetUserAsync(User.FindFirstValue("email"))).cached_school_informationscreen));
                }
            }

            var model = new List<InformatieBoordModel>();

            using var httpClient = new HttpClient();
            var baseUrl = "https://www.carmelcollegegouda.nl/vestigingen/antoniuscollege-gouda/infoscherm";
            var response = await httpClient.GetStringAsync(baseUrl);

            var doc = new HtmlDocument();
            doc.LoadHtml(response);
            var elements = doc.DocumentNode.SelectNodes("//div[contains(@class, 'swiper-slide')]");

            if (elements != null)
            {
                foreach (var element in elements)
                {
                    var title = element.SelectSingleNode(".//h1[contains(@class, 'text-black')]")?.InnerText ?? "";

                    if (title == "Weerbericht Gouda")
                        continue;

                    var subTitle = element.SelectSingleNode(".//h2[contains(@class, 'text-black')]")?.InnerText ??
                                   "";
                    var image = element.SelectSingleNode(".//img")?.Attributes["src"]?.Value ?? "";

                    var contentNodes = //div content text-black
                        element.SelectNodes(".//div[contains(@class, 'content')]//div[contains(@class, 'text-black')]");
                    var contentText = "";
                    if (contentNodes != null)
                        foreach (var contentNode in contentNodes)
                            contentText += contentNode.InnerText;

                    contentText = HTMLUtils.ReplaceHtmlEntities(contentText);

                    //if image is not a full url, add the base url
                    if (!image.StartsWith("http")) image = "https://www.carmelcollegegouda.nl" + image;

                    model.Add(new InformatieBoordModel(title, subTitle, image, contentText));
                }
            }
            
            await _users.UpdateUserAsync(User.FindFirstValue("email"), new user
            {
                cached_school_informationscreen = JsonConvert.SerializeObject(model)
            });

            
            Response.Cookies.Append("cached-school-informationscreen", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), new CookieOptions {Expires = DateTime.Now.AddDays(60)});

            return PartialView(model);
        }
    }
}