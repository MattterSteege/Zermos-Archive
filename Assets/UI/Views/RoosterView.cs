using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RoosterView : View
{
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject RoosterPrefab;
    [SerializeField] private GameObject TussenUurPrefab;
    [SerializeField] private Button RefreshButton;
    [SerializeField] private Schedule _schedule;
    [SerializeField] private int _date;
    
    private List<Schedule.Appointment> appointments;
    [SerializeField] private List<int> NoLessonHours;
    [SerializeField] private List<GameObject> RoosterItems;

    [ContextMenu("clear list")]
    public void clearlist()
    {
        RoosterItems.Clear();
    }
    
#if UNITY_EDITOR
    public static DateTime UnixTimeStampToDateTime( double unixTimeStamp )
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds( unixTimeStamp ).ToLocalTime();
        return dateTime;
    }
#endif
    
    public override void Initialize()
    {
        RefreshButton.onClick.AddListener(Initialize);
        
        content.transform.parent.parent.GetComponent<ScrollRect>().horizontal = PlayerPrefs.GetInt("UltraSatisfyingScheduleMode") == 1;

        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
            NoLessonHours.Clear();
            NoLessonHours = new List<int>();
            appointments.Clear();
            RoosterItems.Clear();
        }
        
        NoLessonHours = new List<int>();
        appointments = new List<Schedule.Appointment>();
        RoosterItems = new List<GameObject>();
        
        
#if UNITY_EDITOR
        if(_date != 0)
            appointments = _schedule.getScheduleOfDay(UnixTimeStampToDateTime(_date));
        else
            appointments = _schedule.getScheduleOfDay(DateTime.Today);
#else
        appointments = _schedule.getScheduleOfDay(DateTime.Today);
