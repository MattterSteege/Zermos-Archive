using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Zermos_Web.Models.PulseCore;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace Zermos_Web.Controllers;

public class SchoolklimaatController : Controller
{
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://app.factorylab.nl/strukton/sensor/")
    };
    
    //https://github.com/MJTSgamer/Zermos/issues/149#issuecomment-1711760659
    
    [ResponseCache(Duration = 600)]
    public async Task<IActionResult> Index()
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
        
        //read the file and return it
        var schoolklimaatJson = await System.IO.File.ReadAllTextAsync("schoolklimaat.json");
        var schoolklimaatModels = JsonConvert.DeserializeObject<Dictionary<string, SchoolklimaatModel>>(schoolklimaatJson);
        return Ok(JsonConvert.SerializeObject(schoolklimaatModels, Formatting.Indented));
    }

    public async Task<Dictionary<string, SchoolklimaatModel>> GetSchoolklimaat()
    {
        Dictionary<string, SchoolklimaatModel> schoolklimaatModels = new Dictionary<string, SchoolklimaatModel>();
        
        Dictionary<string, string> LocationWithUUID = new Dictionary<string, string>();
        LocationWithUUID.Add("B8", "085ecdf6-47ed-55de-80a7-41fc430b3757");
        LocationWithUUID.Add("D3", "3510a390-f361-5299-a438-062e30e77e9a");

        foreach (KeyValuePair<string,string> keyValuePair in LocationWithUUID)
        {
            var response = await _httpClient.GetAsync(keyValuePair.Value);
            var content = await response.Content.ReadAsStringAsync();
            
            var regex = new Regex(@"<[A-Za-z]+\sstyle=""[^""]*""[^""]*""[A-Za-z]+\('([0-9a-fA-F]{8}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{12})',([A-Za-z0-9]+),([A-Za-z0-9]+),'([A-Za-z0-9]+)',([A-Za-z0-9]+),'([A-Za-z0-9]+)',([A-Za-z0-9]+),'([A-Za-z0-9]+)'\)"">");
            var match = regex.Match(content);
            schoolklimaatModels.Add(keyValuePair.Key, new SchoolklimaatModel
            {
                uuid = match.Groups[1].Value,
                temperature = match.Groups[4].Value,
                temperatureScore = int.Parse(match.Groups[3].Value),
                humidity = match.Groups[6].Value,
                humidityScore = int.Parse(match.Groups[5].Value),
                airQuality = match.Groups[8].Value,
                airQualityScore = int.Parse(match.Groups[7].Value)
            });
        }
        
        return schoolklimaatModels;
    }
}