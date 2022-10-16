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

    public override void Initialize()
    {
        if (PlayerPrefs.GetString("main_menu_settings").Split()[0] == "1") // show paklijst
        {
            paklijstGameObject.SetActive(true);

            List<string> lessen = new List<string>();

            foreach (Schedule.Appointment item in zermeloSchedule.getScheduleOfDay(DateTime.Today))
            //foreach (Schedule.Appointment item in zermeloSchedule.getScheduleOfDay(new DateTime(2022, 10, 17, 0, 0, 0)))
            {
                if (!lessen.Contains(item.subjects[0]))
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
    }
}
