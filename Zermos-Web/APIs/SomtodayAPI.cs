using System;
using System.Collections.Generic;
using System.Globalization;
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
using Zermos_Web.Models.SortedSomtodayGradesModel;
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

    #region REFACTOR
    /*
     
     Refactor using an enum for the type of grades you want:
     voortgang, exam, average, history
     
     */
    /// <summary>
    /// Fetches all the grades from think current year
    /// </summary>
    /// <param name="user"></param>
    /// <param name="appendGradeHistory"></param>
    /// <returns></returns>
    public async Task<SortedSomtodayGradesModel> GetCurrentGrades(user user, bool appendGradeHistory)
    {
        // Fetch both at the same time and wait for both to finish
        var baseurl =
            $"https://api.somtoday.nl/rest/v1/geldendvoortgangsdossierresultaten/leerling/{user.somtoday_student_id}?type=Toetskolom&type=DeeltoetsKolom&type=Werkstukcijferkolom&type=Advieskolom&additional=vaknaam&additional=resultaatkolom&additional=vakuuid&additional=lichtinguuid&sort=desc-geldendResultaatCijferInvoer";
        var baseurl2 =
            $"https://api.somtoday.nl/rest/v1/geldendexamendossierresultaten/leerling/{user.somtoday_student_id}?type=Toetskolom&type=DeeltoetsKolom&type=Werkstukcijferkolom&type=Advieskolom&additional=vaknaam&additional=resultaatkolom&&additional=vakuuid&additional=lichtinguuid&sort=desc-geldendResultaatCijferInvoer";

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
        var gradesSE = JsonConvert.DeserializeObject<SomtodayGradesModel>(json2);
        
        return SortGrades(grades, gradesSE, null, appendGradeHistory);
    }
    
    /// <param name="year">-1 for current, 1 for 1ste year, 2e for 2e etc.</param>
    public async Task<SomtodayVakgemiddeldenModel> Getvakgemiddelden(user user, SomtodayPlaatsingenModel plaatsingen, int year)
    {
        string yearId;
        if (year == -1)
            yearId = plaatsingen.items[^1].UUID;
        else
            yearId = plaatsingen.items[year - 1].UUID;
        
        var baseurl = $"https://api.somtoday.nl/rest/v1/vakkeuzes/plaatsing/{yearId}/vakgemiddelden";
        
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("authorization", "Bearer " + user.somtoday_access_token);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        
        var response = await _httpClient.GetAsync(baseurl);
        
        if (response.IsSuccessStatusCode == false)
            return null;
        
        return JsonConvert.DeserializeObject<SomtodayVakgemiddeldenModel>(await response.Content.ReadAsStringAsync());
    }
    
        /// <summary>
    /// Fetches all the grades from think current year
    /// </summary>
    /// <param name="user"></param>
    /// <param name="appendGradeHistory"></param>
    /// <returns></returns>
    public async Task<SortedSomtodayGradesModel> GetCurrentGradesAndvakgemiddelden(user user, SomtodayPlaatsingenModel plaatsingen, int year, bool appendGradeHistory)
    {
        // Fetch all at the same time and wait for both to finish
        string yearId;
        if (year == -1)
            yearId = plaatsingen.items[^1].UUID;
        else
            yearId = plaatsingen.items[year - 1].UUID;
        
        var baseurl = $"https://api.somtoday.nl/rest/v1/geldendvoortgangsdossierresultaten/leerling/{user.somtoday_student_id}?type=Toetskolom&type=DeeltoetsKolom&type=Werkstukcijferkolom&type=Advieskolom&additional=vaknaam&additional=resultaatkolom&additional=vakuuid&additional=lichtinguuid&sort=desc-geldendResultaatCijferInvoer";
        var baseurl2 = $"https://api.somtoday.nl/rest/v1/geldendexamendossierresultaten/leerling/{user.somtoday_student_id}?type=Toetskolom&type=DeeltoetsKolom&type=Werkstukcijferkolom&type=Advieskolom&additional=vaknaam&additional=resultaatkolom&&additional=vakuuid&additional=lichtinguuid&sort=desc-geldendResultaatCijferInvoer";
        var baseurl3 = $"https://api.somtoday.nl/rest/v1/vakkeuzes/plaatsing/{yearId}/vakgemiddelden";

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("authorization", "Bearer " + user.somtoday_access_token);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        var task1 = _httpClient.GetAsync(baseurl);
        var task2 = _httpClient.GetAsync(baseurl2);
        var task3 = _httpClient.GetAsync(baseurl3);

        await Task.WhenAll(task1, task2, task3);

        var response = task1.Result;
        var response2 = task2.Result;
        var response3 = task3.Result;

        if (response.IsSuccessStatusCode == false || response2.IsSuccessStatusCode == false || response3.IsSuccessStatusCode == false)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        var json2 = await response2.Content.ReadAsStringAsync();
        var json3 = await response3.Content.ReadAsStringAsync();

        var grades = JsonConvert.DeserializeObject<SomtodayGradesModel>(json);
        var gradesSE = JsonConvert.DeserializeObject<SomtodayGradesModel>(json2);
        var vakgemiddelden = JsonConvert.DeserializeObject<SomtodayVakgemiddeldenModel>(json3);
        
        return SortGrades(grades, gradesSE, vakgemiddelden, appendGradeHistory);
    }
    
    public SortedSomtodayGradesModel SortGrades(SomtodayGradesModel grades, SomtodayGradesModel gradesSE, SomtodayVakgemiddeldenModel vakgemiddelden, bool appendGradeHistory)
    {
    var gradesBySubject = grades.items.GroupBy(x => x.additionalObjects.vaknaam).ToDictionary(g => g.Key, g => g.ToList());
        var gradesBySubjectSE = gradesSE.items.GroupBy(x => x.additionalObjects.vaknaam).ToDictionary(g => g.Key, g => g.ToList());

        var distinctSubjects = gradesBySubject.Keys.Union(gradesBySubjectSE.Keys);

        var gradesBySubjectGrouped = distinctSubjects.Select(subject =>
        {
            gradesBySubject.TryGetValue(subject, out var grade);
            gradesBySubjectSE.TryGetValue(subject, out var gradeSE);
            return new
            {
                subject,
                grades = grade ?? new List<Models.SomtodayGradesModel.Item>(),
                gradesSE = gradeSE ?? new List<Models.SomtodayGradesModel.Item>()
            };
        }).ToList();
        
        
        var sortedGrades = new SortedSomtodayGradesModel
        {
            items = new List<Models.SortedSomtodayGradesModel.Item>(),
            lastGrades = new List<Models.SomtodayGradesModel.Item>(),
            voortGangsdossierGemiddelde = vakgemiddelden.voortgangsdossierGemiddelde
        };
        
        foreach (var grade in gradesBySubjectGrouped)
        {
            var gemiddelden = vakgemiddelden.gemiddelden.FirstOrDefault(x => x.vakNaam == grade.subject);
            
            var item = new Models.SortedSomtodayGradesModel.Item();
            
            item.cijfers = grade.grades;
            item.weging = item.cijfers.Sum(x => x.weging);
            item.cijfer = gemiddelden.isVoorVoortgangsdossier ? gemiddelden.voortgangsdossierResultaat.cijfer.ToString("0.0000", CultureInfo.InvariantCulture) : "-";
            
            item.cijfersSE = grade.gradesSE;
            item.wegingSE = item.cijfersSE.Sum(x => x.weging);
            item.cijferSE = gemiddelden.isVoorExamendossier ? gemiddelden.examendossierResultaat.cijfer.ToString("0.0000", CultureInfo.InvariantCulture) : "-";
            
            item.vaknaam = grade.subject;
            item.vakAfkorting = gemiddelden.vakAfkorting;
            item.vakuuid = (item.cijfers.Count > 0 ? item.cijfers[0].additionalObjects.vakuuid : item.cijfersSE[0].additionalObjects.vakuuid);

            sortedGrades.items.Add(item);
        }
        
        if (appendGradeHistory)
        {
            var lastGrades = new List<Models.SomtodayGradesModel.Item>();
            foreach (var grade in gradesBySubjectGrouped)
            {
                lastGrades.AddRange(grade.grades);
                lastGrades.AddRange(grade.gradesSE);
            }
            sortedGrades.lastGrades = lastGrades.OrderByDescending(x => x.datumInvoerEerstePoging).ToList();
        }
        
        return sortedGrades;
    }
    #endregion

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
}