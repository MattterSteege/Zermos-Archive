using System.Security.Claims;
using System.Threading.Tasks;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        [AddLoadingScreen("account laden...")]
        public async Task<IActionResult> ShowAccount()
        {
            return PartialView(await _users.GetUserAsync(User.FindFirstValue("email")));
        }
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateTheme(string newTheme)
        {
            var userToUpdate = await _users.GetUserAsync(HttpContext.User.FindFirstValue("email"));
            userToUpdate.theme = newTheme ?? "light";

            HttpContext.Response.Cookies.Append("theme", newTheme ?? "light");

            await _users.UpdateUserAsync(HttpContext.User.FindFirstValue("email"), userToUpdate);

            return RedirectToAction("ShowAccount");
        }
        //https://demos.creative-tim.com/soft-ui-dashboard-tailwind/pages/profile.html
    }
}