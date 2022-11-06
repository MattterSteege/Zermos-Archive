using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsView : View
{
    [Header("settings")]
    [SerializeField] private Toggle ShowTussenUren;
    [SerializeField] private DagRoosterView dagRoosterView;
    [SerializeField] private WeekRoosterView weekRoosterView;
    
    [Space]
    [SerializeField] private TMP_InputField NumberOfDaysHomework;
    [SerializeField] private Button NumberOfDaysHomeworkPlus;
    [SerializeField] private Button NumberOfDaysHomeworkMinus;

    [Space]
    [SerializeField] private TMP_InputField minutesBeforeClass;
    [SerializeField] private Button minutesBeforeClassPlus;
    [SerializeField] private Button minutesBeforeClassMinus;


    [Header("Koppelingen")]
    [SerializeField] private Button SomtodayKoppeling;
    [SerializeField] private Button ZermeloKoppeling;
    
    [Header("User info")]
    [SerializeField] private Button userInfo;
    
    [Header("Secret Settings")]
    [SerializeField] private Button SecretSettingsButton;
    [SerializeField] private int ClicksNeeded = 10;
    
    [Header("Send notif")]
    [SerializeField] private Button SendNotifButton;
    [SerializeField] private LessonNotificationManager lessonNotificationManager;
    
    [Header("Hulp")]
    [SerializeField] private Button openDocumentatie;

    public override void Initialize()
    {
        openNavigationButton.onClick.AddListener(() =>
        {
            openNavigationButton.enabled = false;
            ViewManager.Instance.ShowNavigation();
        });
        
        CloseButtonWholePage.onClick.AddListener(() =>
        {
            openNavigationButton.enabled = true;
            ViewManager.Instance.HideNavigation();
        });
        
        int timesCLicked = 0;
        
        SecretSettingsButton.onClick.AddListener(() =>
        {
            timesCLicked++;
            if (timesCLicked >= ClicksNeeded)
            {
                timesCLicked = 0;
                //ViewManager.Instance.Show<SecretSettingsView, NavBarView>();
            }
        });

        ShowTussenUren.isOn = PlayerPrefs.GetInt("ShowTussenUren", 1) == 1;
        ShowTussenUren.onValueChanged.AddListener((bool isOn) =>
        {
            PlayerPrefs.SetInt("ShowTussenUren", isOn ? 1 : 0);

            if (isOn)
            {
                dagRoosterView.showTussenUren();
                weekRoosterView.showTussenUren();
            }
            else
            {
                dagRoosterView.hideTussenUren();
                weekRoosterView.hideTussenUren();
            }
        });
        
        //--------------------------------------------------------------------------------
        if(PlayerPrefs.GetInt("numberofdayshomework") == 0)
        {
            PlayerPrefs.SetInt("numberofdayshomework", 14);
        }

        NumberOfDaysHomework.text = PlayerPrefs.GetInt("numberofdayshomework").ToString();
        NumberOfDaysHomework.onValueChanged.AddListener((string value) =>
        {
            int number;
            if (int.TryParse(value, out number))
            {
                PlayerPrefs.SetInt("numberofdayshomework", number);
            }
        });
        NumberOfDaysHomeworkPlus.onClick.AddListener(() =>
        {
            int numberOfDaysHomework = int.Parse(NumberOfDaysHomework.text);
            numberOfDaysHomework++;
            NumberOfDaysHomework.text = numberOfDaysHomework.ToString();
            PlayerPrefs.SetInt("numberofdayshomework", numberOfDaysHomework);
        });
        NumberOfDaysHomeworkMinus.onClick.AddListener(() =>
        {
            int numberOfDaysHomework = int.Parse(NumberOfDaysHomework.text);
            if(numberOfDaysHomework <= 1)
            {
                return;
            }
            numberOfDaysHomework--;
            NumberOfDaysHomework.text = numberOfDaysHomework.ToString();
            PlayerPrefs.SetInt("numberofdayshomework", numberOfDaysHomework);
        });
        

        //--------------------------------------------------------------------------------
        minutesBeforeClass.text = PlayerPrefs.GetInt("minutesbeforeclass").ToString();
        minutesBeforeClass.onValueChanged.AddListener((string value) =>
        {
            int number;
            if (int.TryParse(value, out number))
            {
                PlayerPrefs.SetInt("minutesbeforeclass", number);
            }
        });
        minutesBeforeClassPlus.onClick.AddListener(() =>
        {
            int minutes = int.Parse(minutesBeforeClass.text);
            minutes++;
            minutesBeforeClass.text = minutes.ToString();
            PlayerPrefs.SetInt("minutesbeforeclass", minutes);
        });
        minutesBeforeClassMinus.onClick.AddListener(() =>
        {
            int minutes = int.Parse(minutesBeforeClass.text);
            if(minutes <= 1)
            {
                return;
            }
            minutes--;
            minutesBeforeClass.text = minutes.ToString();
            PlayerPrefs.SetInt("minutesbeforeclass", minutes);
        });
        
        //--------------------------------------------------------------------------------

        SomtodayKoppeling.onClick.AddListener(() =>
        {
            ViewManager.Instance.ShowNewView<ConnectSomtodayView>();
        });
        
        ZermeloKoppeling.onClick.AddListener(() =>
        {
            ViewManager.Instance.ShowNewView<ConnectZermeloView>();
        });
        
        userInfo.onClick.AddListener(() =>
        {
            ViewManager.Instance.ShowNewView<SavedInformationView>();
        });
        
        SendNotifButton.onClick.AddListener(() =>
        {
            lessonNotificationManager.SendTestNotification();
        });
        
        openDocumentatie.onClick.AddListener(() =>
        {
            Application.OpenURL(@"https://mjtsgamer.github.io/Zermos/");
        });

        PlayerPrefs.Save();
        
        base.Initialize();
    }
}