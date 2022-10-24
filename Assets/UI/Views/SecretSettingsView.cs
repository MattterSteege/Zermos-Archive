using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SecretSettingsView : View
{
    public override void Initialize()
    {
        Application.logMessageReceived += HandleLog;
    }
    
    [SerializeField] TMP_Text output;

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
            output.text += logString + "\n-----------------\n\n";
    }
}
