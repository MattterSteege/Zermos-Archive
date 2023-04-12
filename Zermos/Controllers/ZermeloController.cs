using System;
using System.Collections.Generic;
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
    
            int dag = 0;
            int index = 0;

            List<List<ZermeloScheduleModel.Appointment>> dagroosters = new List<List<ZermeloScheduleModel.Appointment>>(5);
            //add 5 lists to the list
            for (int i = 0; i < 5; i++)
            {
                dagroosters.Add(new List<ZermeloScheduleModel.Appointment>());
            }

            foreach (ZermeloScheduleModel.Appointment appointment in model.response.data[0].appointments)
            {
                int currentDay = (int) appointment.start.ToDateTime().ToDayTimeSavingDate().DayOfWeek;
                dagroosters[currentDay - 1].Add(appointment);
            }

            ViewData["ActivePage"] = "DaySchedule";
            return View("Dagrooster", dagroosters);
        }
    }
}