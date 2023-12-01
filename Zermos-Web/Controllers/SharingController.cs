using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Zermos_Web.Controllers;

public class SharingController : BaseController
{
    public SharingController(Users user, Shares share, ILogger<BaseController> logger) : base(user, share, logger) { }
    
    // [HttpGet]
    // public IActionResult Index()
    // {
    //     //return View();
    // }
    //
    // [HttpPost]
    // public IActionResult Index(string url, params object[] values)
    // {
    //     //return View();
    // }
    
}
