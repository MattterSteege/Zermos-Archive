using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Infrastructure.Entities;
using Newtonsoft.Json;
using Zermos_Web.Models;
using Zermos_Web.Models.SomtodayLeermiddelen;
using Zermos_Web.Models.SomtodayAfwezigheidModel;
using Zermos_Web.Models.SomtodayGradesModel;
using Zermos_Web.Models.somtodayHomeworkModel;
using Zermos_Web.Models.SomtodayPlaatsingen;
using Zermos_Web.Models.SomtodayRoosterModel;
using Zermos_Web.Models.SomtodayVakgemiddeldenModel;
using Zermos_Web.Models.SortedSomtodayGradesModel;
using Zermos_Web.Utilities;

namespace Zermos_Web.APIs;

public class  SomtodayAPI
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
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.topicus.platinum+json; charset=utf-8");
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("content-type", "application/vnd.topicus.platinum+json; charset=utf-8");
        _httpClient.DefaultRequestHeaders.Add("Origin", "https://leerling.somtoday.nl");
    }

    public async Task<SomtodayAuthenticatieModel> RefreshTokenAsync(string token)
    {
        if (token == null) 
            throw new ArgumentNullException(nameof(token));

        //https://https://somtoday-refresh.mjtsgamer.workers.dev?refresh_token=token
        var response = await _httpClient.GetAsync($"https://somtoday-refresh.mjtsgamer.workers.dev?refresh_token={token}");
        
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("undefined error: " + await response.Content.ReadAsStringAsync());
        }
        
        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<SomtodayAuthenticatieModel>(content);
        
        // string clientId = (string) TokenUtils.DecodeJwt(token).payload.client_id;
        //
        // var form = new Dictionary<string, string>
        // {
        //     {"grant_type", "refresh_token"},
        //     {"refresh_token", token},
        //     {"scope", "openid"},
        //     {"client_id", clientId}
        // };
        //
        // var response = await _httpClient.PostAsync("https://inloggen.somtoday.nl/oauth2/token", new FormUrlEncodedContent(form));
        //
        // Console.WriteLine(JsonConvert.SerializeObject(response));
        //
        // if (!response.IsSuccessStatusCode)
        // {
        //     var errorMessage = await response.Content.ReadAsStringAsync();
        //     
        //     if (errorMessage.EndsWith("access_denied")) //error_description=Access denied by resource owner or authorization server: Unauthorized account error=access_denied
        //         Console.Write("Unauthorized account, token has expired.");
        //     
        //     else if (errorMessage.Contains("invalid_grant")) //error_description=Invalid grant: Token revoked error=invalid_grant
        //         Console.Write("Invalid refresh token. Token has been revoked."); //This can not be fixed by the user, the user has to re-authenticate.
        //     
        //     else 
        //         Console.Write("undefined error: " + errorMessage);
        //     
        //     return null;
        // }
        //
        // var content = await response.Content.ReadAsStringAsync();
        // return JsonConvert.DeserializeObject<SomtodayAuthenticatieModel>(content);
    }
    
    public async Task<SomtodayAfwezigheidModel> GetAfwezigheidAsync(user user)
    {
        SchooljaarUtils.Schooljaar currentSchoolyear = SchooljaarUtils.getCurrentSchooljaar();
        
        //https://api.somtoday.nl/rest/v1/waarnemingen?waarnemingSoort=Afwezig
        
        var baseurl = $"https://passtrough.mjtsgamer.workers.dev/https://api.somtoday.nl/rest/v1/absentiemeldingen?begindatumtijd={currentSchoolyear.vanafDatumDate:yyyy-MM-dd}&einddatumtijd={currentSchoolyear.totDatumDate:yyyy-MM-dd}";
        //var baseurl = $"https://somtoday-afwezigheid.mjtsgamer.workers.dev?accessToken={user.somtoday_access_token}";
        
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
        var baseurl = $"https://passtrough.mjtsgamer.workers.dev/https://api.somtoday.nl/rest/v1/studiemateriaal/algemeen/{user.somtoday_student_id}";
        //var baseurl = $"https://somtoday-leermiddelen.mjtsgamer.workers.dev?studentId={user.somtoday_student_id}&accessToken={user.somtoday_access_token}";
        
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("authorization", "Bearer " + user.somtoday_access_token);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        
        var response = await _httpClient.GetAsync(baseurl);

        if (response.IsSuccessStatusCode == false)
            return null;
        
        return JsonConvert.DeserializeObject<SomtodayLeermiddelenModel>(await response.Content.ReadAsStringAsync());
    }
    
    [Flags]
    public enum GradeType
    {
        Voortgang = 1,
        Exam = 2,
        Average = 4,
        History = 8,
    }

    /// <summary>
    /// Fetches all the grades from think current year. This is for the general grades page.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="plaatsingen"></param>
    /// <param name="gradeType"></param>
    /// <returns></returns>
    public async Task<SortedSomtodayGradesModel> GetCurrentGradesAndVakgemiddelden(user user, SomtodayPlaatsingenModel plaatsingen, 
        GradeType gradeType = GradeType.Voortgang | GradeType.Exam | GradeType.Average | GradeType.History)
    {
        #if DEBUG
        var watch = Stopwatch.StartNew();
        #endif
        
        var urls = new[]
        
        {
            gradeType.HasFlag(GradeType.Voortgang) ? $"https://passtrough.mjtsgamer.workers.dev/https://api.somtoday.nl/rest/v1/geldendvoortgangsdossierresultaten/leerling/{user.somtoday_student_id}?type=Toetskolom&type=SamengesteldeToetsKolom&type=DeeltoetsKolom&type=Werkstukcijferkolom&type=Advieskolom&additional=vaknaam&additional=resultaatkolom&additional=vakuuid&additional=lichtinguuid&sort=desc-geldendResultaatCijferInvoer" : null,
            gradeType.HasFlag(GradeType.Exam) ? $"https://passtrough.mjtsgamer.workers.dev/https://api.somtoday.nl/rest/v1/geldendexamendossierresultaten/leerling/{user.somtoday_student_id}?type=Toetskolom&type=SamengesteldeToetsKolom&type=DeeltoetsKolom&type=Werkstukcijferkolom&type=Advieskolom&additional=vaknaam&additional=resultaatkolom&additional=vakuuid&additional=lichtinguuid&sort=desc-geldendResultaatCijferInvoer" : null,
            gradeType.HasFlag(GradeType.Average) ? $"https://passtrough.mjtsgamer.workers.dev/https://api.somtoday.nl/rest/v1/vakkeuzes/plaatsing/{plaatsingen.items[^1].UUID}/vakgemiddelden" : null
        };
        
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("authorization", "Bearer " + user.somtoday_access_token);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        
        var tasks = urls.Select(url => url != null ? _httpClient.GetAsync(url) : Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK))).ToArray();
        
        await Task.WhenAll(tasks);
        
        if (tasks.Any(t => !t.Result.IsSuccessStatusCode))
            return null;
        
        var jsonTasks = tasks.Select(t => t.Result.Content.ReadAsStringAsync()).ToArray();
        await Task.WhenAll(jsonTasks);
        
        var grades = gradeType.HasFlag(GradeType.Voortgang) ? JsonConvert.DeserializeObject<SomtodayGradesModel>(jsonTasks[0].Result) : null;
        var gradesSE = gradeType.HasFlag(GradeType.Exam) ? JsonConvert.DeserializeObject<SomtodayGradesModel>(jsonTasks[1].Result) : null;
        var vakgemiddelden = gradeType.HasFlag(GradeType.Average) ? JsonConvert.DeserializeObject<SomtodayVakgemiddeldenModel>(jsonTasks[2].Result) : null;
        
        var gradesBySubject = grades?.items.GroupBy(x => x.additionalObjects.vaknaam).ToDictionary(g => g.Key, g => g.ToList()) ?? new Dictionary<string, List<Models.SomtodayGradesModel.Item>>();
        var gradesBySubjectSE = gradesSE?.items.GroupBy(x => x.additionalObjects.vaknaam).ToDictionary(g => g.Key, g => g.ToList()) ?? new Dictionary<string, List<Models.SomtodayGradesModel.Item>>();
        
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
            voortGangsdossierGemiddelde = vakgemiddelden?.voortgangsdossierGemiddelde,
            relevanteCijferLichtingUUID = vakgemiddelden?.gemiddelden.FirstOrDefault()?.relevanteCijferLichtingUUID,
            leerjaarUUID = plaatsingen.items[^1].UUID,
            plaatsing = plaatsingen.items
        };
        
        foreach (var grade in gradesBySubjectGrouped)
        {
            var gemiddelden = vakgemiddelden?.gemiddelden.FirstOrDefault(x => x.vakNaam == grade.subject);
            vakgemiddelden?.gemiddelden.Remove(gemiddelden);
            
            var item = new Models.SortedSomtodayGradesModel.Item
            {
                cijfers = grade.grades ?? new List<Models.SomtodayGradesModel.Item>(),
                weging = grade.grades.Sum(x => x.weging),
                cijfer = gemiddelden?.isVoorVoortgangsdossier == true ? gemiddelden.voortgangsdossierResultaat.formattedResultaat : "-",
                cijfersSE = grade.gradesSE ?? new List<Models.SomtodayGradesModel.Item>(),
                wegingSE = grade.gradesSE.Sum(x => x.weging),
                cijferSE = gemiddelden?.isVoorExamendossier == true ? gemiddelden.examendossierResultaat.formattedResultaat : "-",
                vakNaam = grade.subject,
                vakAfkorting = gemiddelden?.vakAfkorting,
                vakuuid = (grade.grades.FirstOrDefault()?.additionalObjects.vakuuid ?? grade.gradesSE.FirstOrDefault()?.additionalObjects.vakuuid),
            };
        
            sortedGrades.items.Add(item);
        }
        
        foreach (Gemiddelden gemiddelden in vakgemiddelden?.gemiddelden)
        {
            var item = new Models.SortedSomtodayGradesModel.Item
            {
                cijfer = gemiddelden.isVoorVoortgangsdossier ? gemiddelden.voortgangsdossierResultaat.formattedResultaat : "-",
                cijferSE = gemiddelden.isVoorExamendossier ? gemiddelden.examendossierResultaat.formattedResultaat : "-",
                vakNaam = gemiddelden.vakNaam,
                vakAfkorting = gemiddelden.vakAfkorting,
                vakuuid = gemiddelden.vakUUID
            };
        
            sortedGrades.items.Add(item);
        }
        
        sortedGrades.items = sortedGrades.items.OrderBy(x => x.vakNaam).ToList();
        
        if (gradeType.HasFlag(GradeType.History))
        {
            var lastGrades = gradesBySubjectGrouped.SelectMany(g => g.grades.Concat(g.gradesSE)).OrderByDescending(x => x.datumInvoerEerstePoging).ToList();
            sortedGrades.lastGrades = lastGrades;
        }
        
        #if DEBUG
        watch.Stop();
        Console.WriteLine(watch.ElapsedMilliseconds + "ms");
        #endif
        
        return sortedGrades;
    }
    
    /// <summary>
    /// Fetches all the grades from think current year. This is for the general grades page.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="plaatsingen"></param>
    /// <param name="leerjaar"></param>
    /// <returns></returns>
    public async Task<SortedSomtodayGradesModel> GetGradesAndVakgemiddelden(user user, SomtodayPlaatsingenModel plaatsingen, string leerjaar = "0")
    {
        #if DEBUG
        var watch = Stopwatch.StartNew();
        #endif
        
        string plaatsingUUID = leerjaar == "0" ? plaatsingen.items[^1].UUID : plaatsingen.items.FirstOrDefault(x => x.stamgroepnaam == leerjaar)?.UUID;
        
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("authorization", "Bearer " + user.somtoday_access_token);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        var response = await _httpClient.GetAsync($"https://passtrough.mjtsgamer.workers.dev/https://api.somtoday.nl/rest/v1/vakkeuzes/plaatsing/{plaatsingUUID}/vakgemiddelden");
        
        var vakgemiddelden = JsonConvert.DeserializeObject<SomtodayVakgemiddeldenModel>(await response.Content.ReadAsStringAsync());
        
        var sortedGrades = new SortedSomtodayGradesModel
        {
            items = new List<Models.SortedSomtodayGradesModel.Item>(),
            lastGrades = new List<Models.SomtodayGradesModel.Item>(),
            voortGangsdossierGemiddelde = vakgemiddelden?.voortgangsdossierGemiddelde,
            relevanteCijferLichtingUUID = vakgemiddelden?.gemiddelden.FirstOrDefault()?.relevanteCijferLichtingUUID,
            leerjaarUUID = plaatsingen.items[^1].UUID,
            onlyVoorVoortgang = true,
            plaatsing = plaatsingen.items
        };

        foreach (var grade in vakgemiddelden.gemiddelden)
        {
            var item = new Models.SortedSomtodayGradesModel.Item
            {
                cijfer = grade.voortgangsdossierResultaat?.formattedResultaat ?? grade.examendossierResultaat?.formattedResultaat ?? "-",
                cijferSE = "-",
                vakNaam = grade.vakNaam,
                vakAfkorting = grade.vakAfkorting,
                vakuuid = grade.vakUUID
            };

            sortedGrades.items.Add(item);
        }
        
        sortedGrades.items = sortedGrades.items.OrderBy(x => x.vakNaam).ToList();
        sortedGrades.leerjaarUUID = plaatsingUUID;
        
        #if DEBUG
        watch.Stop();
        Console.WriteLine(watch.ElapsedMilliseconds + "ms");
        #endif
        
        return sortedGrades;
    }

    /// <summary>
    /// Fetches all the grades from think current year. This is for the general grades page.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="vakUUID">The UUID param inside the vak class inside the somtodayGradesModel</param>
    /// <param name="lichtingUUID">relevanteCijferLichting > UUID</param>
    /// <param name="plaatsingUuid">items[^1] > UUID</param>
    /// <returns></returns>
    public async Task<SortedSomtodayGradesModel> GetGradesFromUUID(user user, string vakUUID, string lichtingUUID, string plaatsingUuid)
    {
        
        //https://api.somtoday.nl/rest/v1/geldendvoortgangsdossierresultaten/vakresultaten/1409824200/vak/{vakUUID}/lichting/{lichtingUUID}?additional=vaknaam&additional=resultaatkolom&additional=naamalternatiefniveau&additional=naamstandaardniveau&additional=periodeAfkorting&type=Toetskolom&type=SamengesteldeToetsKolom&type=Werkstukcijferkolom&type=Advieskolom&type=PeriodeGemiddeldeKolom&type=RapportGemiddeldeKolom&type=RapportCijferKolom&type=RapportToetskolom&type=SEGemiddeldeKolom&type=ToetssoortGemiddeldeKolom&sort=desc-geldendResultaatCijferInvoer&plaatsingUuid={plaatsingUuid}
        //https://api.somtoday.nl/rest/v1/geldendexamendossierresultaten/vakresultaten/1409824200/vak/{vakUUID}/lichting/{lichtingUUID}?additional=vaknaam&additional=resultaatkolom&additional=naamalternatiefniveau&additional=naamstandaardniveau&type=Toetskolom&type=SamengesteldeToetsKolom&type=Werkstukcijferkolom&type=Advieskolom&type=PeriodeGemiddeldeKolom&type=RapportGemiddeldeKolom&type=RapportCijferKolom&type=RapportToetskolom&type=SEGemiddeldeKolom&type=ToetssoortGemiddeldeKolom&sort=desc-geldendResultaatCijferInvoer&plaatsingUuid={plaatsingUuid}
        
#if DEBUG
        var watch = Stopwatch.StartNew();
#endif
        
        var urls = new[]
        {
            //voeg &type=PeriodeGemiddeldeKolom&type=RapportGemiddeldeKolom&type=RapportCijferKolom&type=RapportToetskolom&type=SEGemiddeldeKolom&type=ToetssoortGemiddeldeKolom toe als je de verschillende gemiddelden wilt zien en wilt verwerken in Zermos
            $"https://passtrough.mjtsgamer.workers.dev/https://api.somtoday.nl/rest/v1/geldendvoortgangsdossierresultaten/vakresultaten/{user.somtoday_student_id}/vak/{vakUUID}/lichting/{lichtingUUID}?additional=vaknaam&additional=resultaatkolom&additional=naamalternatiefniveau&additional=naamstandaardniveau&additional=periodeAfkorting&type=Toetskolom&type=SamengesteldeToetsKolom&type=Werkstukcijferkolom&type=Advieskolom&sort=desc-geldendResultaatCijferInvoer&plaatsingUuid={plaatsingUuid}",
            $"https://passtrough.mjtsgamer.workers.dev/https://api.somtoday.nl/rest/v1/geldendexamendossierresultaten/vakresultaten/{user.somtoday_student_id}/vak/{vakUUID}/lichting/{lichtingUUID}?additional=vaknaam&additional=resultaatkolom&additional=naamalternatiefniveau&additional=naamstandaardniveau&type=Toetskolom&type=SamengesteldeToetsKolom&type=Werkstukcijferkolom&type=Advieskolom&sort=desc-geldendResultaatCijferInvoer&plaatsingUuid={plaatsingUuid}"
        };
        
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("authorization", "Bearer " + user.somtoday_access_token);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        
        var tasks = urls.Select(url => url != null ? _httpClient.GetAsync(url) : Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK))).ToArray();
        
        await Task.WhenAll(tasks);
        
        if (tasks.Any(t => !t.Result.IsSuccessStatusCode))
            return null;
        
        var jsonTasks = tasks.Select(t => t.Result.Content.ReadAsStringAsync()).ToArray();
        await Task.WhenAll(jsonTasks);
        
        var grades = JsonConvert.DeserializeObject<SomtodayGradesModel>(jsonTasks[0].Result);
        var gradesSE = JsonConvert.DeserializeObject<SomtodayGradesModel>(jsonTasks[1].Result);
        
        var sortedGrades = new SortedSomtodayGradesModel
        {
            items = new List<Models.SortedSomtodayGradesModel.Item>(),
            lastGrades = new List<Models.SomtodayGradesModel.Item>(),
            voortGangsdossierGemiddelde = null,
            relevanteCijferLichtingUUID = lichtingUUID,
            leerjaarUUID = plaatsingUuid
        };
        
        var item = new Models.SortedSomtodayGradesModel.Item
        {
            cijfers = grades.items,
            weging = grades.items.Sum(x => x.weging),
            cijfer = grades.items.FirstOrDefault()?.formattedResultaat ?? "-",
            cijfersSE = gradesSE.items,
            wegingSE = gradesSE.items.Sum(x => x.weging),
            cijferSE =gradesSE.items.FirstOrDefault()?.formattedResultaat ?? "-",
            vakNaam = grades.items.FirstOrDefault()?.additionalObjects.vaknaam,
            vakAfkorting = "?", 
            vakuuid = grades.items.FirstOrDefault()?.additionalObjects.vakuuid,
        };
        
        sortedGrades.items.Add(item);
        
        sortedGrades.relevanteCijferLichtingUUID = lichtingUUID;
        sortedGrades.leerjaarUUID = plaatsingUuid;
        sortedGrades.items[0].vakuuid = vakUUID;
        
