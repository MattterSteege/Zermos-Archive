using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zermos_Web.Models.Requirements;
using Zermos_Web.Utilities;

namespace Zermos_Web.Controllers
{
    [Route("[action]")]
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

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
        
        [ZermosPage]
        [HttpGet("/NotImplemented")]
        public IActionResult NotImplemented()
        {
            return PartialView();
        }
    }

    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}