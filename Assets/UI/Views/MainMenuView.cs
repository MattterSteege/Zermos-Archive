using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MainMenuView : View
{
    [SerializeField] private GameObject paklijstGameObject;
    [SerializeField] private GameObject paklijstContentGameObject;
    [SerializeField] private GameObject paklijstPrefab;
    [SerializeField] private Schedule zermeloSchedule;

    [Space] [SerializeField] private TMP_Text tijdText1;
    [SerializeField] private TMP_Text tijdText2;

    [Space] [SerializeField] private float updateTime;

    DateTime timeTillDeparture;

    List<Schedule.Appointment> appointments = new List<Schedule.Appointment>();


    public override void Initialize()
    {
        PlayerPrefs.SetString("main_menu_settings", "1,1");

        appointments = zermeloSchedule.getScheduleOfDay(DateTime.Today);

        if (appointments == null) return;

        if (PlayerPrefs.GetString("main_menu_settings", "1,1").Split(",")[0] == "1") // show paklijst
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
                    //foreach (Schedule.Appointment item in zermeloSchedule.getScheduleOfDay(new DateTime(2022, 10, 17, 0, 0, 0)))
                {
                    if (!lessen.Contains(item.subjects[0]) && item.cancelled != true)
                    {
                        lessen.Add(item.subjects[0]);
                        var paklijstItem = Instantiate(paklijstPrefab, paklijstContentGameObject.transform);
                        paklijstItem.GetComponent<Paklijst>().text.text = "• " + item.subjects[0];
                        paklijstItem.GetComponent<Paklijst>().toggle.isOn = false;

                        paklijstItem.GetComponent<Paklijst>().toggle.onValueChanged.AddListener((bool isOn) =>
                        {
                            paklijstItem.GetComponent<Paklijst>().text.text =
                                isOn ? "<s>• " + item.subjects[0] + "<s>" : "• " + item.subjects[0];
                            paklijstItem.GetComponent<Paklijst>().text.color = isOn ? Color.gray : Color.black;
                        });
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

        if (PlayerPrefs.GetString("main_menu_settings", "1,1").Split(",")[1] == "1") // show vertrektijd
        {
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

        TimeSpan span = (timeTillDeparture - DateTime.Now);
        tijdText1.text = span.ToString(@"hh\:mm\:ss") + " tot vertrek";

        if (span.TotalSeconds <= 0)
        {
            tijdText1.gameObject.SetActive(false);
            tijdText2.gameObject.SetActive(true);
        }

        #endregion

        #region eerste les

        span = (new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(currentAppointment.start)
            .ToLocalTime() - DateTime.Now);

        tijdText2.text = span.ToString(@"hh\:mm\:ss") + " tot " + currentAppointment.subjects[0];

        #endregion
    }

    private Schedule.Appointment GetCurrentLesson(List<Schedule.Appointment> appointments)
    {
        if (appointments.Count == 0) return null;
        
        var a = appointments.Where(x => new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(x.start)
                .ToLocalTime() > DateTime.Now)
            .OrderBy(x => x.start)
            .FirstOrDefault();

        return a;
    }
}