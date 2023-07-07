// using System.Collections.Generic;
// using System.Linq;
// using System.Security.Claims;
// using System.Threading.Tasks;
// using Infrastructure;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Logging;
// using Newtonsoft.Json;
// using Zermos_Web.Models;
// using Zermos_Web.Models.Requirements;
//
// namespace Zermos_Web.Controllers
// {
//     //[Authorize]
//     [Route("notities/{action=Notities}")]
//     [NotImplementedYet]
//     public class NotitiesController : Controller
//     {
//         private readonly IConfiguration _config;
//         private readonly ILogger<NotitiesController> _logger;
//         private readonly Users _users;
//
//         public NotitiesController(ILogger<NotitiesController> logger, IConfiguration config, Users users)
//         {
//             _logger = logger;
//             _config = config;
//             _users = users;
//         }
//         
//         [AddLoadingScreen("notities laden...")]
//         public async Task<IActionResult> Notities()
//         {
//             ViewData["add_css"] = "notities";
//             var user = await _users.GetUserAsync(User.FindFirstValue("email"));
//             
//             var notes = JsonConvert.DeserializeObject<List<Notitie>>(user.notities ?? "[]");
//
//             return View(notes);
//         }
//         
//         [HttpGet]
//         public IActionResult Nieuw()
//         {
//             return View("Bewerk");
//         }
//         
//         [HttpPost]
//         public async Task<IActionResult> Nieuw(Notitie notitie)
//         {
//             if (!ModelState.IsValid)
//                 return BadRequest();
//             
//
//             var user = await _users.GetUserAsync(User.FindFirstValue("email"));
//             
//             var notes = JsonConvert.DeserializeObject<List<Notitie>>(user.notities ?? "[]");
//             
//             if (notes.Any(n => n.id == notitie.id))
//                 return BadRequest("Notitie bestaat al");
//             
//             user.notities = JsonConvert.SerializeObject(notes);
//
//             await _users.UpdateUserAsync(User.FindFirstValue("email"), user);
//
//             return Ok("Notitie toegevoegd");
//         }
//         
//         [HttpPost]
//         public async Task<IActionResult> Verwijder(int id)
//         {
//             var user = await _users.GetUserAsync(User.FindFirstValue("email"));
//             
//             var notes = JsonConvert.DeserializeObject<List<Notitie>>(user.notities ?? "[]");
//             
//             if (!notes.Any(n => n.id == id))
//                 return BadRequest("Notitie bestaat niet");
//             
//             notes.Remove(notes.First(n => n.id == id));
//             
//             user.notities = JsonConvert.SerializeObject(notes);
//
//             await _users.UpdateUserAsync(User.FindFirstValue("email"), user);
//
//             return Ok("Notitie verwijderd");
//         }
//         
//         [HttpGet]
//         public async Task<IActionResult> Bewerk(int id)
//         {
//             var user = await _users.GetUserAsync(User.FindFirstValue("email"));
//             
//             var notes = JsonConvert.DeserializeObject<List<Notitie>>(user.notities ?? "[]");
//             
//             if (!notes.Any(n => n.id == id))
//                 return BadRequest("Notitie bestaat niet");
//             
//             return View(notes.First(n => n.id == id));
//         }
//     }
// }