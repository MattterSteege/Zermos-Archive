using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class Schedule : BetterHttpClient
{
    public List<Appointment> TodaysScheduledAppointments;

    [SerializeField, Tooltip("'*' means Application.persistentDataPath.")]
    private string savePath = "*/Lessons.json";

    public List<Appointment> GetScheduleOfDay(DateTime date, bool shouldRefreshFile)
    {
        int weeknumber = GetweeknumberFromDate(date);

        Items schedule = GetSchedule(weeknumber.ToString(), date.Year.ToString(), shouldRefreshFile);

        if (schedule == null)
        {
            return null;
        }

        List<Appointment> TodaySchedule = new List<Appointment>();

        foreach (Appointment appointment in schedule.appointments)
        {
            if (appointment.start >= ((DateTimeOffset) date).ToUnixTimeSeconds() &&
                appointment.start <= ((DateTimeOffset) date.AddDays(1)).ToUnixTimeSeconds())
            {
                TodaySchedule.Add(appointment);
            }
        }

        if (date == TimeManager.Instance.DateTime)
        {
            TodaysScheduledAppointments = TodaySchedule;
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
            return DownloadLessons(week, year)?.response.data[0] ?? new Items{appointments = new List<Appointment>()};
        }

        using (StreamReader r = new StreamReader(destination))
        {
            string json = r.ReadToEnd();
            var scheduleObject = JsonConvert.DeserializeObject<Items>(json);
            if ((scheduleObject?.laatsteWijziging.ToDateTime().AddMinutes(10) < TimeManager.Instance.CurrentDateTime && tries < 2) || (ShouldRefreshFile && scheduleObject?.laatsteWijziging.ToDateTime().AddSeconds(15) < TimeManager.Instance.CurrentDateTime && tries < 2))
            {
                r.Close();
                Debug.LogWarning("Local file is outdated, downloading new file.");
                return DownloadLessons(week, year)?.response.data[0] ?? new Items{appointments = new List<Appointment>()};
            }
            return scheduleObject;
        }
    }

    [SerializeField] private int weeksSaved = 5;
    private ZermeloSchedule DownloadLessons(string week, string year)
    {
        if (LocalPrefs.GetString("zermelo-access_token") == null || LocalPrefs.GetString("zermelo-user_code") == null)
            return null;

        ZermeloSchedule schedule = new ZermeloSchedule {response = new Response {data = new List<Items> {new() {appointments = new List<Appointment>()}}}};
        for (int i = 0; i < weeksSaved; i++)
        {
            //check if week plus i is bigger than 52, if so, add 1 to year
            int weeknumber = int.Parse(week) + i;
            if (weeknumber > 52)
            {
                weeknumber -= 52;
                year = (int.Parse(year) + 1).ToString();
            }
            
            if (weeknumber.ToString().ToCharArray().Length == 1)
                week = "0" + weeknumber;

            string baseURL = $"https://ccg.zportal.nl/api/v3/liveschedule" +
                             $"?access_token={LocalPrefs.GetString("zermelo-access_token")}" +
                             $"&student={LocalPrefs.GetString("zermelo-user_code")}" +
                             $"&week={year + week}";


            var scheduleResponse = (ZermeloSchedule) Get(baseURL, callback => JsonConvert.DeserializeObject<ZermeloSchedule>(callback.downloadHandler.text));

            if (scheduleResponse != null)
            {
                schedule.response.data[0].appointments.AddRange(scheduleResponse.response.data[0].appointments);
            }
        }

        var convertedJson = JsonConvert.SerializeObject(
            new Items()
            {
                appointments = schedule.response.data[0].appointments,
                laatsteWijziging = TimeManager.Instance.CurrentDateTime.ToUnixTime()
            },
            Formatting.Indented);

        string destination = savePath.Replace("*", Application.persistentDataPath);

        File.WriteAllText(destination, $"//In dit bestand staan alle lessen voor de komende {weeksSaved} weken\r\n");
        File.AppendAllText(destination, convertedJson);
        
        return schedule;
    }
    
    public int GetweeknumberFromDate(DateTime date)
    {
        DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(date);

        int weeknumber;

        if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
        {
            date = date.AddDays(3);
            weeknumber =
                CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek,
                    DayOfWeek.Monday);
            date = date.AddDays(-3);
        }
        else
        {
            weeknumber =
                CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek,
                    DayOfWeek.Monday);
        }

        return weeknumber;
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