using System.Security.Claims;
using System.Threading.Tasks;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
        
        [HttpGet("@me")]
        public async Task<IActionResult> ShowAccount()
        {
            var user = await _users.GetUserAsync(User.FindFirstValue("email"));
            
            var userString = "";
            foreach (var property in user.GetType().GetProperties())
            {
                userString += $"{property.Name}: {property.GetValue(user)}\n";
            }
            
            return Content(userString);
        }
    }
}