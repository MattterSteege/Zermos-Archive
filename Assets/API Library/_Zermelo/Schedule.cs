using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class Schedule : BetterHttpClient
{
    public List<Appointment> TodaysScheduledAppointments;

    [SerializeField, Tooltip("'*' means Application.persistentDataPath.")]
    private string savePath = "*/Lessons.json";

    private void Start()
    {
        ViewManager.onInitializeComplete += FetchSchedule;
    }

    private void FetchSchedule(bool done)
    {
        ZermeloSchedule schedule = new ZermeloSchedule();

        int week = GetweeknumberFromDate(TimeManager.Instance.DateTime);
        schedule = GetSchedule(TimeManager.Instance.DateTime.Year + week.ToString());

        for (int i = 1; i < 5; i++)
        {
            string year = "202201";

            if (week + i > 52)
            {
                year = (TimeManager.Instance.DateTime.Year + 1) + (week + i - 52).ToString("00");
            }
            else
            {
                year = TimeManager.Instance.DateTime.Year + (week + i).ToString();
            }

            schedule.response.data[0].appointments.AddRange(GetSchedule(year).response.data[0].appointments);
        }

        SaveFile(schedule);
    }

    DateTime LastFetched;

    public ZermeloSchedule StartGetSchedule(string week, string year)
    {
        if (LastFetched.Year != TimeManager.Instance.DateTime.Year)
        {
            LastFetched = TimeManager.Instance.CurrentDateTime;
        }

        var schedule = LoadFile();

        if (schedule == null && !(LastFetched.AddMinutes(5) < TimeManager.Instance.CurrentDateTime))
        {
            if (Regex.IsMatch(week, "/^(?=.{1,2}$).*/"))
            {
                if (week.ToCharArray().Length == 1)
                {
                    week = "0" + week;
                }

                if (year == "0")
                {
                    year = TimeManager.Instance.DateTime.Year.ToString();
                }
            }

            LastFetched = TimeManager.Instance.DateTime;
            return GetSchedule(year + week);
        }

        return schedule;
    }

    private ZermeloSchedule GetSchedule(string date)
    {
        if (LocalPrefs.GetString("zermelo-school_code") == null || LocalPrefs.GetString("zermelo-access_token") == null || LocalPrefs.GetString("zermelo-user_code") == null)
            return null;

        string baseURL = $"https://{LocalPrefs.GetString("zermelo-school_code")}.zportal.nl/api/v3/liveschedule" +
                         $"?access_token={LocalPrefs.GetString("zermelo-access_token")}" +
                         $"&student={LocalPrefs.GetString("zermelo-user_code")}" +
                         $"&week={date}";

        return (ZermeloSchedule) Get(baseURL, callback =>
        {
            var schedule = JsonConvert.DeserializeObject<ZermeloSchedule>(callback.downloadHandler.text);
            SaveFile(schedule);
            return schedule;
        }, 
    error =>
        {
            Debug.Log(error);
            return null;
        });
    }

    #region saving and loading the latest schedule

    private void SaveFile(ZermeloSchedule schedule)
    {
        string path = savePath.Replace("*", Application.persistentDataPath);
        string json = JsonConvert.SerializeObject(schedule, Formatting.Indented);
        File.WriteAllText(path, "//In dit bestand staan alle zelf aangemaakte huiswerk items.\r\n");
        File.AppendAllText(path, json);
    }

    private ZermeloSchedule LoadFile()
    {
        string destination = savePath.Replace("*", Application.persistentDataPath);

        if (!File.Exists(destination))
        {
            Debug.LogWarning("File not found");
            return null;
        }

        using (StreamReader r = new StreamReader(destination))
        {
            string json = r.ReadToEnd();
            ZermeloSchedule schedule = JsonConvert.DeserializeObject<ZermeloSchedule>(json);

            //might need to sort by date, if not already sorted

            return schedule;
        }
    }

    #endregion

    public List<Appointment> getScheduleOfDay(DateTime date)
    {
        int weeknumber = GetweeknumberFromDate(date);

        ZermeloSchedule schedule = StartGetSchedule(weeknumber.ToString(), date.Year.ToString());

        if (schedule == null)
        {
            return null;
        }

        List<Appointment> TodaySchedule = new List<Appointment>();

        foreach (Appointment appointment in schedule.response.data[0].appointments)
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

    public class Datum
    {
        public string week { get; set; }
        public string user { get; set; }
        public List<Appointment> appointments { get; set; }
        public List<Status> status { get; set; }
        public List<object> replacements { get; set; }
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
        public List<Datum> data { get; set; }
    }

    public class Status
    {
        public int code { get; set; }
        public string nl { get; set; }
        public string en { get; set; }
    }

    #endregion
}