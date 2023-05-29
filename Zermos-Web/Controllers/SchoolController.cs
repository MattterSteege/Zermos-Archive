using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zermos_Web.Models;

namespace Zermos_Web.Controllers
{
    public class SchoolController : Controller
    {
        private readonly ILogger<SchoolController> _logger;

        public SchoolController(ILogger<SchoolController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> InformatieBoord()
        {
            ViewData["add_css"] = "school";
            
            List<InformatieBoordModel> model = new List<InformatieBoordModel>();
            
            using var httpClient = new HttpClient();
            string baseUrl = "https://www.carmelcollegegouda.nl/vestigingen/antoniuscollege-gouda/infoscherm";
            var response = await httpClient.GetStringAsync(baseUrl);

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(response);
            var elements = doc.DocumentNode.SelectNodes("//div[contains(@class, 'swiper-slide')]");

            if (elements != null)
            {
                foreach (var element in elements)
                {
                    string title = element.SelectSingleNode(".//h1[contains(@class, 'text-black')]")?.InnerText ?? "";
                    string subTitle = element.SelectSingleNode(".//h2[contains(@class, 'text-black')]")?.InnerText ?? "";
                    string image = "https://www.carmelcollegegouda.nl" + element.SelectSingleNode(".//img")?.Attributes["src"]?.Value ?? "";

                    var contentNodes = element.SelectNodes(".//div[contains(@class, 'content')]");
                    string contentText = "";
                    if (contentNodes != null)
                    {
                        foreach (var contentNode in contentNodes)
                        {
                            contentText += contentNode.InnerText;
                        }
                    }
                    
                    contentText = Utilities.HTMLUtils.ReplaceHtmlEntities(contentText);

                    model.Add(new InformatieBoordModel(title, subTitle, image, contentText));
                }
            }

            return View(model);
        }
    }
}