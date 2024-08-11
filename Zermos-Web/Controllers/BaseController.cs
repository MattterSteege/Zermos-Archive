using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Zermos_Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly Users _user;
        private readonly Shares _share;
        private readonly ILogger<BaseController> _logger;
        private readonly GlobalVariables _globalVariables;

        protected BaseController(Users user, Shares share, ILogger<BaseController> logger, GlobalVariables globalVariables = null)
        {
            _user = user;
            _share = share;
            _logger = logger;
            _globalVariables = globalVariables;
        }

        // USER METHODS
        protected string ZermosEmail => User.FindFirstValue("email");
        
        protected user ZermosUser
        {
            get => _user.GetUserAsync(ZermosEmail).Result;
            set => _user.UpdateUserAsync(ZermosEmail, value).Wait();
        }
        
        protected string CurrentZermosVersion => Environment.GetEnvironmentVariable("ZERMOS-WEB-VERSION");
        
        // LOGGING METHODS
        protected void Log(LogLevel logLevel, string message)
        {
            _logger.Log(logLevel, message);
        }
        
        // GLOBAL VARIABLES METHODS
        protected GlobalVariables GlobalVariables => _globalVariables;
        
        // SHARE METHODS
        protected async Task<share> AddShare(share share)
        {
            await _share.AddShareAsync(share);
            return share;
        }
        
        protected async Task<share> GetShare(string key)
        {
            return await _share.GetShareAsync(key);
        }
        
        protected async Task<List<share>> GetShares()
        {
            return await _share.GetSharesAsync(ZermosEmail);
        }
        
        protected async Task DeleteShare(string key)
        {
            await _share.DeleteShareAsync(key);
        }
    }
}