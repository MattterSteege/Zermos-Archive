using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SecretSettingsView : View
{
    [SerializeField] TMP_Text output;
    [SerializeField] Button deletePlayerPrefsButton;
    [SerializeField] Button EnableLeermiddelen;
    [SerializeField] Button DisableLeermiddelen;
    [SerializeField] Vakken _vakken;
    
    public override void Initialize()
    {
        Application.logMessageReceived += HandleLog;
        
        deletePlayerPrefsButton.onClick.AddListener(() =>
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            
            ViewManager.Instance.Show<ConnectZermeloView>();
        });
        
        EnableLeermiddelen.onClick.AddListener(() =>
        {
            PlayerPrefs.SetString("SecretSettings", "1");
            PlayerPrefs.Save();
            
            _vakken.Downloadvakken();
            
            ViewManager.Instance.Refresh<NavBarView>();
        });
        
        DisableLeermiddelen.onClick.AddListener(() =>
        {
            PlayerPrefs.SetString("SecretSettings", "0");
            PlayerPrefs.Save();
            
            ViewManager.Instance.Refresh<NavBarView>();
        });
    }
    

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
            output.text += logString + "\n-----------------\n\n";
    }
}