#endif
        
        if (appointments == null)
        {
            base.Initialize();
            return;
        }
        
        int i = 0;

        foreach (Schedule.Appointment appointment in appointments)
        {
            i++;
            
            try
            {
                if (appointment.id != null)
                {
                    //initialize list<string> named appointments with one value in it
                    List<string> appointments = new List<string> {"No Location(s)"};
                    List<string> teachers = new List<string> {"No Teacher(s)"};
                    List<string> subjects = new List<string> {"No Subject(s)"};

                    if (appointment.locations.Count > 0)
                    {
                        appointments = appointment.locations;
                    }
                    
                    if (appointment.teachers.Count > 0)
                    {
                        teachers = appointment.teachers;
                    }
                    
                    if (appointment.subjects.Count > 0)
                    {
                        subjects = appointment.subjects;
                    }

                    var rooster = Instantiate(RoosterPrefab, content.transform);
                    rooster.GetComponent<AppointmentInfo>().SetAppointmentInfo(string.Join(", ", appointments),
                        DateTimeOffset.FromUnixTimeSeconds(appointment.start).AddHours(2).UtcDateTime.ToShortTimeString() + " - " +
                        DateTimeOffset.FromUnixTimeSeconds(appointment.end).AddHours(2).UtcDateTime.ToShortTimeString(),
                        string.Join(", ", teachers), string.Join(", ", subjects), appointment.startTimeSlotName);
                    rooster.GetComponent<AppointmentInfo>()._appointment = appointment;
                    
                    RoosterItems.Add(rooster);
                }
                else
                {
                    var rooster = Instantiate(TussenUurPrefab, content.transform);
                    rooster.GetComponent<AppointmentInfo>().SetAppointmentInfo("",
                        DateTimeOffset.FromUnixTimeSeconds(appointment.start).AddHours(2).UtcDateTime.ToShortTimeString() + " - " +
                        DateTimeOffset.FromUnixTimeSeconds(appointment.end).AddHours(2).UtcDateTime.ToShortTimeString(),
                        "", "", i.ToString());
                    
                    rooster.GetComponent<AppointmentInfo>()._appointment = appointment;
                    
                    RoosterItems.Add(rooster);
                    
                    NoLessonHours.Add(i);

                    try
                    {
                        //2 consecutive tussenuren
                        if (NoLessonHours[i - 1] == i)
                        {
                            Destroy(content.transform.GetChild(i - 1).gameObject);
                            RoosterItems.Remove(rooster);
                            
                            var rooster2 = Instantiate(TussenUurPrefab, content.transform);
                            rooster2.GetComponent<AppointmentInfo>().SetAppointmentInfo("",
                                DateTimeOffset.FromUnixTimeSeconds(appointments[i - 2].start).AddHours(2).UtcDateTime.ToShortTimeString() + " - " +
                                DateTimeOffset.FromUnixTimeSeconds(appointment.end).AddHours(2).UtcDateTime.ToShortTimeString(),
                                "", "", (i - 1).ToString() + "\n" + i.ToString());
                            
                            RectTransform rectTransform = rooster2.GetComponent<RectTransform>();
                            
                            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x,rectTransform.sizeDelta.y * 2f);
                            
                            Destroy(rooster);
                            
                            rooster2.GetComponent<AppointmentInfo>()._appointment = appointment;
                            
                            RoosterItems.Add(rooster2);
                        }
                        
                        //3 consecutive tussenuren
                        if (NoLessonHours[i - 2] == i - 1)
                        {
                            Destroy(RoosterItems[i - 3]);
                            RoosterItems.Remove(RoosterItems[i - 3]);
                            
                            Destroy(RoosterItems[i - 3]);
                            RoosterItems.Remove(RoosterItems[i - 3]);

                            var rooster3 = Instantiate(TussenUurPrefab, content.transform);
                            rooster3.GetComponent<AppointmentInfo>().SetAppointmentInfo("",
                                DateTimeOffset.FromUnixTimeSeconds(appointments[i - 3].start).AddHours(2).UtcDateTime.ToShortTimeString() + " - " +
                                DateTimeOffset.FromUnixTimeSeconds(appointment.end).AddHours(2).UtcDateTime.ToShortTimeString(),
                                "", "", (i - 2).ToString() + "\n" + (i - 1).ToString() + "\n" + i.ToString());
                            
                            RectTransform rectTransform = rooster3.GetComponent<RectTransform>();
                            
                            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x,rectTransform.sizeDelta.y * 3f);
                            
                            Destroy(rooster);
                            
                            rooster3.GetComponent<AppointmentInfo>()._appointment = appointment;
                            
                            RoosterItems.Add(rooster3);
                        }
                        
                        //4 consecutive tussenuren
                        if (NoLessonHours[i - 3] == i - 2)
                        {
                            Destroy(RoosterItems[i - 4]);
                            RoosterItems.Remove(RoosterItems[i - 4]);
                            
                            Destroy(RoosterItems[i - 4]);
                            RoosterItems.Remove(RoosterItems[i - 4]);
                            
                            Destroy(RoosterItems[i - 4]);
                            RoosterItems.Remove(RoosterItems[i - 4]);

                            var rooster4 = Instantiate(TussenUurPrefab, content.transform);
                            rooster4.GetComponent<AppointmentInfo>().SetAppointmentInfo("",
                                DateTimeOffset.FromUnixTimeSeconds(appointments[i - 4].start).AddHours(2).UtcDateTime.ToShortTimeString() + " - " +
                                DateTimeOffset.FromUnixTimeSeconds(appointment.end).AddHours(2).UtcDateTime.ToShortTimeString(),
                                "", "", (i - 3).ToString() + "\n" + (i - 2).ToString() + "\n" + (i - 1).ToString() + "\n" + i.ToString());
                            
                            RectTransform rectTransform = rooster4.GetComponent<RectTransform>();
                            
                            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x,rectTransform.sizeDelta.y * 4f);
                            
                            Destroy(rooster);
                            
                            RoosterItems.Add(rooster4);
                        }

                    }
                    catch (ArgumentOutOfRangeException) { }
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw e;
            }
        }

        foreach (Transform child in content.transform)
        {
            bool contains = RoosterItems.Contains(child.gameObject);
            
            if (!contains)
            {
                Destroy(child.gameObject);
            }
            else
            {
                child.GetComponent<Button>().onClick.AddListener(() =>
                {
                    ViewManager.Instance.Show<RoosterItemView, MainMenuView>(appointments[child.GetSiblingIndex() + NoLessonHours.Count - 1]);
                });
            };
        }

        base.Initialize();
    }
}