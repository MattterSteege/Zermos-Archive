using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zermos_Web.Models.Requirements;

namespace Zermos_Web.Controllers
{
    public class SchoolController : BaseController
    {
        public SchoolController(Users user, Shares share, CustomAppointments customCustomAppointment, ILogger<BaseController> logger) : base(user, share, customCustomAppointment, logger) { }

        [ZermosPage]
        [Route("/School/Informatiebord")]
        public IActionResult Informatiebord()
        {
            return PartialView();
        }
        
        [Route("/School/Printen")]
        public IActionResult Printen()
        {
            return StatusCode(299, "https://print.carmel.nl/RicohmyPrint/Account/ExternalLogin/?AuthenticationType=OpenID");
        }
    }
}