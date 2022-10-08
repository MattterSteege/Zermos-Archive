using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsView : View
{
    [Header("settings")]
    [SerializeField] private Toggle UltraSatisfyingScheduleMode;
    [SerializeField] private ScrollRect RoosterScrollRect;    
    
    [Space]
    [SerializeField] private Toggle ShowTussenUren;
    [SerializeField] private RoosterView roosterView;
    
    [Header("Koppelingen")]
    [SerializeField] private Button SomtodayKoppeling;
    [SerializeField] private Button ZermeloKoppeling;
    
    [Header("User info")]
    [SerializeField] private Button userInfo;
    
    public override void Initialize()
    {
        UltraSatisfyingScheduleMode.isOn = PlayerPrefs.GetInt("UltraSatisfyingScheduleMode") == 1;
        UltraSatisfyingScheduleMode.onValueChanged.AddListener((bool isOn) =>
        {
            PlayerPrefs.SetInt("UltraSatisfyingScheduleMode", isOn ? 1 : 0);
            RoosterScrollRect.horizontal = isOn;
            RoosterScrollRect.inertia = isOn;
            RoosterScrollRect.elasticity = isOn ? 0.1f : 0.3f;
        });
        
        ShowTussenUren.isOn = PlayerPrefs.GetInt("ShowTussenUren") == 0;
        ShowTussenUren.onValueChanged.AddListener((bool isOn) =>
        {
            PlayerPrefs.SetInt("ShowTussenUren", isOn ? 1 : 0);

            if (isOn)
            {
                roosterView.showTussenUren();
            }
            else
            {
                roosterView.hideTussenUren();
            }
        });
        
        SomtodayKoppeling.onClick.AddListener(() =>
        {
            ViewManager.Instance.Show<ConnectSomtodayView, MainMenuView>();
        });
        
        ZermeloKoppeling.onClick.AddListener(() =>
        {
            ViewManager.Instance.Show<ConnectZermeloView, MainMenuView>();
        });
        
        userInfo.onClick.AddListener(() =>
        {
            ViewManager.Instance.Show<UserInfoView, MainMenuView>();
        });
        
        base.Initialize();
    }
}