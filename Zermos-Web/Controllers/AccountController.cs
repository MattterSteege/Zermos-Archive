using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Entities;
using Infrastructure.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zermos_Web.Models.Requirements;

namespace Zermos_Web.Controllers
{
    //https://demos.creative-tim.com/soft-ui-dashboard-tailwind/pages/profile.html
    [Route("account/[action]")]
    public class AccountController : BaseController
    {
        public AccountController(Users user, Shares share, ILogger<BaseController> logger) : base(user, share, logger) { }

        [Authorize]
        [ZermosPage]
        [Route("/Account")]
        public async Task<IActionResult> Account()
        {
            dynamic model = new
            {
                user = ZermosUser,
                shares = await GetShares()
            };
            
            return PartialView(model);
        }
        
        
        [Authorize]
        [ZermosPage]
        [Route("/Account/Instellingen")]
        public IActionResult Settings()
        {
            return PartialView(ZermosUser);
        }
        
        [Authorize]
        [ZermosPage]
        [Route("/Account/Debug")]
        public IActionResult Debug()
        {
            return PartialView(ZermosUser);
        }

        [HttpPost]
        [Authorize]
        public IActionResult UpdateSetting(string key, string value)
        {
            #if DEBUG
            if (key == "version_used" && ZermosEmail != "58373@ccg-leerling.nl")
            {
                return BadRequest("You may not alter this property when in debug mode");
            }
            #endif
            
            //only if the property which is being updated is a marked with the 'SettingAttribute'
            var userToUpdate = ZermosUser;
            if (userToUpdate == null) return BadRequest("No account was found");
            var property = userToUpdate.GetType().GetProperty(key);
            if (property == null) return BadRequest("Property not found");
            var attribute = property.GetCustomAttributes(typeof(SettingAttribute), false);
            if (attribute.Length == 0) return BadRequest("You may not alter this property");
            //try to set the value as the type of the property
            try {
                property.SetValue(userToUpdate, Convert.ChangeType(value, property.PropertyType));
            }
            catch(Exception e) {
                if (e.InnerException != null && e.InnerException.Message.StartsWith("Invalid")) return BadRequest(e.InnerException.Message);
                return BadRequest("Invalid value type, expected " + property.PropertyType.Name + " got " + value.GetType().Name);
            }

            ZermosUser = userToUpdate;
            return Ok("200");
        }
        
        [HttpGet]
        [Authorize]
        public IActionResult GetSetting(string key)
        {
            //only if the property which is being updated is a marked with the 'SettingAttribute'
            var userToUpdate = ZermosUser;
            if (userToUpdate == null) return BadRequest("No account was found");
            var property = userToUpdate.GetType().GetProperty(key);
            if (property == null) return BadRequest("Property not found");
            var attribute = property.GetCustomAttributes(typeof(SettingAttribute), false);
            if (attribute.Length == 0) return BadRequest("You may not request this property");
            attribute = property.GetCustomAttributes(typeof(RequestableAttribute), false);
            if (attribute.Length == 0) return BadRequest("You may not request this property");
            return Ok(property.GetValue(userToUpdate));
        }
    }
}