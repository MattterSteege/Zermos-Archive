using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Org.BouncyCastle.Tls;
using Zermos_Web.APIs;
using Zermos_Web.Models.Requirements;
using Zermos_Web.Utilities;

namespace Zermos_Web.Controllers;

public class RecapController : BaseController
{
    public RecapController(Users user, Shares share, ILogger<BaseController> logger) : base(user, share, logger) { }

    SomtodayAPI somtodayApi = new(new HttpClient());
    ZermeloApi zermeloApi = new(new HttpClient());
    InfowijsApi infowijsApi = new(new HttpClient());

    // GET
    [Authorize]
    [ZermosPage]
    [Route("/Recap")]
    public async Task<IActionResult> Index()
    {
        Log(LogLevel.Information, "Recap page visited by " + ZermosEmail);
        return await Builder();
    }

    [Authorize]
    [ZermosPage]
    public async Task<IActionResult> Builder()
    {
        RecapModel model = new() { };
        
        var user = ZermosUser;
        
        #region roosterRecap

        if (user.zermelo_access_token != null)
        {

            DateTime dateTime2023Week34 = new DateTime(2023, 8, 21, 0, 0, 0);
            DateTime dateTime2024Week29 = new DateTime(2024, 7, 15, 0, 0, 0);

            var ZermeloRooster =
                await zermeloApi.getRoosterFromStartAndEnd(user, dateTime2023Week34, dateTime2024Week29);

            // Calculate total number of classes
            int totalClasses = int.Parse(ZermeloRooster.response.totalRows);

            // Types of classes
            var groupedData = ZermeloRooster.response.data.GroupBy(x => x.type).ToList();
            int canceledClasses = groupedData.FirstOrDefault(g => g.Key == "lesson")?.Count(x => x.cancelled) ?? 0;
            int examClasses = groupedData.FirstOrDefault(g => g.Key == "exam")?.Count() ?? 0;
            int activityClasses = groupedData.FirstOrDefault(g => g.Key == "activity")?.Count() ?? 0;
            int normalClasses = groupedData.FirstOrDefault(g => g.Key == "lesson")?.Count(x => !x.cancelled) ?? 0;

            // Wierd lesson duration classes
            int wierdLessonDurationClasses =
                groupedData.FirstOrDefault(g => g.Key == "lesson")?.Count(x => x.end - x.start != 50 * 60) ?? 0;

            // Time in class
            int timeInClass = groupedData.FirstOrDefault(g => g.Key == "lesson")?.Sum(x =>
                x.end - x.start - (x.type == "exam" ? 60 * 60 : x.type == "activity" ? 50 * 60 : 0)) ?? 0;
            int hoursInClass = timeInClass / 3600;
            int minutesInClass = (timeInClass / 60) % 60;

            // Most canceled class
            var canceledSubjects = ZermeloRooster.response.data.Where(x => x.cancelled).SelectMany(x => x.subjects)
                .GroupBy(x => x).OrderByDescending(g => g.Count()).ToList();
            var mostCanceledSubject = canceledSubjects.FirstOrDefault()?.Key;
            var totalCanceledClasses = canceledSubjects.FirstOrDefault()?.Count();

            //most used classroom
            var classrooms = ZermeloRooster.response.data.FindAll(x => !x.cancelled).SelectMany(x => x.locations)
                .GroupBy(x => x).OrderByDescending(g => g.Count()).ToList();
            var mostUsedClassroom = classrooms.FirstOrDefault()?.Key;
            var mostUsedClassroomCount = classrooms.FirstOrDefault()?.Count();

            //most used teacher
            var teachers = ZermeloRooster.response.data.FindAll(x => !x.cancelled).SelectMany(x => x.teachers)
                .GroupBy(x => x).OrderByDescending(g => g.Count()).ToList();
            var topTeacher = teachers.FirstOrDefault()?.Key;
            var topTeacherUsed =
                ZermeloRooster.response.data.Count(x => x.teachers.Contains(topTeacher) && x.cancelled == false);

            //most used subject
            var subjects = ZermeloRooster.response.data.FindAll(x => !x.cancelled).SelectMany(x => x.subjects)
                .GroupBy(x => x).OrderByDescending(g => g.Count()).ToList();
            var topSubject = subjects.FirstOrDefault()?.Key;
            var topSubjectUsed =
                ZermeloRooster.response.data.Count(x => x.subjects.Contains(topSubject) && x.cancelled == false);

            //cancel ratio
            var cancelRatio = canceledClasses / (double) totalClasses;

            model.Rooster = new Rooster
            {
                totalClasses = totalClasses,
                canceledClasses = canceledClasses,
                examClasses = examClasses,
                activityClasses = activityClasses,
                normalClasses = normalClasses,
                wierdLessonDurationClasses = wierdLessonDurationClasses,
                hoursInClass = hoursInClass,
                minutesInClass = minutesInClass,
                mostCanceledSubject = mostCanceledSubject,
                totalCanceledClasses = totalCanceledClasses ?? 0,
                mostUsedClassroom = mostUsedClassroom,
                mostUsedClassroomCount = mostUsedClassroomCount ?? 0,
                topTeacher = topTeacher,
                topTeacherUsed = topTeacherUsed,
                totalTeachers = teachers.Count,
                topSubject = topSubject,
                topSubjectUsed = topSubjectUsed,
                totalSubjects = subjects.Count,
                cancelRatio = cancelRatio
            };
        }
        #endregion

        #region CijfersRecap
        if (TokenUtils.CheckToken(user.somtoday_access_token) == false)
        {
            user.somtoday_access_token = await RefreshToken(user.somtoday_refresh_token);
        }

        if (user.somtoday_access_token != null)
        {

            var SomtodayGrades = await somtodayApi.GetGrades(user);
            var SomtodayAverage = (await somtodayApi.Getvakgemiddelden(user, -1)).voortgangsdossierGemiddelde;

            // Calculate total number of grades
            int totalGrades = SomtodayGrades.items.Count;

            // Calculate total number of voldoendes
            int totalVoldoendes = SomtodayGrades.items.Count(x => NumberUtils.ParseFloat(x.geldendResultaat) >= 5.5);
            int totalOnvoldoendes = totalGrades - totalVoldoendes;

            // Calculate highest grade
            var topGrade = SomtodayGrades.items.MaxBy(x => NumberUtils.ParseFloat(x.geldendResultaat));
            var topSubjectsGrade = topGrade.geldendResultaat;
            var topSubjectSubject = topGrade.vak.naam;

            // calculate lowest grade
            var worstGrade = SomtodayGrades.items.MinBy(x => NumberUtils.ParseFloat(x.geldendResultaat));
            var worstSubjectsGrade = worstGrade.geldendResultaat;
            var worstSubjectSubject = worstGrade.vak.naam;

            // calculate most common rounded grade (7, 8, etc)
            var mostCommonGrade = SomtodayGrades.items
                .GroupBy(x => Math.Round(NumberUtils.ParseFloat(x.geldendResultaat))).MaxBy(g => g.Count());
            var mostCommonGradeGrade = mostCommonGrade.Key;
            var mostCommonGradeCount = mostCommonGrade.Count();

            // Calculate total points
            var totalPoints = SomtodayGrades.items.Sum(x =>
                (NumberUtils.ParseFloat(x.geldendResultaat) * (x.weging == 0 ? x.examenWeging : x.weging)));

            model.Cijfers = new Cijfers
            {
                totalGrades = totalGrades,
                totalVoldoendes = totalVoldoendes,
                totalOnvoldoendes = totalOnvoldoendes,
                topSubjectsGrade = topSubjectsGrade,
                topSubjectSubject = topSubjectSubject,
                worstSubjectsGrade = worstSubjectsGrade,
                worstSubjectSubject = worstSubjectSubject,
                mostCommonGradeGrade = mostCommonGradeGrade,
                mostCommonGradeCount = mostCommonGradeCount,
                totalPoints = totalPoints,
                averageGrade = SomtodayAverage
            };
        }

        #endregion
        
        return PartialView("Index", model);
    }
    
