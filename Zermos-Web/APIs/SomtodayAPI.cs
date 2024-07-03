using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Zermos_Web.Models;
using Zermos_Web.Models.SomtodayLeermiddelen;
using Zermos_Web.Models.SomtodayAfwezigheidModel;
using Zermos_Web.Models.SomtodayGradesModel;
using Zermos_Web.Models.SomtodayPlaatsingen;
using Zermos_Web.Models.SomtodayVakgemiddeldenModel;
using Zermos_Web.Utilities;
using Item = Zermos_Web.Models.somtodayHomeworkModel.Item;

namespace Zermos_Web.APIs;

public class SomtodayAPI
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="ZermeloApi"/> class.
    /// </summary>
    /// <param name="httpClient">The HttpClient instance used for making API requests.</param>
    /// <exception cref="ArgumentNullException">Thrown if the provided HttpClient is null.</exception>
    public SomtodayAPI(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _httpClient.DefaultRequestHeaders.Add("Origin", "https://somtoday.nl");
    }

    public async Task<SomtodayAuthenticatieModel> RefreshTokenAsync(string token)
    {
        if (token == null) throw new ArgumentNullException(nameof(token));

        string clientId = (string) TokenUtils.DecodeJwt(token).payload.client_id;
        
        var form = new Dictionary<string, string>
        {
            {"grant_type", "refresh_token"},
            {"refresh_token", token},
            {"scope", "openid"},
            {"client_id", clientId}
        };

        var response = await _httpClient.PostAsync("https://inloggen.somtoday.nl/oauth2/token", new FormUrlEncodedContent(form));

        if (response.IsSuccessStatusCode == false)
        {
            Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            return null;
        }
            
        return JsonConvert.DeserializeObject<SomtodayAuthenticatieModel>(await response.Content.ReadAsStringAsync());
    }
    
    public async Task<SomtodayAfwezigheidModel> GetAfwezigheidAsync(user user)
    {
        SchooljaarUtils.Schooljaar currentSchoolyear = SchooljaarUtils.getCurrentSchooljaar();
        
        //https://api.somtoday.nl/rest/v1/waarnemingen?waarnemingSoort=Afwezig
        
        var baseurl = $"https://api.somtoday.nl/rest/v1/absentiemeldingen?begindatumtijd={currentSchoolyear.vanafDatumDate:yyyy-MM-dd}&einddatumtijd={currentSchoolyear.totDatumDate:yyyy-MM-dd}";
        
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("authorization", "Bearer " + user.somtoday_access_token);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        
        var response = await _httpClient.GetAsync(baseurl);

        if (response.IsSuccessStatusCode == false)
            return null;
        
        return JsonConvert.DeserializeObject<SomtodayAfwezigheidModel>(await response.Content.ReadAsStringAsync());
    }
    
    public async Task<SomtodayLeermiddelenModel> GetStudiemateriaal(user user)
    {
        var baseurl = $"https://api.somtoday.nl/rest/v1/studiemateriaal/algemeen/{user.somtoday_student_id}";
        
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("authorization", "Bearer " + user.somtoday_access_token);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        
        var response = await _httpClient.GetAsync(baseurl);

        if (response.IsSuccessStatusCode == false)
            return null;
        
        var json = await response.Content.ReadAsStringAsync();
        
        return JsonConvert.DeserializeObject<SomtodayLeermiddelenModel>(await response.Content.ReadAsStringAsync());
    }
    
    /// <summary>
    /// Fetches all the grades from think current year
    /// </summary>
    /// <param name="user"></param>
    /// <param name="subjectUUID"></param>
    /// <returns></returns>
    public async Task<SomtodayGradesModel> GetCurrentGrades(user user)
    {
        // Fetch both at the same time and wait for both to finish
        var baseurl = $"https://api.somtoday.nl/rest/v1/geldendvoortgangsdossierresultaten/leerling/{user.somtoday_student_id}?type=Toetskolom&type=DeeltoetsKolom&type=Werkstukcijferkolom&type=Advieskolom&additional=vaknaam&additional=resultaatkolom&additional=vakuuid&additional=lichtinguuid&sort=desc-geldendResultaatCijferInvoer";
        var baseurl2 = $"https://api.somtoday.nl/rest/v1/geldendexamendossierresultaten/leerling/{user.somtoday_student_id}?type=Toetskolom&type=DeeltoetsKolom&type=Werkstukcijferkolom&type=Advieskolom&additional=vaknaam&additional=resultaatkolom&&additional=vakuuid&additional=lichtinguuid&sort=desc-geldendResultaatCijferInvoer";

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("authorization", "Bearer " + user.somtoday_access_token);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        var task1 = _httpClient.GetAsync(baseurl);
        var task2 = _httpClient.GetAsync(baseurl2);

        await Task.WhenAll(task1, task2);

        var response = task1.Result;
        var response2 = task2.Result;

        if (response.IsSuccessStatusCode == false || response2.IsSuccessStatusCode == false)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        var json2 = await response2.Content.ReadAsStringAsync();

        var grades = JsonConvert.DeserializeObject<SomtodayGradesModel>(json);
        var grades2 = JsonConvert.DeserializeObject<SomtodayGradesModel>(json2);

        grades.items.AddRange(grades2.items);

        return grades;
    }

    public async Task<SomtodayPlaatsingenModel> GetPlaatsingen(user user)
    {
        //https://api.somtoday.nl/rest/v1/plaatsingen?leerling=1409824200
        
        var baseurl = $"https://api.somtoday.nl/rest/v1/plaatsingen?leerling={user.somtoday_student_id}";
        
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("authorization", "Bearer " + user.somtoday_access_token);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        
        var response = await _httpClient.GetAsync(baseurl);
        
        if (response.IsSuccessStatusCode == false)
            return null;
        
        return JsonConvert.DeserializeObject<SomtodayPlaatsingenModel>(await response.Content.ReadAsStringAsync());
    }
    
    /// <param name="year">-1 for current, 1 for 1ste year, 2e for 2e etc.</param>
    public async Task<SomtodayVakgemiddeldenModel> Getvakgemiddelden(user user, int year)
    {
        //https://api.somtoday.nl/rest/v1/vakkeuzes/plaatsing/d3ff5175-162b-493a-80c9-febb405665bc/vakgemiddelden
        var years = await GetPlaatsingen(user);
        var yearId = "";
        if (year == -1)
            yearId = years.items[^1].UUID;
        else
            yearId = years.items[year - 1].UUID;
        
        var baseurl = $"https://api.somtoday.nl/rest/v1/vakkeuzes/plaatsing/{yearId}/vakgemiddelden";
        
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("authorization", "Bearer " + user.somtoday_access_token);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        
        var response = await _httpClient.GetAsync(baseurl);
        
        if (response.IsSuccessStatusCode == false)
            return null;
        
        return JsonConvert.DeserializeObject<SomtodayVakgemiddeldenModel>(await response.Content.ReadAsStringAsync());
    }
}