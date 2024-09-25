using System;
using System.Collections.Generic;
using System.Net.Http;
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
        protected readonly CustomAppointments CustomCustomAppointment;
        private readonly ILogger<BaseController> _logger;
        protected readonly IHttpClientFactory _clientFactory;

        protected BaseController(Users user, Shares share, CustomAppointments customCustomAppointment,
            ILogger<BaseController> logger, IHttpClientFactory httpClientFactory)
        {
            _user = user;
            _share = share;
            CustomCustomAppointment = customCustomAppointment;
            _logger = logger;
        }

        // USER METHODS
        protected string ZermosEmail => User.FindFirstValue("email");
        
        protected user ZermosUser
        {
            get
            {
                try
                {
                    var user = _user.GetUserAsync(ZermosEmail).Result;
                    return user;
                }
                catch (Exception e)
                {
                    Log(LogLevel.Error, e.Message + "BaseController.cs:42");
                    return new user();
                }
            }
            set => _user.UpdateUserAsync(ZermosEmail, value).Wait();
        }

        protected string CurrentZermosVersion => Environment.GetEnvironmentVariable("ZERMOS-WEB-VERSION");
        //make a persistent  
        protected string BuildHash => Environment.GetEnvironmentVariable("PROJECT-HASH");
        
        
        // LOGGING METHODS
        protected void Log(LogLevel logLevel, string message)
        {
            _logger.Log(logLevel, message);
        }
        
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
        
        // CUSTOM APPOINTMENT METHODS
        protected async Task<custom_appointment> AddCustomAppointment(custom_appointment customAppointment)
        {
            await CustomCustomAppointment.AddAppointmentAsync(ZermosEmail, customAppointment);
            return customAppointment;
        }
        
        protected async Task<List<custom_appointment>> GetCustomAppointmentsForUser()
        {
            return await CustomCustomAppointment.GetAppointmentsForUserAsync(ZermosEmail);
        } 
        
        protected async Task<List<custom_appointment>> GetCustomAppointmentsForUser(DateTime startDate, DateTime endDate)
        {
            return await CustomCustomAppointment.GetAppointmentsForUserAsync(ZermosEmail, startDate, endDate);
        }
        
        protected async Task<int> DeleteCustomAppointmentForUser(int id)
        {
            return await CustomCustomAppointment.DeleteAppointmentAsync(ZermosEmail, id);
        }
    }
}