using System.Diagnostics;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zermos_Web.Models.Requirements;

namespace Zermos_Web.Controllers
{
    [Route("[action]")]
    public class ErrorController : BaseController
    {
        public ErrorController(Users user, ILogger<BaseController> logger) : base(user, logger) { }

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
    }

    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}