using System;
using System.Collections.Generic;
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
    

        public override void Initialize()
        {
            openNavigationButton.onClick.RemoveAllListeners();
            openNavigationButton.onClick.AddListener(() =>
            {
                openNavigationButton.enabled = false;
                ViewManager.Instance.ShowNavigation();
            });
        
            closeButtonWholePage.onClick.RemoveAllListeners();
            closeButtonWholePage.onClick.AddListener(() =>
            {
                openNavigationButton.enabled = true;
                ViewManager.Instance.HideNavigation();
            });
        
            RefreshButton.onClick.RemoveAllListeners();
            RefreshButton.onClick.AddListener(Initialize);
            
            WeekRoosterButton.onClick.RemoveAllListeners();
            WeekRoosterButton.onClick.AddListener(() =>
            {
                ViewManager.Instance.ShowNewView<WeekRoosterView>();
            
            });
        
            nextDayButton.onClick.RemoveAllListeners(); 
            nextDayButton.onClick.AddListener(() =>
            {
                addedDays++;
                Initialize();
            });
            
            previousDayButton.onClick.RemoveAllListeners(); 
            previousDayButton.onClick.AddListener(() => 
            { 
                addedDays--;
                Initialize(); 
            });

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

            DateTime extraDays = TimeManager.Instance.DateTime.AddDays(addedDays);

            _dateText.text = extraDays.ToString("d MMMM");
        
            appointments = _schedule.getScheduleOfDay(extraDays);

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

                    var rooster = Instantiate(RoosterPrefab, content.transform);
                    rooster.GetComponent<AppointmentInfo>().SetAppointmentInfo(
                        String.Join(", ", appointments[listIndex].locations),
                        TimeZoneInfo.ConvertTime(DateTimeOffset.FromUnixTimeSeconds(appointments[listIndex].start).DateTime, TimeZoneInfo.Local).AddHours(1).ToString("HH:mm") + " - " + 
                        TimeZoneInfo.ConvertTime(DateTimeOffset.FromUnixTimeSeconds(appointments[listIndex].end).DateTime, TimeZoneInfo.Local).AddHours(1).ToString("HH:mm"),
                        String.Join(", ", appointments[listIndex].teachers),
                        String.Join(", ", appointments[listIndex].subjects), appointments[listIndex].startTimeSlotName,
                        appointments[listIndex]);

                    RoosterItems.Add(rooster);

                    if (appointments[listIndex].status[0].code == 4007)
                    {
                        rooster.GetComponent<Image>().color = new Color(1f, 0f, 0f, 0.5f);
                    }

                    rooster.GetComponent<Button>().onClick.RemoveAllListeners();
                    rooster.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        ViewManager.Instance.ShowNewView<RoosterItemView>(rooster.GetComponent<AppointmentInfo>()._appointment);
                    });

                    //must be at the end
                    NoLessonHours.Remove(i);
                    listIndex++;
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
                        ViewManager.Instance.ShowNewView<RoosterItemView>(tussenUur.GetComponent<AppointmentInfo>()._appointment);
                    });

                    RoosterItems.Add(tussenUur);

                    if (!(listIndex >= appointments.Count) && appointments[listIndex].startTimeSlotName == (i).ToString())
                    {
                        listIndex++;
                    }
                }
            
                lastlesson = i;

                foreach (Transform child in content.transform)
                {
                    bool contains = RoosterItems.Contains(child.gameObject);

                    if (!contains)
                    {
                        Destroy(child.gameObject);
                    }
                }

                if (!LocalPrefs.GetBool("ShowTussenUren", true))
                {
                    hideTussenUren();
                }
            
                base.Initialize();
            }
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

        string consecutiveNumbersToString(int[] numbers)
        {
            string result = "";
            int start = 0;
            int end = 0;
            for (int i = 0; i < numbers.Length; i++)
            {
                if (i == 0)
                {
                    start = numbers[i];
                    end = numbers[i];
                }
                else
                {
                    if (numbers[i] == end + 1)
                    {
                        end = numbers[i];
                    }
                    else
                    {
                        if (start == end)
                        {
                            result += start + ", ";
                        }
                        else
                        {
                            result += start + "-" + end + ", ";
                        }

                        start = numbers[i];
                        end = numbers[i];
                    }
                }
            }

            if (start == end)
            {
                result += start;
            }
            else
            {
                result += start + "-" + end;
            }

            return result;
        }
    }
}