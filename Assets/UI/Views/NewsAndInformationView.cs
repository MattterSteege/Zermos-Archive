using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AwesomeCharts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI.Views
{
    public class NewsAndInformationView : View
    {
        [Space, Header("Widgets"), SerializeField] private GameObject paklijstGameObject;
        [SerializeField] private GameObject tijdenGameObject;

        [Space, SerializeField] private GameObject paklijstContentGameObject;
        [SerializeField] private GameObject paklijstPrefab;
        [SerializeField] private Schedule zermeloSchedule;

        [Space] 
        [SerializeField] private TMP_Text tijdTotVertrekkenText;
        [SerializeField] private TMP_Text tijdTotVolgendeLesText;
        [SerializeField] private float updateTime;
        
        [Space] 
        [SerializeField] private LineChart weerChart;
        [SerializeField] private Weer BuienAlarmWeer;
        [SerializeField] private TMP_Text weerText;
        [SerializeField] private Slider slider;
        
        
        DateTime timeTillDeparture;

        List<Schedule.Appointment> appointments = new List<Schedule.Appointment>();
        [SerializeField] private Vakken _vakken;

        private Vakken.SomtodayVakken vakken;
    

    
        public override void Initialize()
        {
            vakken = _vakken.getVakken();

            bool showPaklijst = LocalPrefs.GetBool("show_paklijst", true);
            bool showTijd = LocalPrefs.GetBool("show_tijd", true);

            if (zermeloSchedule.TodaysScheduledAppointments != null)
                appointments = zermeloSchedule.TodaysScheduledAppointments.response.data[0].appointments;
            else
                appointments = zermeloSchedule.GetScheduleOfDay(TimeManager.Instance.DateTime, false);
            

            #region paklijst
            if (showPaklijst) // show paklijst
            {
                paklijstGameObject.SetActive(true);

                List<string> lessen = new List<string>();

                foreach (Schedule.Appointment item in appointments)
                {
                    if (item.appointmentType != "choice" && !lessen.Contains(item.subjects[0]) && item.cancelled != true)
                    {
                        try
                        {
                            lessen.Add(item.subjects[0]);
                            var paklijstItem = Instantiate(paklijstPrefab, paklijstContentGameObject.transform);

                            string vak = item.subjects[0];
                            try
                            {
                                vak = vakken.items.Find(x => x.vak.afkorting == item.subjects[0]).vak.naam ??
                                      item.subjects[0];
                            }
                            catch (Exception)
                            {
                            }

                            paklijstItem.GetComponent<Paklijst>().text.text = "• " + vak;
                            paklijstItem.GetComponent<Paklijst>().toggle.isOn = false;

                            paklijstItem.GetComponent<Paklijst>().toggle.onValueChanged.AddListener((bool isOn) =>
                            {
                                paklijstItem.GetComponent<Paklijst>().text.text =
                                    isOn
                                        ? "<s>• " + vak + "<s>"
                                        : "• " + vak;
                                paklijstItem.GetComponent<Paklijst>().text.color = isOn
                                    ? new Color(0.509804f, 0.5176471f, 0.6039216f, 0.8f)
                                    : new Color(0.509804f, 0.5176471f, 0.6039216f, 1f);
                            });
                        }
                        catch (Exception)
                        {
                            var paklijstItem = Instantiate(paklijstPrefab, paklijstContentGameObject.transform);
                            paklijstItem.GetComponent<Paklijst>().text.text = "Geen lessen vandaag";
                            paklijstItem.GetComponent<Paklijst>().text.alignment = TextAlignmentOptions.Center;
                            paklijstItem.GetComponent<Paklijst>().toggle.gameObject.SetActive(false);
                        }
                    }
                }

                if (paklijstContentGameObject.transform.childCount == 0)
                {
                    var paklijstItem = Instantiate(paklijstPrefab, paklijstContentGameObject.transform);
                    paklijstItem.GetComponent<Paklijst>().text.text = "Geen lessen vandaag";
                    paklijstItem.GetComponent<Paklijst>().text.alignment = TextAlignmentOptions.Center;
                    paklijstItem.GetComponent<Paklijst>().toggle.gameObject.SetActive(false);
                }
                
                //tell packlijstGameObject to refresh, since we added new items
                paklijstGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            }
            else
            {
                paklijstGameObject.SetActive(false);
            }
            #endregion

            #region countdown

            if (showTijd) // show vertrektijd
            {
                tijdenGameObject.SetActive(true);

                if (appointments != null)
                {
                    int minutesbeforeclass = LocalPrefs.GetInt("minutes_before_class", 1);
                    if (minutesbeforeclass != 0)
                    {
                        var firstlesson =
                            appointments.Find(x => x.appointmentType == "lesson" && x.status[0].code != 4007);

                        if (firstlesson != null)
                        {
                            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                            timeTillDeparture = dateTime.AddSeconds(firstlesson.start).ToLocalTime() -
                                                new TimeSpan(0, minutesbeforeclass, 0);

                            tijdTotVertrekkenText.transform.parent.parent.parent.gameObject.SetActive(true);
                            tijdTotVertrekkenText.gameObject.SetActive(true);
                            tijdTotVolgendeLesText.gameObject.SetActive(false);

                            InvokeRepeating("Countdowns", 0f, updateTime);
                        }
                    }
                }
            }
            else
            {
                tijdenGameObject.SetActive(false);
            }
            #endregion
            
            #region Weer

            LineDataSet weerDataSet = new LineDataSet();
            Weer.BuienAlarmWeer weer = BuienAlarmWeer.getWeer();
            int[] times = new int[weer?.precip?.Count ?? 0];

            for (var i = 0; i < weer?.precip?.Count; i++)
            {
                var mmu = weer.precip[i];
                
                int hour = int.Parse(weer.start_human.Split(':')[0]);
                int minute = int.Parse(weer.start_human.Split(':')[1]);
                
                minute += (5 * i);
                if (minute >= 60)
                {
                    hour++;
                    minute -= 60;
                }
                if (minute >= 60)
                {
                    hour++;
                    minute -= 60;
                }
                
                string time = hour + (minute.ToString().Length == 1 ? "0" + minute : minute.ToString());
                int timeInt = int.Parse(time);
                times[i] = timeInt;

                weerDataSet.AddEntry(new LineEntry(i, (float) mmu));
            }

            weerDataSet.Title = "weer in " + weer?.source;
            weerDataSet.LineColor = new Color(0.3921569f, 0.572549f, 0.9764706f, 1f);
            weerDataSet.FillColor = new Color(0.3921569f, 0.572549f, 0.9764706f, 1f);
            weerDataSet.LineThickness = 3f;
            weerDataSet.UseBezier = true;
            weerChart.GetChartData().DataSets.Add(weerDataSet);
            weerChart.SetDirty();
            
            LineEntry entry = weerDataSet.Entries[2];
            string timeString = times[2].ToString().Substring(0, times[2].ToString().Length - 2) + ":" + times[2].ToString().Substring(times[2].ToString().Length - 2, 2);
            weerText.text = timeString + " - " + entry.Value + " mm/u";
            
            slider.maxValue = weerDataSet.Entries.Count;
            slider.value = 2f;
            
            slider.onValueChanged.AddListener((float value) =>
            {
                int index = (int) value;
                if (index >= weerDataSet.Entries.Count) return;
                
                LineEntry entry = weerDataSet.Entries[index];
                string time = times[index].ToString();
                string hour = time.Substring(0, time.Length - 2);
                string minute = time.Substring(time.Length - 2, 2);
                string timeString = hour + ":" + minute;

                weerText.text = timeString + " - " + entry.Value + " mm/u";
            });
            
            #endregion
        }
    
        private void Countdowns()
        {
            if(isVisible == false) return;
        
            Schedule.Appointment currentAppointment = GetCurrentLesson(appointments);
        
            if (currentAppointment == null)
            {
                tijdTotVertrekkenText.gameObject.SetActive(false);
                tijdTotVolgendeLesText.gameObject.SetActive(true);
            
                tijdTotVolgendeLesText.text = "Geen lessen meer vandaag";
                return;
            }

            #region Vertrektijd

            TimeSpan span = (timeTillDeparture - TimeManager.Instance.CurrentDateTime);
            tijdTotVertrekkenText.text = span.ToString(@"hh\:mm\:ss") + " tot vertrek";

            if (span.TotalSeconds <= 0)
            {
                tijdTotVertrekkenText.gameObject.SetActive(false);
                tijdTotVolgendeLesText.gameObject.SetActive(true);
            }

            #endregion

            #region eerste les

            span = (new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(currentAppointment.start)
                .ToLocalTime() - TimeManager.Instance.CurrentDateTime);

            try
            {
                tijdTotVolgendeLesText.text = span.ToString(@"hh\:mm\:ss") + " tot " + (currentAppointment.subjects[0] ?? "error")  + " in " + (currentAppointment.locations[0] ?? "error");
            }
            catch (Exception) { }

            #endregion
        }

        private Schedule.Appointment GetCurrentLesson(List<Schedule.Appointment> appointments)
        {
            if (appointments.Count == 0) return null;
        
            var a = appointments.Where(x => new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(x.start)
                    .ToLocalTime() > TimeManager.Instance.CurrentDateTime)
                .OrderBy(x => x.start)
                .FirstOrDefault();

            return a;
        }
        
        public override void Refresh(object args)
        {
            openNavigationButton.onClick.RemoveAllListeners();
            closeButtonWholePage.onClick.RemoveAllListeners();
            base.Refresh(args);
        }
    }
}
