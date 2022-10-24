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
    [SerializeField] private Toggle UltraSatisfyingScheduleMode;
    [SerializeField] private ScrollRect RoosterScrollRect;    
    
    [Space]
    [SerializeField] private Toggle ShowTussenUren;
    [SerializeField] private DagRoosterView dagRoosterView;
    [SerializeField] private WeekRoosterView weekRoosterView;
    
    [Space]
    [SerializeField] private TMP_InputField NumberOfDaysHomework;
    [SerializeField] private Button NumberOfDaysHomeworkPlus;
    [SerializeField] private Button NumberOfDaysHomeworkMinus;
    [SerializeField] private Button HomeworkRefreshButton;
    
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
    
    public override void Initialize()
    {
        int timesCLicked = 0;
        
        SecretSettingsButton.onClick.AddListener(() =>
        {
            print($"clicked {timesCLicked} times");
            
            timesCLicked++;
            if (timesCLicked >= ClicksNeeded)
            {
                timesCLicked = 0;
                ViewManager.Instance.Show<SecretSettingsView, NavBarView>();
            }
        });
        
        UltraSatisfyingScheduleMode.isOn = PlayerPrefs.GetInt("UltraSatisfyingScheduleMode") == 1;
        UltraSatisfyingScheduleMode.onValueChanged.AddListener((bool isOn) =>
        {
            PlayerPrefs.SetInt("UltraSatisfyingScheduleMode", isOn ? 1 : 0);
            RoosterScrollRect.horizontal = isOn;
            RoosterScrollRect.inertia = isOn;
            RoosterScrollRect.elasticity = isOn ? 0.1f : 0.3f;
        });
        
        ShowTussenUren.isOn = PlayerPrefs.GetInt("ShowTussenUren") == 1;
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

        HomeworkRefreshButton.GetComponent<CanvasGroup>().alpha = 0;
        HomeworkRefreshButton.gameObject.SetActive(false);
        
        NumberOfDaysHomework.text = PlayerPrefs.GetInt("numberofdayshomework").ToString();
        NumberOfDaysHomework.onValueChanged.AddListener((string value) =>
        {
            int number;
            if (int.TryParse(value, out number))
            {
                PlayerPrefs.SetInt("numberofdayshomework", number);
            }
            
            HomeworkRefreshButton.gameObject.SetActive(true);
            HomeworkRefreshButton.GetComponent<CanvasGroup>().DOFade(1f, 0.2f);
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
        
        HomeworkRefreshButton.onClick.AddListener(() =>
        {
            ViewManager.Instance.Refresh<HomeworkView>();
            HomeworkRefreshButton.GetComponent<CanvasGroup>().DOFade(0f, 0.2f);
            HomeworkRefreshButton.gameObject.SetActive(false);
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
        
        SomtodayKoppeling.onClick.AddListener(() =>
        {
            ViewManager.Instance.Show<NavBarView, ConnectSomtodayView>();
        });
        
        ZermeloKoppeling.onClick.AddListener(() =>
        {
            ViewManager.Instance.Show<NavBarView, ConnectZermeloView>();
        });
        
        userInfo.onClick.AddListener(() =>
        {
            ViewManager.Instance.Show<NavBarView, UserInfoView>();
        });

        PlayerPrefs.Save();
        
        base.Initialize();
    }
}