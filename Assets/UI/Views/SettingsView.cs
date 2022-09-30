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
        
        SomtodayKoppeling.onClick.AddListener(() =>
        {
            print("No yet implemented");
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