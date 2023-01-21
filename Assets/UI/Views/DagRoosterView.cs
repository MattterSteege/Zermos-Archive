using System;
using System.Collections.Generic;
using System.Web;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class DagRoosterView : View
    {
        [SerializeField] private GameObject content;
        [SerializeField] private GameObject RoosterPrefab;
        [SerializeField] private GameObject TussenUurPrefab;
        [SerializeField] private Button RefreshButton;
        [SerializeField] private Button WeekRoosterButton;
        [SerializeField] private Schedule _schedule;
        [SerializeField] private int _date;
    
        [Space, Header("UI controls")]
        [SerializeField] private TMP_Text _dateText;
        [SerializeField] private Button nextDayButton;
        [SerializeField] private Button previousDayButton;
    
        [Space]
        private List<Schedule.Appointment> appointments;
        [SerializeField] private List<int> NoLessonHours;
        [SerializeField] private List<GameObject> RoosterItems;
        [SerializeField] private int maxNumberOfLessons;
        [SerializeField] private int addedDays = 0;

        [Space] [SerializeField] private TimeTable _timeTable;
    

        public override void Initialize()
        {
            if (args == null) args = false;
            if (LocalPrefs.GetString("zermelo-access_token") == null) return;

            _timeTable.Initialize();

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
            
            RefreshButton.onClick.AddListener(() => Refresh(true));
            
            WeekRoosterButton.onClick.AddListener(() =>
            {
                ViewManager.Instance.ShowNewView<WeekRoosterView>();
            });
            
            nextDayButton.onClick.AddListener(() =>
            {
                addedDays++;
                Refresh(false);
            });
            
            previousDayButton.onClick.AddListener(() => 
            { 
                addedDays--;
                Refresh(false);
            });
            
            foreach (Transform child in content.transform)
            {
                Destroy(child.gameObject);
            }

            NoLessonHours = new List<int>();
            appointments = new List<Schedule.Appointment>();
            RoosterItems = new List<GameObject>();

            DateTime extraDays = TimeManager.Instance.DateTime.AddDays(addedDays);

            _dateText.text = extraDays.ToString("d MMMM");
            
            appointments = _schedule.GetScheduleOfDay(extraDays, (bool) args);

            if (appointments == null)
                appointments = new List<Schedule.Appointment>();
            
            
            for (int i = 1, listIndex = 0; i < maxNumberOfLessons + 1; i++)
            {
                NoLessonHours.Add(i);

                if ((!(listIndex >= appointments.Count) && appointments[listIndex]?.appointmentType == "lesson" &&
                     int.Parse(appointments[listIndex].startTimeSlotName) == i) ||
                    (!(i >= appointments.Count) && appointments[i - 1]?.appointmentType == "lesson" &&
                     int.Parse(appointments[i - 1].startTimeSlotName) == i))
                {
                    //les

                    var rooster = Instantiate(RoosterPrefab, content.transform);
                    rooster.GetComponent<AppointmentInfo>().SetAppointmentInfo(
                        String.Join(", ", appointments[listIndex].locations),
                        TimeZoneInfo.ConvertTime(DateTimeOffset.FromUnixTimeSeconds(appointments[listIndex].start).DateTime, TimeZoneInfo.Local).AddHours(1).ToString("HH:mm") + " - " + 
                        TimeZoneInfo.ConvertTime(DateTimeOffset.FromUnixTimeSeconds(appointments[listIndex].end).DateTime, TimeZoneInfo.Local).AddHours(1).ToString("HH:mm"),
                        String.Join(", ", appointments[listIndex].teachers),
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
                                    .GetComponent<AppointmentInfo>()._appointment);
                            });
                        }
                    }
                    catch (Exception) { }
                    

                    //must be at the end
                    NoLessonHours.Remove(i);
                    listIndex++;
                    _timeTable.addItem(rooster.GetComponent<RectTransform>(), timeTableItem);
                }
                else
                {
                    //no lesson
                    var tussenUur = Instantiate(TussenUurPrefab, content.transform);
                
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
                    
                    tussenUur.GetComponent<Button>().onClick.RemoveAllListeners();
                    tussenUur.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        ViewManager.Instance.ShowNewView<InPlanLesView>(tussenUur.GetComponent<AppointmentInfo>()._appointment);
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
                        _timeTable.addItem(tussenUur.GetComponent<RectTransform>(), timeTableItem);   
                    }
                    else
                    {
                        DestroyImmediate(tussenUur);
                    }
                }
            }
            
            foreach (Transform child in content.transform)
            {
                bool contains = RoosterItems.Contains(child.gameObject);

                if (!contains)
                {
                    Destroy(child.gameObject);
                }
            }

            if (!LocalPrefs.GetBool("show_tussenuren", true))
            {
                hideTussenUren();
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
        
        public override void Refresh(object args)
        {
            openNavigationButton.onClick.RemoveAllListeners();
            closeButtonWholePage.onClick.RemoveAllListeners();
            RefreshButton.onClick.RemoveAllListeners();
            WeekRoosterButton.onClick.RemoveAllListeners();
            nextDayButton.onClick.RemoveAllListeners(); 
            previousDayButton.onClick.RemoveAllListeners();
            base.Refresh(args);
        }
    }
}