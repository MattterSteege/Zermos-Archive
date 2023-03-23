using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using DG.Tweening;
using MagneticScrollView;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class DagRoosterView : View
    {
        [SerializeField] private GameObject RoosterPrefab;
        [SerializeField] private GameObject TussenUurPrefab;
        [SerializeField] private Button RefreshButton;
        [SerializeField] private Button WeekRoosterButton;
        [SerializeField] private Schedule _schedule;
        [SerializeField] private MagneticScrollRect scrollRect;
        [SerializeField] private TMP_Text currentDate;

        [Space, Header("UI controls")]
        [SerializeField] DagRoosterObject[] _dagRoosterObjects;
 
        [Space]
        [SerializeField] private List<int> NoLessonHours;
        [SerializeField] private List<GameObject> RoosterItems;
        [SerializeField] private int maxNumberOfLessons;
        [SerializeField] private int addedDays = 0;
        
        [Space(30f)]
        [SerializeField] private AntoniusNews antoniusNews;
        [SerializeField] private GameObject NieuwsPrefab;
        [SerializeField] private Transform NieuwsParent;

        bool hasLoaded = false;
        int CurrentIndex = 1;
        
        DateTime closestLessonDate;
        Transform closestLessonTransform;

        private object ProvidedArgs;

        public override void Initialize()
        {
            ProvidedArgs = args ??= false;
            if (LocalPrefs.GetString("zermelo-access_token") == null) return;

            closeButtonWholePage.onClick.AddListener(() =>
            {
                openNavigationButton.enabled = true;
                ViewManager.Instance.HideNavigation();
            });
            
            RefreshButton.enabled = true;
            RefreshButton.onClick.AddListener(() =>
            {
                RefreshButton.enabled = false;
                //ZermeloRoosterRefresh(TimeManager.Instance.DateTime.AddDays(addedDays));
            });
            
            WeekRoosterButton.onClick.AddListener(() =>
            {
                SwitchView.Instance.Show<DagRoosterView>();
            });
            
            currentDate.text = TimeManager.Instance.DateTime.ToString("dddd dd MMMM");
            
            int currentPanel = (1 + _dagRoosterObjects.Length) / 2;
            scrollRect.onSelectionChangeDelay = true;
            scrollRect.onSelectionChange.AddListener((changedTo) =>
            {
                int changedToPanel = int.Parse(changedTo.name.Replace("Panel", ""));
                if (changedToPanel == currentPanel + 1 || (currentPanel == 5 && changedToPanel == 1))
                {
                    currentPanel = changedToPanel;
                    int PanelToChangeIndex = (currentPanel - 3 <= 0 ? currentPanel + 2 : currentPanel - 3) - 1;
                    _dagRoosterObjects[PanelToChangeIndex].dayOffset += _dagRoosterObjects.Length;

                    UpdateRooster(PanelToChangeIndex, (bool)ProvidedArgs);
                }
                else if (changedToPanel == currentPanel - 1 || (currentPanel == 1 && changedToPanel == 5))
                {
                    currentPanel = changedToPanel;
                    int PanelToChangeIndex = (currentPanel - 3 < 0 ? currentPanel + 2 : currentPanel - 3);
                    _dagRoosterObjects[PanelToChangeIndex].dayOffset -= _dagRoosterObjects.Length;

                    UpdateRooster(PanelToChangeIndex, (bool)ProvidedArgs);
                }
            });

            if (hasLoaded == false || (bool)args == false)
            {
                for (int i = 0; i < _dagRoosterObjects.Length; i++)
                {
                    UpdateRooster(i, (bool)ProvidedArgs);
                }
                hasLoaded = true;
            }

            base.Initialize();
            
            StartCoroutine(FetchSchoolNieuws());
        }

        private void UpdateRooster(int CurrentPanel, bool savedIsGood = false)
        {
            DagRoosterObject currentPanel = _dagRoosterObjects[CurrentPanel];
            
            DateTime date = TimeManager.Instance.DateTime.AddDays(currentPanel.dayOffset);

            currentPanel._dateTextNumber.text = date.ToString("dd");
            currentPanel._dateTextDayName.text = date.ToString("dddd").Substring(0, 3);
            closestLessonDate = date;

            foreach (Transform child in currentPanel.content.transform)
                Destroy(child.gameObject);
            

            NoLessonHours = new List<int>();
            List<Schedule.Appointment> appointments = new List<Schedule.Appointment>();
            RoosterItems = new List<GameObject>();

            appointments = _schedule.GetScheduleOfDay(date, CurrentPanel != 0 || savedIsGood);

            if (appointments == null)
                appointments = new List<Schedule.Appointment>();


            for (int i = 1, listIndex = 0; i < maxNumberOfLessons + 1; i++)
            {
                //if (appointments.Count == 0)
                //    break;

                NoLessonHours.Add(i);

                if ((!(listIndex >= appointments.Count) && appointments[listIndex]?.appointmentType == "lesson" && int.Parse(appointments[listIndex].startTimeSlotName) == i) ||
                    (!(i >= appointments.Count) && appointments[i - 1]?.appointmentType == "lesson" && int.Parse(appointments[i - 1].startTimeSlotName) == i))
                {
                    //les

                    var rooster = Instantiate(RoosterPrefab, currentPanel.content.transform);
                    rooster.GetComponent<AppointmentInfo>().SetAppointmentInfo(
                        String.Join(", ", appointments[listIndex].locations),
                        TimeZoneInfo
                            .ConvertTime(DateTimeOffset.FromUnixTimeSeconds(appointments[listIndex].start).DateTime,
                                TimeZoneInfo.Local).AddHours(1).ToString("HH:mm") + " - " +
                        TimeZoneInfo
                            .ConvertTime(DateTimeOffset.FromUnixTimeSeconds(appointments[listIndex].end).DateTime,
                                TimeZoneInfo.Local).AddHours(1).ToString("HH:mm"),
                        String.Join(", ", appointments[listIndex].subjects),
                        appointments[listIndex].startTimeSlotName,
                        appointments[listIndex]);


                    RoosterItems.Add(rooster);

                    if (appointments[listIndex]?.status[0]?.code == 4007)
                    {
                        rooster.GetComponent<Image>().color = new Color(1f, 0f, 0f, 0.5f);
                    }

                    try
                    {
                        string id = ParseQueryString.ParseQuery("https://ccg.zportal.nl" + appointments?[listIndex]?.actions?[0]?.post).Get("unenroll");
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
                    catch (Exception)
                    {
                    }

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
                    var tussenUur = Instantiate(TussenUurPrefab, currentPanel.content.transform);

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

                    if (!(appointment != null && appointment.appointmentType == "choice"))
                        continue;
                    
                    tussenUur.GetComponent<Button>().onClick.RemoveAllListeners();
                    tussenUur.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        ViewManager.Instance.ShowNewView<InPlanLesView>(tussenUur
                            .GetComponent<AppointmentInfo>().Appointment);
                    });

                    RoosterItems.Add(tussenUur);

                    if (!(listIndex >= appointments.Count) &&
                        appointments[listIndex].startTimeSlotName == (i).ToString())
                    {
                        closestLessonDate =
                            closestLessonDate.IsDateCloser(appointments[listIndex].start.ToDateTime());
                        if (closestLessonDate == appointments[listIndex].start.ToDateTime())
                        {
                            closestLessonTransform = tussenUur.transform;
                        }
                    
                        listIndex++;
                    }
                }
            }

            // foreach (Transform child in currentPanel.content.transform)
            // {
            //     bool contains = RoosterItems.Contains(child.gameObject);
            //
            //     if (!contains)
            //     {
            //         Destroy(child.gameObject);
            //     }
            // }

            currentPanel._scrollRect.decelerationRate = 0f;
            currentPanel._scrollRect.content.DOLocalMove(currentPanel._scrollRect.GetSnapToPositionToBringChildIntoView((RectTransform) closestLessonTransform), 0.1f, true).onComplete += () => currentPanel._scrollRect.decelerationRate = 0.135f;
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

        
        private IEnumerator FetchSchoolNieuws()
        {
            List<AntoniusNews.AntoniusNewsItem> newsItems = antoniusNews.GetNews();
            if (newsItems == null)
            {
                var newsItemObject = Instantiate(NieuwsPrefab, NieuwsParent.transform);
                newsItemObject.GetComponent<AntoniusNieuwsPaneel>().SetTitleAndDescription("Niets te melden", "");
                yield break;
            }

            foreach (AntoniusNews.AntoniusNewsItem newsItem in newsItems)
            {
                if (newsItem.Title == "Weerbericht Gouda") continue;
                
                var newsItemObject = Instantiate(NieuwsPrefab, NieuwsParent.transform);
                newsItemObject.GetComponent<AntoniusNieuwsPaneel>().SetTitleAndDescription(newsItem.Title, newsItem.Content);
            }
            
            yield return null;
        }
    }
}

[Serializable]
public class DagRoosterObject
{
    [SerializeField] public int dayOffset;
    [SerializeField] public int PanelId;
    [SerializeField] public GameObject gameObject;
    [SerializeField] public GameObject content;
    [SerializeField] public TMP_Text _dateTextNumber;
    [SerializeField] public TMP_Text _dateTextDayName;
    [SerializeField] public ScrollRect _scrollRect;
}