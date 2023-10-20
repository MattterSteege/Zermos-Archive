using System.Security.Claims;
using Infrastructure;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Zermos_Web.Controllers
{
    public class BaseController : Controller
    {
        private readonly Users _user;
        private readonly ILogger<BaseController> _logger;

        public BaseController(Users user, ILogger<BaseController> logger)
        {
            _user = user;
            _logger = logger;
        }

        public string email => User.FindFirstValue("email");
        public user user
        {
            get => _user.GetUserAsync(email).Result;
            set => _user.UpdateUserAsync(email, value).Wait();
        }
        public void Log(LogLevel logLevel, string message)
        {
            _logger.Log(logLevel, message);
        }
    }
}