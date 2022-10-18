using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : View
{
    [SerializeField] private GameObject paklijstGameObject;
    [SerializeField] private GameObject paklijstContentGameObject;
    [SerializeField] private GameObject paklijstPrefab;
    [SerializeField] private Schedule zermeloSchedule;
    
    [Space]
    [SerializeField] private TMP_Text timeTillDepartureText;
    DateTime timeTillDeparture;
    
    List<Schedule.Appointment> appointments = new List<Schedule.Appointment>();
    

    public override void Initialize()
    {
        PlayerPrefs.SetString("main_menu_settings", "1,1");
        
        appointments = zermeloSchedule.getScheduleOfDay(DateTime.Today);

        if (appointments == null) return;
        
        if (PlayerPrefs.GetString("main_menu_settings", "1,1").Split(",")[0] == "1") // show paklijst
        {
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
                        paklijstItem.GetComponent<Paklijst>().text.text = isOn ? "<s>• " + item.subjects[0] + "<s>" : "• " + item.subjects[0];
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
            timeTillDeparture = dateTime.AddSeconds(firstlesson.start).ToLocalTime() - new TimeSpan(0, minutesbeforeclass, 0);
        }
    }

    private void LateUpdate()
    {
        TimeSpan span = (timeTillDeparture - DateTime.Now);
        timeTillDepartureText.text = span.ToString(@"hh\:mm\:ss");
    }
}