    [NonAction]
    public async Task<string> RefreshToken(string token = null)
    {
        if (token == null) return null;
            
        var somtoday = await somtodayApi.RefreshTokenAsync(token);
            
        ZermosUser = new user
        {
            somtoday_access_token = somtoday.access_token,
            somtoday_refresh_token = somtoday.refresh_token
        };
            
        return somtoday.access_token;
    }
}

public class Cijfers
{
    public int totalGrades { get; set; }
    public int totalVoldoendes { get; set; }
    public int totalOnvoldoendes { get; set; }
    public string topSubjectsGrade { get; set; }
    public string topSubjectSubject { get; set; }
    public string worstSubjectsGrade { get; set; }
    public string worstSubjectSubject { get; set; }
    public double mostCommonGradeGrade { get; set; }
    public int mostCommonGradeCount { get; set; }
    public double totalPoints { get; set; }
    public double averageGrade { get; set; }
}

public class Rooster
{
    public int totalClasses { get; set; }
    public int canceledClasses { get; set; }
    public int examClasses { get; set; }
    public int activityClasses { get; set; }
    public int normalClasses { get; set; }
    public int wierdLessonDurationClasses { get; set; }
    public int hoursInClass { get; set; }
    public int minutesInClass { get; set; }
    public string mostCanceledSubject { get; set; }
    public int totalCanceledClasses { get; set; }
    public string mostUsedClassroom { get; set; }
    public int mostUsedClassroomCount { get; set; }
    public string topTeacher { get; set; }
    public int topTeacherUsed { get; set; }
    public int totalTeachers { get; set; }
    public string topSubject { get; set; }
    public int topSubjectUsed { get; set; }
    public int totalSubjects { get; set; }
    public double cancelRatio { get; set; }
}

public class RecapModel
{
    public Cijfers Cijfers { get; set; }
    public Rooster Rooster { get; set; }
}