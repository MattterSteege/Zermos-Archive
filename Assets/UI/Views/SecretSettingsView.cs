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
    
    public override void Initialize()
    {
        Application.logMessageReceived += HandleLog;
        
        deletePlayerPrefsButton.onClick.AddListener(() =>
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            
            ViewManager.Instance.Show<ConnectZermeloView>();
        });
    }
    

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
            output.text += logString + "\n-----------------\n\n";
    }
}
