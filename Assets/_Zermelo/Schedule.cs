using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class Schedule : MonoBehaviour
{
    public static List<Appointment> TodaysScheduledAppointments;
    
    
    public ZermeloSchedule StartGetSchedule(string week = "38", string year = "0")
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

        
        return new CoroutineWithData<ZermeloSchedule>(this, GetSchedule(year + week)).result;
    }

    private IEnumerator GetSchedule(string date)
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("zermelo-user_code")))
        {
            GetComponent<User>().startGetUser();
            yield break; 
            
        }

        string baseURL = "https://{school}.zportal.nl/api/v3/liveschedule?access_token={access_token}&student={student}&week={week}";
        
        baseURL = baseURL.Replace("{school}", PlayerPrefs.GetString("zermelo-school_code"));
        baseURL = baseURL.Replace("{access_token}", PlayerPrefs.GetString("zermelo-access_token"));
        baseURL = baseURL.Replace("{student}", PlayerPrefs.GetString("zermelo-user_code"));
        baseURL = baseURL.Replace("{week}", date);

        UnityWebRequest www = HttpRequest(baseURL);
 
        if(www.result != UnityWebRequest.Result.Success) 
        {
            Debug.Log(www.error);
        }
        else 
        {
            yield return JsonConvert.DeserializeObject<ZermeloSchedule>(www.downloadHandler.text);
        }
    }
    
    private UnityWebRequest HttpRequest(string baseURL)
    {
        //print(baseURL);
        UnityWebRequest www = UnityWebRequest.Get(baseURL);
        www.SendWebRequest();

        while (!www.isDone) { }
        
        return www;
    }

    [ContextMenu("get todays items")]
    public void testgetScheduleOfDay()
    {
        getScheduleOfDay(new DateTime(2022, 9, 19, 0, 0, 0));
    }
    
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

        return TodaySchedule;
    }

    public int GetweeknumberFromDate(DateTime date)
    {
        DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(date);

        int weeknumber;
            
        if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
        {
            date = date.AddDays(3);
            weeknumber = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            date = date.AddDays(-3);
        }
        else
        {
            weeknumber = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
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
