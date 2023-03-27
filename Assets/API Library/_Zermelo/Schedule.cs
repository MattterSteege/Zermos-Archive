using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class Schedule : BetterHttpClient
{
    [HideInInspector]public ZermeloSchedule TodaysScheduledAppointments;
    [HideInInspector] public DateTime LastUpdatedTodaysScheduledAppointments;
    [HideInInspector] public ZermeloSchedule SavedzermeloSchedule; //this is a place that a random week can be saved to, so every schedule date change will not have to download a new version, only when the week changes
    [HideInInspector] public int weeknumber;


    [SerializeField, Tooltip("'*' means Application.persistentDataPath.")]
    private string savePath = "*/Schedule.json";

    public List<Appointment> GetScheduleOfDay(DateTime date, bool shouldRefreshFile)
    {
        int weeknumber = date.GetWeekNumber();

        if (TodaysScheduledAppointments != null && TodaysScheduledAppointments.response.data[0].appointments.Count != 0 && LastUpdatedTodaysScheduledAppointments.AddMinutes(10) < TimeManager.Instance.CurrentDateTime && shouldRefreshFile == false)
        {
            return TodaysScheduledAppointments.response.data[0].appointments;
        }
        
        Items schedule = GetSchedule(weeknumber.ToString(), date.Year.ToString(), shouldRefreshFile);

        if (schedule == null)
        {
            return null;
        }


        List<Appointment> TodaySchedule = new List<Appointment>();

        foreach (Appointment appointment in schedule.appointments)
        {
            if (appointment.start >= date.ToDayTimeSavingDate().ToUnixTime() &&
                appointment.start <= date.AddDays(1).ToDayTimeSavingDate().ToUnixTime())
            {
                TodaySchedule.Add(appointment);
            }
        }

        return TodaySchedule;
    }

    
    int tries = 0;
    public Items GetSchedule(string week, string year, bool ShouldRefreshFile)
    {
        string destination = savePath.Replace("*", Application.persistentDataPath);

        if (!File.Exists(destination))
        {
            Debug.LogWarning("File not found, creating new file.");
            File.Create(destination).Dispose();
            return DownloadLessons(week, year)?.response.data[0] ?? new Items{appointments = new List<Appointment>()};
        }
        
        if (weeknumber == int.Parse(week))
        {
            return SavedzermeloSchedule.response.data[0];
        }

        using (StreamReader r = new StreamReader(destination))
        {
            string json = r.ReadToEnd();
            var scheduleObject = JsonConvert.DeserializeObject<Items>(json);
            r.Dispose();
            
            if (scheduleObject == null)
            {
                Debug.LogWarning("File is empty, creating new file.");
                return DownloadLessons(week, year)?.response.data[0] ?? new Items{appointments = new List<Appointment>()};
            }

            int currentUnixTime = TimeManager.Instance.CurrentDateTime.ToUnixTime();
            int lastUpdatedUnixTime = scheduleObject.laatsteWijziging;
            bool isSameWeek = TimeManager.Instance.CurrentDateTime.IsSameWeek(DateTimeUtils.GetMondayOfWeekAndYear(week, year));

            TimeSpan timeSinceLastUpdate = TimeSpan.FromSeconds(currentUnixTime - lastUpdatedUnixTime);

            if (timeSinceLastUpdate.TotalMinutes > 10 || ShouldRefreshFile || !isSameWeek)
            {
                if (isSameWeek)
                {
                    if (timeSinceLastUpdate.TotalSeconds < 15 && ShouldRefreshFile)
                        return scheduleObject;
                    
                    if (!ShouldRefreshFile)
                        return scheduleObject;
                    
                }
                return DownloadLessons(week, year)?.response.data[0] ?? new Items {appointments = new List<Appointment>()};
            }
            return scheduleObject;
        }
    }
    
    private ZermeloSchedule DownloadLessons(string week, string year)
    {
        if (LocalPrefs.GetString("zermelo-access_token") == null || LocalPrefs.GetString("zermelo-user_code") == null)
            return null;

        ZermeloSchedule schedule = new ZermeloSchedule {response = new Response {data = new List<Items> {new() {appointments = new List<Appointment>()}}}};

        string date = $"{year}-{week}";
        
        date = year + (week.ToCharArray().Length == 1 ? "0" + week : week);

        string baseURL = $"https://ccg.zportal.nl/api/v3/liveschedule" +
                         $"?access_token={LocalPrefs.GetString("zermelo-access_token")}" +
                         $"&student={LocalPrefs.GetString("zermelo-user_code")}" +
                         $"&week={date}";
        
        var scheduleResponse = (ZermeloSchedule) Get(baseURL, callback => JsonConvert.DeserializeObject<ZermeloSchedule>(callback.downloadHandler.text), (error) =>
        {

            AndroidUIToast.ShowToast("Het rooster kon niet opgehaald, probeer het later opnieuw.");
            return null;
        });

        if (scheduleResponse != null)
        {
            schedule.response.data[0].appointments.AddRange(scheduleResponse.response.data[0].appointments);
        }

        if (DateTimeUtils.GetMondayOfWeekAndYear(week, year) != TimeManager.Instance.CurrentDateTime.GetMondayOfWeek())
        {
            SavedzermeloSchedule = schedule;
            weeknumber = int.Parse(week);
            return schedule;
        }

        TodaysScheduledAppointments = schedule;
        LastUpdatedTodaysScheduledAppointments = TimeManager.Instance.CurrentDateTime;

        var convertedJson = JsonConvert.SerializeObject(
            new Items()
            {
                appointments = schedule.response.data[0].appointments,
                laatsteWijziging = TimeManager.Instance.CurrentDateTime.ToUnixTime()
            },
            Formatting.Indented);

        string destination = savePath.Replace("*", Application.persistentDataPath);

        File.WriteAllText(destination, convertedJson);
        
        return schedule;
    }

    #region models

    public class Action
    {
        public Appointment appointment { get; set; }
        public List<Status> status { get; set; }
        public bool allowed { get; set; }
        public string post { get; set; }
    }

    public class ZermeloSchedule
    {
        public Response response { get; set; }
    }

    public class Appointment
    {
        public List<Status> status { get; set; }
        public List<Action> actions { get; set; }
        public int start { get; set; }
        public int end { get; set; }
        public bool cancelled { get; set; }
        public string appointmentType { get; set; }
        public bool online { get; set; }
        public bool optional { get; set; }
        public int? appointmentInstance { get; set; }
        public string startTimeSlotName { get; set; }
        public string endTimeSlotName { get; set; }
        public List<string> subjects { get; set; }
        public List<string> groups { get; set; }
        public List<string> locations { get; set; }
        public List<string> teachers { get; set; }
        public List<object> onlineTeachers { get; set; }
        public object onlineLocationUrl { get; set; }
        public object capacity { get; set; }
        public object expectedStudentCount { get; set; }
        public object expectedStudentCountOnline { get; set; }
        public string changeDescription { get; set; }
        public string schedulerRemark { get; set; }
        public object content { get; set; }
        public int? id { get; set; }
    }

    public class Items
    {
        public int laatsteWijziging { get; set; }
        public List<Appointment> appointments { get; set; }
    }

    public class Response
    {
        public int status { get; set; }
        public string message { get; set; }
        public string details { get; set; }
        public int eventId { get; set; }
        public int startRow { get; set; }
        public int endRow { get; set; }
        public int totalRows { get; set; }
        public List<Items> data { get; set; }
    }

    public class Status
    {
        public int code { get; set; }
        public string nl { get; set; }
        public string en { get; set; }
    }

    #endregion
}