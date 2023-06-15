using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
            
            //the request was by ajax, so return the partial view
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
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

                        if (title == "Weerbericht Gouda")
                            continue;
                        
                        string subTitle = element.SelectSingleNode(".//h2[contains(@class, 'text-black')]")?.InnerText ?? "";
                        string image = element.SelectSingleNode(".//img")?.Attributes["src"]?.Value ?? "";

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
                        
                        //if image is not a full url, add the base url
                        if (!image.StartsWith("http"))
                        {
                            image = "https://www.carmelcollegegouda.nl" + image;
                        }

                        model.Add(new InformatieBoordModel(title, subTitle, image, contentText));
                    }
                }

                return View(model);
            }
            
            ViewData["laad_tekst"] = "Nieuws word van de het informatieboord gehaald";
            //the request was by a legitimate user, so return the loading view
            ViewData["url"] = "/" + ControllerContext.RouteData.Values["controller"] + "/" + ControllerContext.RouteData.Values["action"];
            return View("_Loading");
        }

        public async Task<IActionResult> Message(string content)
        {
            ViewData["add_css"] = "school";
            
            //the request was by ajax, so return the partial view
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                //turn the model into a json string turn it into bytes and encode with base64 but in reverse
                return View(JsonConvert.DeserializeObject<InformatieBoordModel>(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(content))));
            }
            
            //the request was by a legitimate user, so return the loading view
            ViewData["url"] = "/" + ControllerContext.RouteData.Values["controller"] + "/" + ControllerContext.RouteData.Values["action"] + "?content=" + content;
            return View("_Loading");
        }
    }
}