using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

namespace UI.Views
{
    public class NewsAndInformationView : View
    {
        [Space, Header("Widgets"), SerializeField] private GameObject paklijstGameObject;
        [SerializeField] private GameObject tijdenGameObject;
        [SerializeField] private GameObject samenvattingsGameObject;
        
        [Space, SerializeField] private GameObject paklijstContentGameObject;
        [SerializeField] private GameObject paklijstPrefab;
        [SerializeField] private Schedule zermeloSchedule;

        [Space] 
        [SerializeField] private TMP_Text tijdText1;
        [SerializeField] private TMP_Text tijdText2;
    
        [Space]
        [SerializeField] private TMP_Text dagSamenvattingText;

        [Space] [SerializeField] private float updateTime;

        DateTime timeTillDeparture;

        List<Schedule.Appointment> appointments = new List<Schedule.Appointment>();
        [SerializeField] private Vakken _vakken;

        private Vakken.SomtodayVakken vakken;
    

    
        public override void Initialize()
        {
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
        
            vakken = _vakken.getVakken();
            

            if (zermeloSchedule.TodaysScheduledAppointments != null)
            {
                appointments = zermeloSchedule.TodaysScheduledAppointments;
            }
            else
            {
                appointments = zermeloSchedule.getScheduleOfDay(TimeManager.Instance.DateTime);
            }

            if (appointments == null || appointments.Count == 0) return;

            #region paklijst
            if (true) // show paklijst
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
                                vak = vakken.items.Find(x => x.afkorting == item.subjects[0]).naam ??
                                      item.subjects[0];
                            }
                            catch(Exception) { }

                            paklijstItem.GetComponent<Paklijst>().text.text = "• " + vak;
                            paklijstItem.GetComponent<Paklijst>().toggle.isOn = false;

