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
        user user = ZermosUser;
        var SomtodayGrades = await somtodayApi.GetGrades(user);
        
        DateTime dateTime2023Week34 = new DateTime(2023, 8, 21, 0, 0, 0);
        DateTime dateTime2024Week29 = new DateTime(2024, 7, 15, 0, 0, 0);
        
        var ZermeloRooster = await zermeloApi.getRoosterFromStartAndEnd(user, dateTime2023Week34, dateTime2024Week29);

        //The following slides will be shown in the recap:
        //cijfers
        /*
         
         Dit jaar bleven ook je cijfers niet achter, en je bent uiteindelijke met een {gemiddeldCijfer} geeindigd...
            ===========================================================
            Je hebt in totaal {totalGrades} cijfers gehaald, waarvan {totalVoldoendes} voldoendes en {totalOnvoldoendes} onvoldoendes.
            ===========================================================
            Het hoogste cijfer dat je hebt gehaald was een {highestGrade} en het laagste cijfer was een {lowestGrade}.
            ===========================================================
            De meest voorkomende afgeronde cijfer was: {topGrade}
            ===========================================================
            De beste vakken waren: {topSubjects}
            De slechtste vakken waren: {worstSubjects}
            ===========================================================
            Je hebt in totaal {totalPoints} punten gehaald, en dat is een gemiddelde van {averagePoints} punten per cijfer.
            ===========================================================
            En dan nu, 2 leugens en 1 waarheid:
            1. {lie1}
            2. {lie2}
            3. {truth}
            
         
         */
        
        //rooster
        /*
         
             Dit schooljaar was weer wild!
            ===========================================================
            Je hebt {totalClasses} lessen gehad, waarvan er {canceledClasses} uitvielen.
            Daardoor heb je een grande totaal van {hoursInClass} uur en {minutesInClass} minuten in de les gezeten.
            ===========================================================
            Je hebt {examClasses} examens gehad en {activityClasses} activiteiten.
            Dan blijven er nog {normalClasses} normale lessen over.
            ===========================================================
            Er waren dit jaar {wierdLessonDurationClasses} lessen die niet 50 minuten duurden.
            ===========================================================
            Je hebt met al die lessen natuurlijk ook veel verschillende docenten gehad.
            In totaal heb je {totalTeachers} verschillende docenten gehad.
            En de meest voorkomende docenten waren: {topTeachers}
            De meest voorkomende vakken waren: {topSubjects}
            Het vak dat het meeste uitviel was: {mostCanceledSubject} met {totalCanceledClasses} uitgevallen lessen. Dat betekent een wel/niet ratio van {cancelRatio} / percentage van {cancelPercentage}%
            ===========================================================
            Ook de lokalen blijven niet achter.
            In totaal heb je {totalClassrooms} verschillende lokalen gehad.
            En de meest voorkomende lokalen waren: {topClassrooms}
            Het lokaal dat het meeste gebruikt werd was: {mostUsedClassroom}, je hebt hier {mostUsedClassroomCount} lessen doorgebracht.

         
         */

        #region roosterRecap
        // Calculate total number of classes
        int totalClasses = int.Parse(ZermeloRooster.response.totalRows);

        // Types of classes
        var groupedData = ZermeloRooster.response.data.GroupBy(x => x.type).ToList();
        int canceledClasses = groupedData.FirstOrDefault(g => g.Key == "lesson")?.Count(x => x.cancelled) ?? 0;
        int examClasses = groupedData.FirstOrDefault(g => g.Key == "exam")?.Count() ?? 0;
        int activityClasses = groupedData.FirstOrDefault(g => g.Key == "activity")?.Count() ?? 0;
        int normalClasses = groupedData.FirstOrDefault(g => g.Key == "lesson")?.Count(x => !x.cancelled) ?? 0;

        // Wierd lesson duration classes
        int wierdLessonDurationClasses = groupedData.FirstOrDefault(g => g.Key == "lesson")?.Count(x => x.end - x.start != 50 * 60) ?? 0;

        // Time in class
        int timeInClass = groupedData.FirstOrDefault(g => g.Key == "lesson")?.Sum(x => x.end - x.start - (x.type == "exam" ? 60 * 60 : x.type == "activity" ? 50 * 60 : 0)) ?? 0;
        int hoursInClass = timeInClass / 3600;
        int minutesInClass = (timeInClass / 60) % 60;

        // Most canceled class
        var canceledSubjects = ZermeloRooster.response.data.Where(x => x.cancelled).SelectMany(x => x.subjects).GroupBy(x => x).OrderByDescending(g => g.Count()).ToList();
        var mostCanceledSubject = canceledSubjects.FirstOrDefault()?.Key;
        var totalCanceledClasses = canceledSubjects.FirstOrDefault()?.Count();

        //most used classroom
        var classrooms = ZermeloRooster.response.data.SelectMany(x => x.locations).GroupBy(x => x).OrderByDescending(g => g.Count()).ToList();
        var mostUsedClassroom = classrooms.FirstOrDefault()?.Key;
        var mostUsedClassroomCount = classrooms.FirstOrDefault()?.Count();
        
        //most used teacher
        var teachers = ZermeloRooster.response.data.SelectMany(x => x.teachers).GroupBy(x => x).OrderByDescending(g => g.Count()).ToList();
        var topTeacher = teachers.FirstOrDefault()?.Key;
        var totalTeachers = teachers.FirstOrDefault()?.Count();
        
        //most used subject
        var subjects = ZermeloRooster.response.data.SelectMany(x => x.subjects).GroupBy(x => x).OrderByDescending(g => g.Count()).ToList();
        var topSubject = subjects.FirstOrDefault()?.Key;
        var totalSubjects = subjects.FirstOrDefault()?.Count();
        
        //cancel ratio
        var cancelRatio = canceledClasses / (double) totalClasses;
        #endregion

        #region CijfersRecap
        // Calculate total number of grades
        int totalGrades = SomtodayGrades.items.Count;
        
        // Calculate total number of voldoendes
        int totalVoldoendes = SomtodayGrades.items.Count(x => NumberUtils.ParseFloat(x.geldendResultaat) >= 5.5);
        int totalOnvoldoendes = totalGrades - totalVoldoendes;
        
        // Calculate highest grade
        var topGrade = SomtodayGrades.items.MaxBy(x => Math.Round(NumberUtils.ParseFloat(x.geldendResultaat)));
        var topSubjectsGrade = topGrade.geldendResultaat;
        var topSubjectSubject = topGrade.vak.naam;

        // calculate lowest grade
        var worstGrade = SomtodayGrades.items.MinBy(x => Math.Round(NumberUtils.ParseFloat(x.geldendResultaat)));
        var worstSubjectsGrade = worstGrade.geldendResultaat;
        var worstSubjectSubject = worstGrade.vak.naam;
        
        // calculate most common rounded grade (7, 8, etc)
        var mostCommonGrade = SomtodayGrades.items.GroupBy(x => Math.Round(NumberUtils.ParseFloat(x.geldendResultaat))).OrderByDescending(g => g.Count()).FirstOrDefault();
        var mostCommonGradeGrade = mostCommonGrade.Key;
        var mostCommonGradeCount = mostCommonGrade.Count();
        
        // Calculate total points
        var totalPoints = SomtodayGrades.items.Sum(x => (NumberUtils.ParseFloat(x.geldendResultaat) * (x.weging == 0 ? x.examenWeging : x.weging)));
        #endregion
        
        dynamic model = new
        {
            Cijfers = new
            {
                totalGrades,
                totalVoldoendes,
                totalOnvoldoendes,
                topSubjectsGrade,
                topSubjectSubject,
                worstSubjectsGrade,
                worstSubjectSubject,
                mostCommonGradeGrade,
                mostCommonGradeCount,
                totalPoints
            },
            Rooster = new
            {
                totalClasses,
                canceledClasses,
                examClasses,
                activityClasses,
                normalClasses,
                wierdLessonDurationClasses,
                hoursInClass,
                minutesInClass,
                mostCanceledSubject,
                totalCanceledClasses,
                mostUsedClassroom,
                mostUsedClassroomCount,
                topTeacher,
                totalTeachers,
                topSubject,
                totalSubjects,
                cancelRatio
            }
        };
        
        
        return PartialView(model);
    }
}