#if DEBUG
        watch.Stop();
        Console.WriteLine(watch.ElapsedMilliseconds + "ms");
#endif
        
        return sortedGrades;
    }
    
    public async Task<SomtodayPlaatsingenModel> GetPlaatsingen(user user)
    {
        //var baseurl = $"https://api.somtoday.nl/rest/v1/plaatsingen?leerling={user.somtoday_student_id}";
        var baseurl = $"https://passtrough.mjtsgamer.workers.dev/https://api.somtoday.nl/rest/v1/plaatsingen?leerling={user.somtoday_student_id}";
        //var baseurl = $"https://somtoday-leerjaar.mjtsgamer.workers.dev?studentId={user.somtoday_student_id}&accessToken={user.somtoday_access_token}";
        
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("authorization", "Bearer " + user.somtoday_access_token);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        
        var response = await _httpClient.GetAsync(baseurl);
        
        if (response.IsSuccessStatusCode == false)
            return null;
        
        return JsonConvert.DeserializeObject<SomtodayPlaatsingenModel>(await response.Content.ReadAsStringAsync());
    }
    
    public async Task<SomtodayHomeworkModel> GetHomeworkAsync(user user, int dagen)
    {
        #if DEBUG
        var watch = Stopwatch.StartNew();
        #endif
        
        var urls = new[]
        {
            $"https://passtrough.mjtsgamer.workers.dev/https://api.somtoday.nl/rest/v1/studiewijzeritemafspraaktoekenningen?begintNaOfOp={DateTime.Now.AddDays(-dagen):yyyy-MM-dd}&additional=swigemaaktVinkjes",
            $"https://passtrough.mjtsgamer.workers.dev/https://api.somtoday.nl/rest/v1/studiewijzeritemdagtoekenningen?schooljaar=&begintNaOfOp={DateTime.Now.AddDays(-dagen):yyyy-MM-dd}&additional=swigemaaktVinkjes",
            $"https://passtrough.mjtsgamer.workers.dev/https://api.somtoday.nl/rest/v1/studiewijzeritemweektoekenningen?schooljaar=&begintNaOfOp={DateTime.Now.AddDays(-dagen):yyyy-MM-dd}&additional=swigemaaktVinkjes"
        };
        
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("authorization", "Bearer " + user.somtoday_access_token);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        
        var tasks = urls.Select(url => url != null ? _httpClient.GetAsync(url) : Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK))).ToArray();
        
        await Task.WhenAll(tasks);
        
        if (tasks.Any(t => !t.Result.IsSuccessStatusCode))
            return null;
        
        var jsonTasks = tasks.Select(t => t.Result.Content.ReadAsStringAsync()).ToArray();
        await Task.WhenAll(jsonTasks);
        
        var homework = JsonConvert.DeserializeObject<SomtodayHomeworkModel>(jsonTasks[0].Result);
        var homeworkDay = JsonConvert.DeserializeObject<SomtodayHomeworkModel>(jsonTasks[1].Result);
        var homeworkWeek = JsonConvert.DeserializeObject<SomtodayHomeworkModel>(jsonTasks[2].Result);
        
        homework.items.AddRange(homeworkDay.items);
        homework.items.AddRange(homeworkWeek.items);
        
        homework.items = homework.items.OrderBy(x => x.datumTijd).ToList();
        
        // //https://somtoday-huiswerk.mjtsgamer.workers.dev/?accessToken=eyJ4NXQjUzI1NiI6IkdpZ295b0kyZXcxQS00TDUweGoyWGlPdXIxdE9BMFo3M05mYmZuQXFkU3ciLCJraWQiOiJpcmlkaXVtaWRwLTE2NjgzMzc3ODYzMTA4ODY0NzQwNTkwOTk4NzcyNDAzMjI1MTM0NSIsInR5cCI6ImF0K2p3dCIsImFsZyI6IlJTMjU2In0.eyJzdWIiOiJjMjNmYmI5OS1iZTRiLTRjMTEtYmJmNS01N2U3ZmM0ZjQzODhcXDM3NWQyMTNmLThmYjYtNGZlNC1iMzM2LWU5MmY3YTIyYzVkZSIsImFtciI6InB3ZCIsImlzcyI6Imh0dHBzOi8vc29tdG9kYXkubmwiLCJ0eXBlIjoiYWNjZXNzIiwiY2xpZW50X2lkIjoic29tdG9kYXktbGVlcmxpbmctd2ViIiwiYXVkIjoiaHR0cHM6Ly9zb210b2RheS5ubCIsIm5iZiI6MTcyNjY4MDY4MCwic2NvcGUiOiJvcGVuaWQiLCJjbGFpbXMiOnsiaWRfdG9rZW4iOnsib3JnbmFtZSI6bnVsbCwiYWZmaWxpYXRpb24iOnsidmFsdWVzIjpbInN0dWRlbnQiLCJwYXJlbnQvZ3VhcmRpYW4iXX0sImxlZXJsaW5nZW4iOm51bGwsImdpdmVuX25hbWUiOm51bGx9fSwiZXhwIjoxNzI2Njg0MjgwLCJpYXQiOjE3MjY2ODA2ODAsImp0aSI6IjBiNjU4YzY0LWQ1YmUtNDhmZi04Njg2LWJlY2RjM2U1ZDhjMDoxODg1MzMyNDE3MzUwMCJ9.bcyL7JQbtV34EMkLXfzkx_53j3NvlSFXpn4hfZd8puHJn-y8sdJ80QK04JvK5_hlzX8zuf3kmbaHV7aqz-m6C3JbWfvrjmSGfVhLst2PKJAAbPf-C4wrhZo00J7oNO0SpuJiTOwxcQlD8fUzGUF9Zut65_NP9TwDNwkS3KVMkKqSXsc33doFsrlxhBoKbWTe9kKYFo8-KapSNZsNbb2pghLbHWGQffibxCDuvEzjs9_OpOHgDrmqLhnvzjDifa3oJHPOHTWXHBkJ0y9LVYIV-OuWFh530D5FYph9kmyOQPoBkbfJYFmiBsRmjBG4wFHzRuRJoTfQuYKeqBtra--TkQ&dagen=21
        // var url = $"https://somtoday-huiswerk.mjtsgamer.workers.dev/?accessToken={user.somtoday_access_token}&dagen={dagen}";
        //
        // _httpClient.DefaultRequestHeaders.Clear();
        // _httpClient.DefaultRequestHeaders.Add("authorization", "Bearer " + user.somtoday_access_token);
        // _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        //
        // var response = await _httpClient.GetAsync(url);
        //
        // if (response.IsSuccessStatusCode == false)
        //     return null;
        //
        // var homework = JsonConvert.DeserializeObject<SomtodayHomeworkModel>(await response.Content.ReadAsStringAsync());
        
        #if DEBUG
        watch.Stop();
        Console.WriteLine(watch.ElapsedMilliseconds + "ms");
        #endif
        
        return homework;
    }

    public async Task<SomtodayRoosterModel> GetRoosterAsync(user user, string year, string week)
    {
#if DEBUG
        var watch = Stopwatch.StartNew();
#endif
        string url = $"https://passtrough.mjtsgamer.workers.dev/https://api.somtoday.nl/rest/v1/afspraakitems/{user.somtoday_student_id}/jaar/{year}/week/{week}";
        //string url = $"https://somtoday-rooster.mjtsgamer.workers.dev/?studentId={user.somtoday_student_id}&accessToken={user.somtoday_access_token}&week={week}&year={year}";

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("authorization", "Bearer " + user.somtoday_access_token);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        
        var response = await _httpClient.GetAsync(url);
        
        if (response.IsSuccessStatusCode == false)
            return null;
        
        var afspraakItems = JsonConvert.DeserializeObject<SomtodayRoosterModel>(await response.Content.ReadAsStringAsync());
        
        var timestamps = user.zermelo_timestamps ?? "08:00-17:00";
        timestamps = (timestamps == "-" ? "08:00-17:00" : timestamps);
        var start = timestamps.Split('-')[0].Split(':');
        var end = timestamps.Split('-')[1].Split(':');
        var secondsStart = int.Parse(start[0]) * 3600 + int.Parse(start[1]) * 60;
        var secondsEnd = int.Parse(end[0]) * 3600 + int.Parse(end[1]) * 60;
            
        //"[28800, 61200]" -> 08:00 - 17:00 in seconds
        afspraakItems.MondayOfAppointmentsWeek = DateTimeUtils.GetMondayOfWeekAndYear(week, year);
        afspraakItems.timeStamps = new List<int> {secondsStart, secondsEnd};
        
#if DEBUG
        watch.Stop();
        Console.WriteLine(watch.ElapsedMilliseconds + "ms");
#endif
        return afspraakItems;
    }

    public async Task<SomtodayStudentModel> GetSomtodayStudent(user user)
    {
        var baseurl = "https://passtrough.mjtsgamer.workers.dev/https://api.somtoday.nl/rest/v1/leerlingen"; //"?additional=pasfoto";

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("authorization", "Bearer " + user.somtoday_access_token);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        var response = await _httpClient.GetAsync(baseurl);
        
        if (response.IsSuccessStatusCode == false)
            return null;
        
        return JsonConvert.DeserializeObject<SomtodayStudentModel>(await response.Content.ReadAsStringAsync());
    }

    public async Task<bool> SetHomeworkStateAsync(user user, string swiToekenningId, bool afvinken)
    {
        var baseurl = "https://passtrough.mjtsgamer.workers.dev/https://api.somtoday.nl/rest/v1/swigemaakt/cou";
        
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("authorization", "Bearer " + user.somtoday_access_token);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        
        var body = new
        {
            leerling = new
            {
                links = new[]
                {
                    new
                    {
                        id = user.somtoday_student_id,
                        rel = "self",
                        type = "leerling.RLeerlingPrimer"
                    }
                }
            },
            swiToekenningId = swiToekenningId,
            gemaakt = afvinken
        };
        
        var response = await _httpClient.PutAsync(baseurl, new StringContent(JsonConvert.SerializeObject(body), System.Text.Encoding.UTF8, "application/json"));
        
        //if 201 or 200, then it was successful
        if (response.IsSuccessStatusCode == false)
            Console.WriteLine("Error: " + await response.Content.ReadAsStringAsync());
        
        return response.IsSuccessStatusCode;
    }
}