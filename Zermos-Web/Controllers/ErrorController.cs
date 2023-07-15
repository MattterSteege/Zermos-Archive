using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zermos_Web.Utilities;

namespace Zermos_Web.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
        
        [HttpGet("/error/404")]
        public IActionResult FourZeroFour()
        {
            HttpContext.AddNotification("da's pech, pagina weg", "De pagina die je zocht bestaat niet meer, of is verplaatst.", "error");
            HttpContext.AddNotification("opgelet", "Deze pagina is niet meer beschikbaar, of is verplaatst.", "warning");
            HttpContext.AddNotification("even voor jouw infomatie", "Deze pagina is niet meer beschikbaar, of is verplaatst.", "info");
            return View("404");
        }
        
        public IActionResult NotImplemented()
        {
            return View();
        }
    }

    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}


/*

When you want to load a page with ajax, you can use this snippet:
It will return a partial view when the request was by ajax, and a loading view when the request was by a legitimate user.
Make sure you remove the 'Layout = "_Layout";' line from the cshtml file, otherwise it will load the whole page, not the content.

public ActionResult LoadPage()
{
    //the request was by ajax, so return the partial view
    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
    {
        return PartialView();
    }

    //the request was by a legitimate user, so return the loading view
    ViewData["url"] = "/" + ControllerContext.RouteData.Values["controller"] + "/" +
                      ControllerContext.RouteData.Values["action"];
    return View("_Loading");
}


*/