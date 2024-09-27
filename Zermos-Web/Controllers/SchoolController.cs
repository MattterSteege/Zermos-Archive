using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Zermos_Web.Models.PulseCore;
using Zermos_Web.Models.Requirements;
using Zermos_Web.Models.zermelo;

namespace Zermos_Web.Controllers
{
    public class SchoolController : BaseController
    {
        public SchoolController(Users user, Shares share, CustomAppointments customCustomAppointment, ILogger<BaseController> logger) : base(user, share, customCustomAppointment, logger) { }
        
        private readonly HttpClient _httpClient = new()
        {
            BaseAddress = new Uri("https://app.factorylab.nl/strukton/sensor/")
        };
        
        
        [ZermosPage]
        [Route("/School/Informatiebord")]
        public IActionResult Informatiebord()
        {
            return PartialView();
        }
        
        /// <summary>
        /// Gets the schoolklimaat data
        /// </summary>
        /// <param name="locatie">the location of the meter, or * for everything</param>
        /// <param name="isOnline">0 for offline, 1 for online, or 2 for everything</param>
        /// <returns></returns>
        /// https://github.com/MJTSgamer/Zermos/issues/149#issuecomment-1711760659
        [Route("/Schoolklimaat")]
        [ResponseCache(Duration = 600)]
        public async Task<IActionResult> Index(string locatie = "*", int isOnline = 2)
        {
            //check if the file exists otherwise create it
            var file = new FileInfo("schoolklimaat.json");
            if (!file.Exists)
            {
                file.Create().Close();

                var schoolklimaat = await GetSchoolklimaat();
                var json = JsonConvert.SerializeObject(schoolklimaat);
                await System.IO.File.WriteAllTextAsync("schoolklimaat.json", json);
            }

            //check when the file was last modified
            var lastModified = file.LastWriteTime;
            var now = DateTime.Now;
            var difference = now - lastModified;

            //if the file is older than 10 minutes, update it
            if (difference.TotalMinutes > 10)
            {
                var schoolklimaat = await GetSchoolklimaat();
#if DEBUG
                var json = JsonConvert.SerializeObject(schoolklimaat, Formatting.Indented);
#else
                var json = JsonConvert.SerializeObject(schoolklimaat);
#endif
                await System.IO.File.WriteAllTextAsync("schoolklimaat.json", json);
            }
            
            var schoolklimaatModels = JsonConvert.DeserializeObject<Dictionary<string, SchoolklimaatModel>>(await System.IO.File.ReadAllTextAsync("schoolklimaat.json"));
            
            if (locatie != "*")
            {
                schoolklimaatModels = schoolklimaatModels.Where(x => x.Key.ToLower() == locatie.ToLower())
                    .ToDictionary(x => x.Key, x => x.Value);
                return Ok(JsonConvert.SerializeObject(schoolklimaatModels, Formatting.Indented));
            }
            
            if (isOnline == 0)
            {
                schoolklimaatModels = schoolklimaatModels.Where(x => !x.Value.isOnline)
                    .ToDictionary(x => x.Key, x => x.Value);
                return Ok(JsonConvert.SerializeObject(schoolklimaatModels, Formatting.Indented));
            }
            
            if (isOnline == 1)
            {
                schoolklimaatModels = schoolklimaatModels.Where(x => x.Value.isOnline)
                    .ToDictionary(x => x.Key, x => x.Value);
                return Ok(JsonConvert.SerializeObject(schoolklimaatModels, Formatting.Indented));
            }
            
            return Ok(JsonConvert.SerializeObject(schoolklimaatModels, Formatting.Indented));
        }

        public async Task<Dictionary<string, SchoolklimaatModel>> GetSchoolklimaat()
        {
            var lokalen = (await (await _httpClient.GetAsync("https://zermos-docs.kronk.tech/schoolklimaat.txt"))
                .Content.ReadAsStringAsync()).Split("\n");

            Dictionary<string, string> LocationWithUUID = new Dictionary<string, string>();

            foreach (string lokaalMetId in lokalen)
            {
                if (!lokaalMetId.Contains(':'))
                    continue;

                var lokaal = lokaalMetId.Split(": ")[0];
                var id = lokaalMetId.Split(": ")[1];
                LocationWithUUID.Add(lokaal, id);
            }

            Dictionary<string, SchoolklimaatModel> schoolklimaatModels = new Dictionary<string, SchoolklimaatModel>();

            foreach (KeyValuePair<string, string> keyValuePair in LocationWithUUID)
            {
                var response = await _httpClient.GetAsync(keyValuePair.Value);
                var content = await response.Content.ReadAsStringAsync();

                var regex = new Regex(
                    @"<[A-Za-z]+\sstyle=""[^""]*""[^""]*""[A-Za-z]+\('([0-9a-fA-F]{8}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{12})',([A-Za-z0-9]+),([A-Za-z0-9]+),'([A-Za-z0-9]+)',([A-Za-z0-9]+),'([A-Za-z0-9]+)',([A-Za-z0-9]+),'([A-Za-z0-9]+)'\)"">");
                var match = regex.Match(content);

                if (!match.Success)
                {
                    schoolklimaatModels.Add(keyValuePair.Key, new SchoolklimaatModel
                    {
                        uuid = keyValuePair.Value,
                        isOnline = false
                    });
                    
                    continue;
                }

                schoolklimaatModels.Add(keyValuePair.Key, new SchoolklimaatModel
                {
                    uuid = match.Groups[1].Value,
                    temperature = match.Groups[4].Value,
                    temperatureScore = int.Parse(match.Groups[3].Value),
                    humidity = match.Groups[6].Value,
                    humidityScore = int.Parse(match.Groups[5].Value),
                    airQuality = match.Groups[8].Value,
                    airQualityScore = int.Parse(match.Groups[7].Value),
                    isOnline = true
                });
            }

            return schoolklimaatModels;
        }
        
        [Route("/School/Printen")]
        public IActionResult Printen()
        {
            return StatusCode(299, "https://print.carmel.nl/RicohmyPrint/Account/ExternalLogin/?AuthenticationType=OpenID");
        }
    }
}