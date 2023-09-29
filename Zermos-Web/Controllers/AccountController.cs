using System.Security.Claims;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Zermos_Web.Models;
using Zermos_Web.Models.Requirements;

namespace Zermos_Web.Controllers
{
    [Route("account/[action]")]
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

        [Authorize]
        [ZermosPage]
        [Route("/Account")]
        public async Task<IActionResult> Account()
        {
            return PartialView(await _users.GetUserAsync(User.FindFirstValue("email")));
        }
        
        
        [Authorize]
        [ZermosPage]
        [Route("/Account/Instellingen")]
        public async Task<IActionResult> Settings()
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

            return RedirectToAction("Account");
        }
        //https://demos.creative-tim.com/soft-ui-dashboard-tailwind/pages/profile.html
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateDefaultPage(string newDefaultPage)
        {
            var userToUpdate = await _users.GetUserAsync(HttpContext.User.FindFirstValue("email"));
            userToUpdate.default_page = newDefaultPage ?? "/Zermelo/Rooster";

            HttpContext.Response.Cookies.Append("default_page", newDefaultPage ?? "/Zermelo/Rooster");

            await _users.UpdateUserAsync(HttpContext.User.FindFirstValue("email"), userToUpdate);

            return RedirectToAction("Account");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateSetting(string key, string value)
        {
            //only if the property which is being updated is a marked with the 'SettingAttribute'
            var userToUpdate = await _users.GetUserAsync(HttpContext.User.FindFirstValue("email"));
            var property = userToUpdate.GetType().GetProperty(key);
            if (property == null) return BadRequest("Property not found");
            var attribute = property.GetCustomAttributes(typeof(SettingAttribute), false);
            if (attribute.Length == 0) return BadRequest("You may not alter this property");
            property.SetValue(userToUpdate, value);
            await _users.UpdateUserAsync(HttpContext.User.FindFirstValue("email"), userToUpdate);
            return Ok("200");
        }
    }
}