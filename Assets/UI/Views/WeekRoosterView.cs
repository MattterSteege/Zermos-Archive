using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeekRoosterView : View
{
    [SerializeField] private GameObject[] dagenVanDeWeek;
    [SerializeField] private GameObject RoosterPrefab;
    [SerializeField] private GameObject TussenUurPrefab;
    [SerializeField] private Button RefreshButton;
    [SerializeField] private Button DagRoosterButton;
    [SerializeField] private Schedule _schedule;
    [SerializeField] private int _date;

    private List<Schedule.Appointment> appointments;
    [SerializeField] private List<int> NoLessonHours;
    [SerializeField] private List<GameObject> RoosterItems;
    [SerializeField] private int maxNumberOfLessons;

#if UNITY_EDITOR
    public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dateTime;
    }
#endif

    public override void Initialize()
    {
        RefreshButton.onClick.AddListener(Initialize);
        DagRoosterButton.onClick.AddListener(() => { ViewManager.Instance.Show<MainMenuView, DagRoosterView>(); });

        for (int i = 0; i < dagenVanDeWeek.Length; i++)
        {
            foreach (Transform child in dagenVanDeWeek[i].transform)
            {
                Destroy(child.gameObject);
                NoLessonHours.Clear();
                NoLessonHours = new List<int>();
                appointments.Clear();
                RoosterItems.Clear();
            }
        }
        
        
        for (int x = 0; x < dagenVanDeWeek.Length; x++)
        {
            NoLessonHours = new List<int>();
            appointments = new List<Schedule.Appointment>();
            RoosterItems = new List<GameObject>();


#if UNITY_EDITOR
            if (_date != 0)
                appointments = _schedule.getScheduleOfDay(UnixTimeStampToDateTime(_date).StartOfWeek(DayOfWeek.Monday).AddDays(x));
            else
                appointments = _schedule.getScheduleOfDay(DateTime.Today.StartOfWeek(DayOfWeek.Monday).AddDays(x));
#else
            appointments = _schedule.getScheduleOfDay(DateTime.Today);
#endif

            if (appointments == null)
            {
                base.Initialize();
                return;
            }

            int lastlesson = 0;
            for (int i = 1, listIndex = 0; i < maxNumberOfLessons + 1; i++)
            {
                NoLessonHours.Add(i);

                if ((!(listIndex >= appointments.Count) && appointments[listIndex]?.appointmentType == "lesson" &&
                     int.Parse(appointments[listIndex].startTimeSlotName) == i) ||
                    (!(i >= appointments.Count) && appointments[i - 1]?.appointmentType == "lesson" &&
                     int.Parse(appointments[i - 1].startTimeSlotName) == i))
                {
                    //les

                    var rooster = Instantiate(RoosterPrefab, dagenVanDeWeek[x].transform);
                    rooster.GetComponent<AppointmentInfo>().SetAppointmentInfo(
                        String.Join(", ", appointments[listIndex].locations),
                        DateTimeOffset.FromUnixTimeSeconds(appointments[listIndex].start).AddHours(2).UtcDateTime
                            .ToShortTimeString() + " - " + DateTimeOffset.FromUnixTimeSeconds(appointments[listIndex].end)
                            .AddHours(2).UtcDateTime
                            .ToShortTimeString(), appointments[listIndex].teachers[0] + "...",
                        String.Join(", ", appointments[listIndex].subjects), appointments[listIndex].startTimeSlotName,
                        appointments[listIndex]);

                    RoosterItems.Add(rooster);

                    if (appointments[listIndex].status[0].code == 4007)
                    {
                        rooster.GetComponent<Image>().color = new Color(1f, 0f, 0f, 0.5f);
                    }

                    rooster.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        ViewManager.Instance.Show<RoosterItemView, MainMenuView>(rooster.GetComponent<AppointmentInfo>()
                            ._appointment);
                    });

                    //must be at the end
                    NoLessonHours.Remove(i);
                    listIndex++;
                }
                else
                {
                    //no lesson
                    var tussenUur = Instantiate(TussenUurPrefab, dagenVanDeWeek[x].transform);

                    Schedule.Appointment appointment;
                    if (appointments.Count <= listIndex)
                    {
                        appointment = null;
                    }
                    else
                    {
                        appointment = appointments[listIndex];
                    }

                    tussenUur.GetComponent<AppointmentInfo>()
                        .SetAppointmentInfo("Geen les", "", "", "", i.ToString(), appointment);

                    tussenUur.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        ViewManager.Instance.Show<RoosterItemView, MainMenuView>(tussenUur
                            .GetComponent<AppointmentInfo>()._appointment);
                    });

                    RoosterItems.Add(tussenUur);

                    if (!(listIndex >= appointments.Count) && appointments[listIndex].startTimeSlotName == (i).ToString())
                    {
                        listIndex++;
                    }
                }

                lastlesson = i;

                foreach (Transform child in dagenVanDeWeek[x].transform)
                {
                    bool contains = RoosterItems.Contains(child.gameObject);

                    if (!contains)
                    {
                        Destroy(child.gameObject);
                    }
                    // else
                    // {
                    //     child.GetComponent<Button>().onClick.AddListener(() =>
                    //     {
                    //         ViewManager.Instance.Show<RoosterItemView, MainMenuView>(
                    //             appointments[
                    //                 child.GetSiblingIndex() + (NoLessonHours.Count > 0 ? (NoLessonHours.Count - 1) : 0)]);
                    //     });
                    // }
                }

                if (PlayerPrefs.GetInt("ShowTussenUren") == 0)
                {
                    hideTussenUren();
                }
            }

        }
        base.Initialize();
    }

    [ContextMenu("Show Tussenuren")]
    public void showTussenUren()
    {
        for (int i = 0; i < NoLessonHours.Count; i++)
        {
            RoosterItems[NoLessonHours[i] - 1].SetActive(true);
        }
    }

    [ContextMenu("Hide Tussenuren")]
    public void hideTussenUren()
    {
        for (int i = 0; i < NoLessonHours.Count; i++)
        {
            RoosterItems[NoLessonHours[i] - 1].SetActive(false);
        }
    }
}

public static class DateTimeExtensions
{
    public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
    {
        int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }
}
