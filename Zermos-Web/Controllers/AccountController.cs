using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Zermos_Web.Models.Requirements;

namespace Zermos_Web.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<AccountController> _logger;
        private readonly Users _users;

        public AccountController(ILogger<AccountController> logger, IConfiguration config, Users users)
        {
            _logger = logger;
            _config = config;
            _users = users;
        }
        
        [HttpGet]
        [AddLoadingScreen("account laden...")]
        public async Task<IActionResult> ShowAccount()
        {
            ViewData["add_css"] = "account";
            return View(await _users.GetUserAsync(User.FindFirstValue("email")));
        }
        //https://demos.creative-tim.com/soft-ui-dashboard-tailwind/pages/profile.html
    }
}