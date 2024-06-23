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

        var form = new Dictionary<string, string>
        {
            {"grant_type", "refresh_token"},
            {"refresh_token", token},
            {"scope", "openid"},
            {"client_id", "D50E0C06-32D1-4B41-A137-A9A850C892C2"}
        };

        var response = await _httpClient.PostAsync("https://inloggen.somtoday.nl/oauth2/token", new FormUrlEncodedContent(form));
            
        if (response.IsSuccessStatusCode == false) return null;
            
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
    
    public async Task<SomtodayGradesModel> GetGrades(user user)
    {
                    var baseUrl =
            $"https://api.somtoday.nl/rest/v1/resultaten/huidigVoorLeerling/{user.somtoday_student_id}?additional=samengesteldeToetskolomId&additional=resultaatkolomId";

        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + user.somtoday_access_token);
        _httpClient.DefaultRequestHeaders.Add("Range", "items=0-99");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        var response = await _httpClient.GetAsync(baseUrl);

        var grades =
            JsonConvert.DeserializeObject<SomtodayGradesModel>(await response.Content.ReadAsStringAsync());

        if (response.IsSuccessStatusCode == false)
        {
            return new SomtodayGradesModel {items = new List<Models.SomtodayGradesModel.Item>()};
        }

        if (int.TryParse(response.Content.Headers.GetValues("Content-Range").First().Split('/')[1],
                out var total))
        {
            
            
            var requests = total / 100 * 100;

            for (var i = 100; i < requests; i += 100)
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + user.somtoday_access_token);
                _httpClient.DefaultRequestHeaders.Add("Range", $"items={i}-{i + 99}");
                _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                response = await _httpClient.GetAsync(baseUrl);

                var _grades =
                    JsonConvert.DeserializeObject<SomtodayGradesModel>(
                        await response.Content.ReadAsStringAsync());
                grades.items.AddRange(_grades.items);
            }
        }
        
        if (grades == null) return new SomtodayGradesModel() {items = new List<Models.SomtodayGradesModel.Item>()};
    
        grades.items = grades.items.OrderBy(x => x.datumInvoer).ToList();

        foreach (var item in grades.items.Where(x => x.resultaatLabelAfkorting == "V"))
            item.geldendResultaat = "7";
    
        foreach (var item in grades.items.Where(x => x.resultaatLabelAfkorting == "G"))
            item.geldendResultaat = "8";


        grades.items = grades.items
            .Where(x => !(string.IsNullOrEmpty(x.omschrijving) && x.weging == 0))
            //.Where(x => (x.type != "DeeltoetsKolom" || x.type != "SamengesteldeToetsKolom"))
            .Where(x => x.geldendResultaat != null)
            .ToList();

        return grades;
    }
}