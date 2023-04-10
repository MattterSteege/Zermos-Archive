using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Zermos.Models;

namespace Zermos.Controllers
{
    public class ZermeloController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public ZermeloController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Dagrooster()
        {
            string URL = @"https://ccg.zportal.nl/api/v3/liveschedule?access_token=gvcnr9ck37l96uk70u4dcdbuq8&student=58373&week=202315";
            
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(URL);
            var data = await response.Content.ReadAsStringAsync();
            ZermeloScheduleModel.ZermeloSchedule model = JsonConvert.DeserializeObject<ZermeloScheduleModel.ZermeloSchedule>(data);
    
            ViewData["ActivePage"] = "DaySchedule";
            return View("Dagrooster", model.response.data[0].appointments);
        }
    }
}