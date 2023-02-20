using System;
using System.Collections.Generic;
using System.Web;
using DG.Tweening;
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
        [SerializeField] private Homework _homework;
        [SerializeField] private ScrollRect _scrollRect;

        [Space, Header("UI controls")]
        [SerializeField] private TMP_Text _dateTextNumber;
        [SerializeField] private TMP_Text _dateTextDayName;

        [Space]
        private List<Schedule.Appointment> appointments;
        [SerializeField] private List<int> NoLessonHours;
        [SerializeField] private List<GameObject> RoosterItems;
        [SerializeField] private int maxNumberOfLessons;
        [SerializeField] private int addedDays = 0;


        [Header("RoosterPart")] 
        [SerializeField] private GameObject HomeworkParent;
        [SerializeField] private GameObject HomeworkPrefab;
        [SerializeField] private RectTransform _HomeworkToShow;
        [SerializeField] private ScrollRect _HomeworkScrollRect;
        
        bool hasLoaded = false;
        
        DateTime closestLessonDate;
        Transform closestLessonTransform;

        public override void Initialize()
        {
            if (args == null) args = false;
            if (LocalPrefs.GetString("zermelo-access_token") == null) return;

            addedDays = TimeManager.Instance.DateTime.DayOfWeek switch
            {
                DayOfWeek.Saturday => 2,
                DayOfWeek.Sunday => 1,
                _ => addedDays
            };
            
            closeButtonWholePage.onClick.AddListener(() =>
            {
                openNavigationButton.enabled = true;
                ViewManager.Instance.HideNavigation();
            });
            
            RefreshButton.enabled = true;
            RefreshButton.onClick.AddListener(() =>
            {
                RefreshButton.enabled = false;
                RoosterRefresh(TimeManager.Instance.DateTime.AddDays(addedDays));
            });
            
            WeekRoosterButton.onClick.AddListener(() =>
            {
                SwitchView.Instance.Show<DagRoosterView>();
            });
            
            // SwipeDetector.onSwipeRight += () =>
            // {
            //     addedDays--;
            //     RoosterRefresh(TimeManager.Instance.DateTime.AddDays(addedDays), true);
            // };
            //
            // SwipeDetector.onSwipeLeft += () => 
            // { 
            //     addedDays++;
            //     RoosterRefresh(TimeManager.Instance.DateTime.AddDays(addedDays), true);
            // };

            if (hasLoaded == false)
            {
                RoosterRefresh(TimeManager.Instance.DateTime.AddDays(addedDays));
                hasLoaded = true;
            }

            base.Initialize();
        }

        private void RoosterRefresh(DateTime date, bool savedIsGood = false)
        {
            _dateTextNumber.text = date.ToString("dd");
            _dateTextDayName.text = date.ToString("dddd").Substring(0, 3);
            closestLessonDate = date;
            
            NoLessonHours = new List<int>();
            appointments = new List<Schedule.Appointment>();
            RoosterItems = new List<GameObject>();

            appointments = _schedule.GetScheduleOfDay(date, savedIsGood);

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
                        String.Join(", ", appointments[listIndex].subjects), appointments[listIndex].startTimeSlotName,
                        appointments[listIndex]);


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
                    
                    closestLessonDate = closestLessonDate.IsDateCloser(appointments[listIndex].start.ToDateTime());
                    if (closestLessonDate == appointments[listIndex].start.ToDateTime())
                    {
                        closestLessonTransform = rooster.transform;
                    }

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
                
                    tussenUur.GetComponent<AppointmentInfo>().SetAppointmentInfo("Geen les", "", "", i.ToString(), appointment);
                    
                    tussenUur.GetComponent<Button>().onClick.RemoveAllListeners();
                    tussenUur.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        ViewManager.Instance.ShowNewView<InPlanLesView>(tussenUur.GetComponent<AppointmentInfo>().Appointment);
                    });
                
                    RoosterItems.Add(tussenUur);
                
                    if (!(listIndex >= appointments.Count) && appointments[listIndex].startTimeSlotName == (i).ToString())
                    {
                        closestLessonDate = closestLessonDate.IsDateCloser(appointments[listIndex].start.ToDateTime());
                        if (closestLessonDate == appointments[listIndex].start.ToDateTime())
                        {
                            closestLessonTransform = tussenUur.transform;
                        }
                        listIndex++;
                    }

                    if (appointment != null && appointment.appointmentType == "choice")
                    {
                        
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

            _scrollRect.decelerationRate = 0f;
            _scrollRect.content.DOLocalMove(_scrollRect.GetSnapToPositionToBringChildIntoView((RectTransform) closestLessonTransform), 0.1f, true).onComplete += () => _scrollRect.decelerationRate = 0.135f;
            
            //HomeworkRefresh();
            // if (!LocalPrefs.GetBool("show_tussenuren", true))
            // {
            //     hideTussenUren();
            // }
            
            
        }

        private void HomeworkRefresh()
        {
            foreach (Transform child in HomeworkParent.transform)
            {
                Destroy(child.gameObject);
            }
            
            var homework = _homework.GetTodaysHomework();
            if (homework == null)
                homework = new List<Homework.Item>();

            Instantiate(HomeworkPrefab, HomeworkParent.transform).AddComponent<CanvasGroup>().alpha = 0;
            
            foreach (Homework.Item homeworkItem in homework)
            {
                var HomeworkItem = Instantiate(HomeworkPrefab, HomeworkParent.transform);
                HomeworkItem.GetComponent<HomeworkInfo>().SetHomeworkInfo(homeworkItem.lesgroep.vak.naam, homeworkItem.studiewijzerItem.onderwerp, homeworkItem.datumTijd,false, homeworkItem);
                
                if (_HomeworkToShow == null)
                {
                    _HomeworkToShow = HomeworkItem.GetComponent<RectTransform>();
                }
            }
            
            _HomeworkScrollRect.decelerationRate = 0f;
            _HomeworkScrollRect.content.DOLocalMove(_HomeworkScrollRect.GetSnapToPositionToBringChildIntoView(_HomeworkToShow), 0.1f, true).onComplete += () => _HomeworkScrollRect.decelerationRate = 0.135f;
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
            // for (int i = 0; i < NoLessonHours.Count; i++)
            // {
            //     RoosterItems[NoLessonHours[i] - 1].SetActive(false);
            // }
        }
        
        public override void Refresh(object args)
        {
            openNavigationButton.onClick.RemoveAllListeners();
            closeButtonWholePage.onClick.RemoveAllListeners();
            RefreshButton.onClick.RemoveAllListeners();
            WeekRoosterButton.onClick.RemoveAllListeners();
            base.Refresh(args);
        }
    }
}