                            paklijstItem.GetComponent<Paklijst>().toggle.onValueChanged.AddListener((bool isOn) =>
                            {
                                paklijstItem.GetComponent<Paklijst>().text.text =
                                    isOn
                                        ? "<s>• " + vak + "<s>"
                                        : "• " + vak;
                                paklijstItem.GetComponent<Paklijst>().text.color = isOn ? new Color(0.3490196f, 0.7411765f, 9568628f) : new Color(0.0627451f, 0.1529412f, 0.4352942f);
                            });
                        }
                        catch (Exception) { }
                    }
                }

                if (paklijstContentGameObject.transform.childCount == 0)
                {
                    var paklijstItem = Instantiate(paklijstPrefab, paklijstContentGameObject.transform);
                    paklijstItem.GetComponent<Paklijst>().text.text = "Geen lessen vandaag";
                    paklijstItem.GetComponent<Paklijst>().text.alignment = TextAlignmentOptions.Center;
                    paklijstItem.GetComponent<Paklijst>().toggle.gameObject.SetActive(false);
                }
                //}
            }
            else
            {
                paklijstGameObject.SetActive(false);
            }
            #endregion

            #region countdown
            if (true) // show vertrektijd
            {
                tijdenGameObject.SetActive(true);
                
                if (appointments == null) return;
            
                int minutesbeforeclass = LocalPrefs.GetInt("minutesbeforeclass", 1);
                if (minutesbeforeclass == 0) return;

                var firstlesson = appointments.Find(x => x.appointmentType == "lesson" && x.status[0].code != 4007);

                if (firstlesson == null) return;
                
                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                timeTillDeparture = dateTime.AddSeconds(firstlesson.start).ToLocalTime() -
                                    new TimeSpan(0, minutesbeforeclass, 0);

                tijdText1.transform.parent.parent.parent.gameObject.SetActive(true);
                tijdText1.gameObject.SetActive(true);
                tijdText2.gameObject.SetActive(false);

                InvokeRepeating("Countdowns", 0f, updateTime);
            }
            else
            {
                tijdenGameObject.SetActive(false);
            }
            #endregion

            #region samenvatting - Disabled by default
            if (false) // show dag samenvatting
            {
                samenvattingsGameObject.SetActive(true);

                StringBuilder sb = new StringBuilder();

                //lessen
                if (appointments.Count == 0)
                {
                    sb.Append("je hebt vandaag geen lessen, je had dus lekker kunnen uitslapen!");
                }
                else if (appointments.Count == 1)
                {
                    sb.Append($"je hebt vandaag maar 1 les, dat is {appointments[0].subjects[0]}. Dit is op het {appointments[0].startTimeSlotName}e uur.");
                }
                else
                {
                    sb.Append($"je hebt vandaag {appointments.Count} lessen, dat zijn: ");
                    for (int i = 0; i < appointments.Count; i++)
                    {
                        if (i == appointments.Count - 1 && appointments[i].cancelled == false)
                        {
                            sb.Append($"en {appointments[i].subjects[0]}. ");
                        }
                        else if (appointments[i].cancelled == false)
                        {
                            sb.Append($"{appointments[i].subjects[0]}, ");
                        }
                    }
                
                    sb.Append($"Deze lessen heb je tussen het {appointments.First(x => x.appointmentType.ToLower() == "lesson" && x.cancelled == false).startTimeSlotName}e en {appointments.Last(x => x.appointmentType.ToLower() == "lesson" && x.cancelled == false).startTimeSlotName}e uur.");
                }
                //lessen

                //uitval
                if (!appointments.Any(x => x.cancelled))
                {
                    sb.Append("Vandaag is er geen les uitval, balen.");
                }
                else if (appointments.Count(x => x.cancelled) == 1)
                {
                    sb.Append($"Vandaag is er 1 les uitval, dat is: {appointments.First(x => x.cancelled).subjects[0]}.");
                }
                else
                {
                    sb.Append($"Vandaag zijn er {appointments.Count(x => x.cancelled)} lessen uitval, dat zijn: ");
                    for (int i = 0; i < appointments.Count(x => x.cancelled); i++)
                    {
                        if (i == appointments.Count(x => x.cancelled) - 1)
                        {
                            sb.Append($"en {appointments.Where(x => x.cancelled).ToList()[i].subjects[0]}. ");
                        }
                        else
                        {
                            sb.Append($"{appointments.Where(x => x.cancelled).ToList()[i].subjects[0]}, ");
                        }
                    }
                }
                //uitval
            
                //Toest
           
                // if (appointments.Count(x => x.appointmentType.ToLower() == "toest") == 1)
                // {
                //     sb.Append($"Vandaag is er 1 toest, dat is: {appointments.First(x => x.appointmentType.ToLower() == "toest").subjects[0]}.");
                // }
                // else
                // {
                //     sb.Append($"Vandaag zijn er {appointments.Count(x => x.appointmentType.ToLower() == "toest")} toesten, dat zijn: ");
                //     for (int i = 0; i < appointments.Count(x => x.appointmentType.ToLower() == "toest"); i++)
                //     {
                //         if (i == appointments.Count(x => x.appointmentType.ToLower() == "toest") - 1)
                //         {
                //             sb.Append($"en {appointments.Where(x => x.appointmentType.ToLower() == "toest").ToList()[i].subjects[0]}.");
                //         }
                //         else
                //         {
                //             sb.Append($"{appointments.Where(x => x.appointmentType.ToLower() == "toest").ToList()[i].subjects[0]}, ");
                //         }
                //     }
                // }
            
                dagSamenvattingText.text = sb.ToString();
            
            }
            else
            {
                samenvattingsGameObject.SetActive(false);
            }
        
            // begin:                                           je lesdag is vanaf het [eerste les uur nummer]e uur tot en met het [laatste les uur nummer]e.
            // als er uitvals is:                               er zijn X lessen uitgevallen, en je hebt X tussenuren.
            // als er geen uitvals is, maar wel tussenuren:     er zijn X lessen uitgevallen, en je hebt X tussenuren.
            // als er geen uitvals is, en geen tussenuren:      [VOEG NIKS TOE]
            // als er vandaag een toets is:                     er staat vandaag een (grote )toets in gepland voor [vaknaam]
        
            #endregion
        }
    
        private void Countdowns()
        {
            if(isVisible == false) return;
        
            Schedule.Appointment currentAppointment = GetCurrentLesson(appointments);
        
            if (currentAppointment == null)
            {
                tijdText1.gameObject.SetActive(false);
                tijdText2.gameObject.SetActive(true);
            
                tijdText2.text = "Geen lessen meer vandaag";
                return;
            }

            #region Vertrektijd

            TimeSpan span = (timeTillDeparture - TimeManager.Instance.CurrentDateTime);
            tijdText1.text = span.ToString(@"hh\:mm\:ss") + " tot vertrek";

            if (span.TotalSeconds <= 0)
            {
                tijdText1.gameObject.SetActive(false);
                tijdText2.gameObject.SetActive(true);
            }

            #endregion

            #region eerste les

            span = (new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(currentAppointment.start)
                .ToLocalTime() - TimeManager.Instance.CurrentDateTime);

            try
            {
                tijdText2.text = span.ToString(@"hh\:mm\:ss") + " tot " + (currentAppointment.subjects[0] ?? "error");
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
    }
}
