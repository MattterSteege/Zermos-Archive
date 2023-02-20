using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class WeekRoosterView : View
    {
        //[SerializeField] private GameObject[] dagenVanDeWeek;
        [SerializeField] private TimeTable[] _timeTables;
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
        [SerializeField] private bool showHoursAfterLastLesson;
        [SerializeField] private int addedDays = 0;
    
        [Space, Header("UI controls")]
        [SerializeField] private TMP_Text _dateText;
        [SerializeField] private Button nextDayButton;
        [SerializeField] private Button previousDayButton;
    
        public override void Initialize()
        {
            if (args == null) args = false;
            if (LocalPrefs.GetString("zermelo-access_token") == null) return;
            
            openNavigationButton.onClick.AddListener(() =>
            {
                openNavigationButton.enabled = false;
                ViewManager.Instance.ShowNavigation();
            });

            closeButtonWholePage.onClick.AddListener(() =>
            {
                openNavigationButton.enabled = true;
                ViewManager.Instance.HideNavigation();
            });

            RefreshButton.enabled = true;
            RefreshButton.onClick.AddListener(() =>
            {
                RefreshButton.enabled = false;
                Refresh(true);
            });

            DagRoosterButton.onClick.AddListener(() =>
            {
                ViewManager.Instance.ShowNewView<DagRoosterView>();
            });

            nextDayButton.onClick.AddListener(() =>
            {
                addedDays += 7;
                Refresh(false);
            });

            previousDayButton.onClick.AddListener(() => 
            { 
                addedDays -= 7;
                Refresh(false);
            });

            for (int i = 0; i < _timeTables.Length; i++)
            {
                foreach (Transform child in _timeTables[i].transform)
                {
                    Destroy(child.gameObject);
                    NoLessonHours.Clear();
                    NoLessonHours = new List<int>();
                    appointments.Clear();
                    RoosterItems.Clear();
                }
            }
        
            DateTime extraDays = TimeManager.Instance.DateTime.StartOfWeek(DayOfWeek.Monday).AddDays(addedDays);
            _dateText.text = "Week van " + extraDays.ToString("d MMMM");
        
            NoLessonHours = new List<int>();
            RoosterItems = new List<GameObject>();
        
            for (int x = 0; x < _timeTables.Length; x++)
            {
                appointments = new List<Schedule.Appointment>();
            
                appointments = _schedule.GetScheduleOfDay(TimeManager.Instance.DateTime.StartOfWeek(DayOfWeek.Monday).AddDays(x + addedDays), (bool)args);

                if (appointments == null)
                    appointments = new List<Schedule.Appointment>();
                
                _timeTables[x].Initialize();
                
                for (int i = 1, listIndex = 0; i < maxNumberOfLessons + 1; i++)
                {
                    NoLessonHours.Add(i);

                    if ((!(listIndex >= appointments.Count) && appointments[listIndex]?.appointmentType == "lesson" &&
                         int.Parse(appointments[listIndex].startTimeSlotName) == i) ||
                        (!(i >= appointments.Count) && appointments[i - 1]?.appointmentType == "lesson" &&
                         int.Parse(appointments[i - 1].startTimeSlotName) == i))
                    {
                        //les

                        var rooster = Instantiate(RoosterPrefab, _timeTables[x].transform);
                        rooster.GetComponent<AppointmentInfo>().SetAppointmentInfo(
                            String.Join(", ", appointments[listIndex].locations),
                            TimeZoneInfo.ConvertTime(DateTimeOffset.FromUnixTimeSeconds(appointments[listIndex].start).DateTime, TimeZoneInfo.Local).AddHours(1).ToString("HH:mm") + " - " + 
                            TimeZoneInfo.ConvertTime(DateTimeOffset.FromUnixTimeSeconds(appointments[listIndex].end).DateTime, TimeZoneInfo.Local).AddHours(1).ToString("HH:mm"),
                            String.Join(", ", appointments[listIndex].subjects), appointments[listIndex].startTimeSlotName,
                            appointments[listIndex]);

                        var timeTableItem = new TimeTable.TimeTableItem
                        {
                            startTimeHour = TimeZoneInfo.ConvertTime(DateTimeOffset.FromUnixTimeSeconds(appointments[listIndex].start).DateTime, TimeZoneInfo.Local).AddHours(1).Hour,
                            startTimeMinute = TimeZoneInfo.ConvertTime(DateTimeOffset.FromUnixTimeSeconds(appointments[listIndex].start).DateTime, TimeZoneInfo.Local).AddHours(1).Minute,
                            endTimeHour = TimeZoneInfo.ConvertTime(DateTimeOffset.FromUnixTimeSeconds(appointments[listIndex].end).DateTime, TimeZoneInfo.Local).AddHours(1).Hour,
                            endTimeMinute = TimeZoneInfo.ConvertTime(DateTimeOffset.FromUnixTimeSeconds(appointments[listIndex].end).DateTime, TimeZoneInfo.Local).AddHours(1).Minute,
                        };
                        
                        

                        RoosterItems.Add(rooster);

                        if (appointments[listIndex].status[0].code == 4007)
                        {
                            rooster.GetComponent<Image>().color = new Color(1f, 0f, 0f, 0.5f);
                        }

                        try
                        {
                            string id = ParseQueryString.ParseQuery("https://ccg.zportal.nl"+ appointments?[listIndex]?.actions?[0]?.post ?? string.Empty).Get("unenroll");
                            if (!string.IsNullOrEmpty(id))
                            {
                                rooster.GetComponent<Button>().onClick.RemoveAllListeners();
                                rooster.GetComponent<Button>().onClick.AddListener(() =>
                                {
                                    ViewManager.Instance.ShowNewView<InPlanLesView>(rooster
                                        .GetComponent<AppointmentInfo>().Appointment);
                                });
                            }
                        }
                        catch (Exception) { }
                        

                        //must be at the end
                        NoLessonHours.Remove(i);
                        listIndex++;
                        _timeTables[x].addItem(rooster.GetComponent<RectTransform>(), timeTableItem);
                    }
                    else
                    {
                        //no lesson
                        var tussenUur = Instantiate(TussenUurPrefab, _timeTables[x].transform);
                    
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
                            .SetAppointmentInfo("Geen les", "", "", i.ToString(), appointment);
                        
                        tussenUur.GetComponent<Button>().onClick.RemoveAllListeners();
                        tussenUur.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            ViewManager.Instance.ShowNewView<InPlanLesView>(tussenUur.GetComponent<AppointmentInfo>().Appointment);
                        });
                    
                        RoosterItems.Add(tussenUur);
                    
                        if (!(listIndex >= appointments.Count) && appointments[listIndex].startTimeSlotName == (i).ToString())
                        {
                            listIndex++;
                        }

                        if (appointment != null && appointment.appointmentType == "choice")
                        {
                            var timeTableItem = new TimeTable.TimeTableItem
                            {
                                startTimeHour = TimeZoneInfo.ConvertTime(DateTimeOffset.FromUnixTimeSeconds(appointments[listIndex - 1].start).DateTime, TimeZoneInfo.Local).AddHours(1).Hour,
                                startTimeMinute = TimeZoneInfo.ConvertTime(DateTimeOffset.FromUnixTimeSeconds(appointments[listIndex - 1].start).DateTime, TimeZoneInfo.Local).AddHours(1).Minute,
                                endTimeHour = TimeZoneInfo.ConvertTime(DateTimeOffset.FromUnixTimeSeconds(appointments[listIndex - 1].end).DateTime, TimeZoneInfo.Local).AddHours(1).Hour,
                                endTimeMinute = TimeZoneInfo.ConvertTime(DateTimeOffset.FromUnixTimeSeconds(appointments[listIndex - 1].end).DateTime, TimeZoneInfo.Local).AddHours(1).Minute,
                            };
                            _timeTables[x].addItem(tussenUur.GetComponent<RectTransform>(), timeTableItem);   
                        }
                        else
                        {
                            DestroyImmediate(tussenUur);
                        }
                    }
                }

                NoLessonHours.Add(-1);
            }
            base.Initialize();
        }

        [ContextMenu("Show Tussenuren")]
        public void showTussenUren()
        {
            List<int> temp = new List<int>();
            temp = NoLessonHours;
        
            for (int i = 0, x = 0; i < NoLessonHours.Count; i++)
            {
                if (temp[i] != -1)
                {
                    RoosterItems[temp[i] - 1 + (x * maxNumberOfLessons)].GetComponent<CanvasGroup>().alpha = 1f;
                }
                else
                {
                    x++;
                }
            }
        }
    
        [ContextMenu("Hide Tussenuren")]
        public void hideTussenUren()
        {
            // List<int> temp = new List<int>();
            // temp = NoLessonHours;
            //
            // for (int i = 0, x = 0; i < NoLessonHours.Count; i++)
            // {
            //     if (temp[i] != -1)
            //     {
            //         RoosterItems[temp[i] - 1 + (x * maxNumberOfLessons)].GetComponent<CanvasGroup>().alpha = 0f;
            //     }
            //     else
            //     {
            //         x++;
            //     }
            // }
        }

        public override void Refresh(object args)
        {
            openNavigationButton.onClick.RemoveAllListeners();
            closeButtonWholePage.onClick.RemoveAllListeners();
            RefreshButton.onClick.RemoveAllListeners();
            DagRoosterButton.onClick.RemoveAllListeners();
            nextDayButton.onClick.RemoveAllListeners();
            previousDayButton.onClick.RemoveAllListeners(); 
            base.Refresh(args);
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
}