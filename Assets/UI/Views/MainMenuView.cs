using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class MainMenuView : View
{
    [SerializeField] private GameObject paklijstGameObject;
    [SerializeField] private GameObject paklijstContentGameObject;
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
        vakken = _vakken.getVakken();
        
        PlayerPrefs.SetString("main_menu_settings", "1,1,0");

        appointments = zermeloSchedule.getScheduleOfDay(TimeManager.Instance.DateTime);

        if (appointments == null || appointments.Count == 0) return;

        #region paklijst
        if (PlayerPrefs.GetString("main_menu_settings").Split(",")[0] == "1") // show paklijst
        {
            // TimeSpan span = (timeTillDeparture - DateTime.Now);
            // if (UnixTimeStampToDateTime(appointments[1].start) <= DateTime.Now)
            // {
            //     paklijstGameObject.SetActive(false);
            // }
            //else
            //{
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
                                        ? "<s>• " + item.subjects[0] + "<s>"
                                        : "• " + vakken.items.Find(x => x.afkorting == item.subjects[0]).naam;
                                paklijstItem.GetComponent<Paklijst>().text.color = isOn ? Color.gray : Color.black;
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
        if (PlayerPrefs.GetString("main_menu_settings").Split(",")[1] == "1") // show vertrektijd
        {
            if (appointments == null) return;
            
            int minutesbeforeclass = PlayerPrefs.GetInt("minutesbeforeclass", 0);
            if (minutesbeforeclass == 0) return;

            var firstlesson = appointments.Find(x => x.appointmentType == "lesson");

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
            tijdText1.transform.parent.parent.parent.gameObject.SetActive(false);
        }
        #endregion

        #region samenvatting - Disabled by default
        if (PlayerPrefs.GetString("main_menu_settings").Split(",")[2] == "1") // show dag samenvatting
        {
            dagSamenvattingText.transform.parent.parent.parent.gameObject.SetActive(true);
            dagSamenvattingText.gameObject.SetActive(true);
            
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
                
                sb.Append($"Deze lessen heb je tussen het {appointments.Where(x => x.appointmentType.ToLower() == "lesson" && x.cancelled == false).First().startTimeSlotName}e en {appointments.Where(x => x.appointmentType.ToLower() == "lesson" && x.cancelled == false).Last().startTimeSlotName}e uur.");
            }
            //lessen

            //uitval
            if (appointments.Where(x => x.cancelled).Count() == 0)
            {
                sb.Append("Vandaag is er geen les uitval, balen.");
            }
            else if (appointments.Where(x => x.cancelled).Count() == 1)
            {
                sb.Append($"Vandaag is er 1 les uitval, dat is: {appointments.Where(x => x.cancelled).First().subjects[0]}.");
            }
            else
            {
                sb.Append($"Vandaag zijn er {appointments.Where(x => x.cancelled).Count()} lessen uitval, dat zijn: ");
                for (int i = 0; i < appointments.Where(x => x.cancelled).Count(); i++)
                {
                    if (i == appointments.Where(x => x.cancelled).Count() - 1)
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
            /*
            if (appointments.Where(x => x.appointmentType.ToLower() == "toest").Count() == 1)
            {
                sb.Append($"Vandaag is er 1 toest, dat is: {appointments.Where(x => x.appointmentType.ToLower() == "toest").First().subjects[0]}.");
            }
            else
            {
                sb.Append($"Vandaag zijn er {appointments.Where(x => x.appointmentType.ToLower() == "toest").Count()} toesten, dat zijn: ");
                for (int i = 0; i < appointments.Where(x => x.appointmentType.ToLower() == "toest").Count(); i++)
                {
                    if (i == appointments.Where(x => x.appointmentType.ToLower() == "toest").Count() - 1)
                    {
                        sb.Append($"en {appointments.Where(x => x.appointmentType.ToLower() == "toest").ToList()[i].subjects[0]}.");
                    }
                    else
                    {
                        sb.Append($"{appointments.Where(x => x.appointmentType.ToLower() == "toest").ToList()[i].subjects[0]}, ");
                    }
                }
            }*/
            
            dagSamenvattingText.text = sb.ToString();
            
        }
        else
        {
            dagSamenvattingText.transform.parent.parent.parent.gameObject.SetActive(false);
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
        if(IsVisible == false) return;
        
        Schedule.Appointment currentAppointment = GetCurrentLesson(appointments);
        
        if (currentAppointment == null)
        {
            tijdText1.gameObject.SetActive(false);
            tijdText2.gameObject.SetActive(true);
            
            tijdText2.text = "Geen lessen meer vandaag";
            return;
        }

        #region Vertrektijd

        TimeSpan span = (timeTillDeparture - TimeManager.Instance.DateTime);
        tijdText1.text = span.ToString(@"hh\:mm\:ss") + " tot vertrek";

        if (span.TotalSeconds <= 0)
        {
            tijdText1.gameObject.SetActive(false);
            tijdText2.gameObject.SetActive(true);
        }

        #endregion

        #region eerste les

        span = (new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(currentAppointment.start)
            .ToLocalTime() - TimeManager.Instance.DateTime);
        
        tijdText2.text = span.ToString(@"hh\:mm\:ss") + " tot " + currentAppointment.subjects[0];

        #endregion
    }

    private Schedule.Appointment GetCurrentLesson(List<Schedule.Appointment> appointments)
    {
        if (appointments.Count == 0) return null;
        
        var a = appointments.Where(x => new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(x.start)
                .ToLocalTime() > TimeManager.Instance.DateTime)
            .OrderBy(x => x.start)
            .FirstOrDefault();

        return a;
    }
}