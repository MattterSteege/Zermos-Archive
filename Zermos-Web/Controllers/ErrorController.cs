using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zermos_Web.Models.Requirements;

namespace Zermos_Web.Controllers
{
    [Route("[action]")]
    public class ErrorController : BaseController
    {
        public ErrorController(Users user, Shares share, ILogger<BaseController> logger) : base(user, share, logger) { }

        [ZermosPage]
        [HttpGet("/Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return PartialView(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
        
        [ZermosPage]
        [HttpGet("/404")]
        public IActionResult FourZeroFour()
        {
            return PartialView("404");
        }
        
        [HttpPost("/report/bug")]
        public async Task<IActionResult> ReportBug([FromBody] BugReport report)
        {
            if (report == null)
            {
                return BadRequest();
            }
            
            //turn origin into URL object
            var origin = new Uri(Request.Headers["Origin"]);
            report.ReportedBy = ZermosEmail;
            
            dynamic embed = new 
            {
                embeds = new[] 
                {
                    new 
                    {
                        title = "",
                        description = report.Description,
                        color = 3092790,
                        footer = new 
                        {
                            text = ""
                        },
                        author = new 
                        {
                            name = origin.Authority,
                            url = origin.AbsoluteUri
                        },
                        fields = new object[] 
                        {
                            new 
                            {
                                name = "Bug ID",
                                value = report.BugId,
                                inline = true
                            },
                            new 
                            {
                                name = "Reported By",
                                value = report.ReportedBy,
                                inline = true
                            },
                            new 
                            {
                                name = "Reported On",
                                value = report.ReportedAt.ToString("dd MMMM yyyy HH:mm:ss"),
                                inline = true
                            },
                            new 
                            {
                                name = "Enviroment",
                                value = report.Enviroment,
                                inline = false
                            },
                            new 
                            {
                                name = "Opslag info",
                                value = report.OpslagInfo,
                                inline = false
                            },
                            new 
                            {
                                name = "Console Logs",
                                value = report.ConsoleLog,
                                inline = false
                            }
                        },
                        url = origin.AbsoluteUri
                    }
                },
                content = ""
            };
            
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://discord.com/api/webhooks/1271384242517053491/e5AFmZvHIKbryPW1cvnKRpqbqLGiaBkKADOakGcr4jhSkMt1ZUXQioX4hZ4J1pF6c3Jt");
            var json = System.Text.Json.JsonSerializer.Serialize(embed);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            
            // Send report to bug tracker
            return Ok(response.StatusCode);
        }
        
        [HttpPost("/report/idea")]
        public async Task<IActionResult> ReportIdea([FromBody] BugReport report)
        {
            if (report == null)
            {
                return BadRequest();
            }
            
            //turn origin into URL object
            var origin = new Uri(Request.Headers["Origin"]);
            report.ReportedBy = ZermosEmail;
            
            dynamic embed = new 
            {
                embeds = new[] 
                {
                    new 
                    {
                        title = "",
                        description = report.Description,
                        color = 3092790,
                        footer = new 
                        {
                            text = ""
                        },
                        author = new 
                        {
                            name = origin.Authority,
                            url = origin.AbsoluteUri
                        },
                        fields = new object[] 
                        {
                            new 
                            {
                                name = "Idea ID",
                                value = report.BugId,
                                inline = true
                            },
                            new 
                            {
                                name = "Reported By",
                                value = report.ReportedBy,
                                inline = true
                            },
                            new 
                            {
                                name = "Reported On",
                                value = report.ReportedAt.ToString("dd MMMM yyyy HH:mm:ss"),
                                inline = true
                            }
                        },
                        url = origin.AbsoluteUri
                    }
                },
                content = ""
            };
            
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://discord.com/api/webhooks/1271384153002213377/-Bt4dtPEY8fe3LLREQpPRKj46R_mZcD0kR1OU1rxWAq8cjjIo00JgwBb0kYkcgEk53VB");
            var json = System.Text.Json.JsonSerializer.Serialize(embed);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            
            // Send report to bug tracker
            return Ok(response.StatusCode);
        }
    }
    
    

    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
    
    public class BugReport
    {
        public string BugId => Guid.NewGuid().ToString();
        public string ReportedBy { get; set; }
        public DateTime ReportedAt => DateTime.Now;
        public string Enviroment { get; set; }
        public string OpslagInfo { get; set; }
        public string ConsoleLog { get; set; }
        public string NetworkLog { get; set; }
        public string Description { get; set; }
    }
}