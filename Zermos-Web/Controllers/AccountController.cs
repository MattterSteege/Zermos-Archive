using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zermos_Web.Models.Requirements;

namespace Zermos_Web.Controllers
{
    [Route("account/[action]")]
    public class AccountController : BaseController
    {
        public AccountController(Users user, ILogger<BaseController> logger) : base(user, logger) { }

        [Authorize]
        [ZermosPage]
        [Route("/Account")]
        public IActionResult Account()
        {
            return PartialView(ZermosUser);
        }
        
        
        [Authorize]
        [ZermosPage]
        [Route("/Account/Instellingen")]
        public IActionResult Settings()
        {
            return PartialView(ZermosUser);
        }
        //https://demos.creative-tim.com/soft-ui-dashboard-tailwind/pages/profile.html


        [HttpPost]
        [Authorize]
        public IActionResult UpdateSetting(string key, string value)
        {
            //only if the property which is being updated is a marked with the 'SettingAttribute'
            var userToUpdate = ZermosUser;
            var property = userToUpdate.GetType().GetProperty(key);
            if (property == null) return BadRequest("Property not found");
            var attribute = property.GetCustomAttributes(typeof(SettingAttribute), false);
            if (attribute.Length == 0) return BadRequest("You may not alter this property");
            property.SetValue(userToUpdate, value);
            ZermosUser = userToUpdate;
            return Ok("200");
        }

                
        [Authorize]
        [ZermosPage]
        [Route("/Account/Debug")]
        public IActionResult Debug()
        {
            return PartialView(ZermosUser);
        }
    }
}