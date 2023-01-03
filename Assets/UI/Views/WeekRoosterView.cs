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

            RefreshButton.onClick.AddListener(() => Refresh(false));

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
        
            DateTime extraDays = TimeManager.Instance.DateTime.StartOfWeek(DayOfWeek.Monday).AddDays(addedDays);
            _dateText.text = "Week van " + extraDays.ToString("d MMMM");
        
            NoLessonHours = new List<int>();
            RoosterItems = new List<GameObject>();
        
            for (int x = 0; x < dagenVanDeWeek.Length; x++)
            {
                appointments = new List<Schedule.Appointment>();
            
                appointments = _schedule.GetScheduleOfDay(TimeManager.Instance.DateTime.StartOfWeek(DayOfWeek.Monday).AddDays(x + addedDays), (bool)args);

                if (appointments == null)
                    appointments = new List<Schedule.Appointment>();
                
                for (int i = 1, listIndex = 0; i < maxNumberOfLessons + 1; i++)
                {
                    if ((!(listIndex >= appointments.Count) && appointments[listIndex]?.appointmentType == "lesson" &&
                         int.Parse(appointments[listIndex].startTimeSlotName) == i) ||
                        (!(i >= appointments.Count) && appointments[i - 1]?.appointmentType == "lesson" &&
                         int.Parse(appointments[i - 1].startTimeSlotName) == i))
                    {
                        //les

                        var rooster = Instantiate(RoosterPrefab, dagenVanDeWeek[x].transform);
                        rooster.GetComponent<AppointmentInfo>().SetAppointmentInfo(
                            String.Join(", ", appointments[listIndex].locations),
                            TimeZoneInfo.ConvertTime(DateTimeOffset.FromUnixTimeSeconds(appointments[listIndex].start), TimeZoneInfo.Local).ToString("HH:mm") + " - " +
                            TimeZoneInfo.ConvertTime(DateTimeOffset.FromUnixTimeSeconds(appointments[listIndex].end), TimeZoneInfo.Local).ToString("HH:mm"),
                            appointments[listIndex].teachers[0],
                            String.Join(", ", appointments[listIndex].subjects),
                            appointments[listIndex].startTimeSlotName,
                            appointments[listIndex]);

                        RoosterItems.Add(rooster);

                        if (appointments[listIndex].status[0].code == 4007)
                        {
                            rooster.GetComponent<Image>().color = new Color(1f, 0f, 0f, 0.5f);
                        }

                        rooster.GetComponent<Button>().onClick.RemoveAllListeners();
                        rooster.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            ViewManager.Instance.ShowNewView<RoosterItemView>(rooster.GetComponent<AppointmentInfo>()
                                ._appointment);
                        });

                        //must be at the end
                        listIndex++;
                    }
                    else
                    {
                        NoLessonHours.Add(i);
                    
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

                        tussenUur.GetComponent<Button>().onClick.RemoveAllListeners();
                        tussenUur.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            ViewManager.Instance.ShowNewView<RoosterItemView>(tussenUur
                                .GetComponent<AppointmentInfo>()._appointment);
                        });

                        RoosterItems.Add(tussenUur);

                        if (!(listIndex >= appointments.Count) && appointments[listIndex].startTimeSlotName == (i).ToString())
                        {
                            listIndex++;
                        }
                    }

                    foreach (Transform child in dagenVanDeWeek[x].transform)
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
            List<int> temp = new List<int>();
            temp = NoLessonHours;
        
            for (int i = 0, x = 0; i < NoLessonHours.Count; i++)
            {
                if (temp[i] != -1)
                {
                    RoosterItems[temp[i] - 1 + (x * maxNumberOfLessons)].GetComponent<CanvasGroup>().alpha = 0f;
                }
                else
                {
                    x++;
                }
            }
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