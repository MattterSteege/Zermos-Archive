using System.Security.Claims;
using System.Threading.Tasks;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
        public async Task<IActionResult> ShowAccount()
        {
            return View(await _users.GetUserAsync(User.FindFirstValue("email")));
        }
        //https://demos.creative-tim.com/soft-ui-dashboard-tailwind/pages/profile.html
    }
}