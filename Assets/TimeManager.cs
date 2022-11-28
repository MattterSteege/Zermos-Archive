using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [SerializeField] public int unixTime;
    [SerializeField] private UDateTime _dateTime;    /*<- inspector field | public field -> */ public DateTime DateTime;
    [SerializeField] private UDateTime _currentDateTime; /*<- inspector field | public field -> */ public DateTime CurrentDateTime;
    [SerializeField] private bool IsCurrent;
    
    void Start()
    {
        Instance = this;
        
        _dateTime = new DateTime(1970, 1, 1).AddSeconds(unixTime);

        DateTime = _dateTime.dateTime;
        
#if UNITY_EDITOR
        if (!IsCurrent)
        {
            CurrentDateTime = DateTime.Now.AddDays(-(DateTime.Now - DateTime).Days);
            _currentDateTime.dateTime = CurrentDateTime;   
        }
        else
        {
            DateTime = DateTime.Today;
            CurrentDateTime = DateTime.Now;
            _currentDateTime.dateTime = CurrentDateTime;
        }
#elif UNITY_ANDROID && !UNITY_EDITOR
        DateTime = DateTime.Today;
        CurrentDateTime = DateTime.Now;
        _currentDateTime.dateTime = CurrentDateTime;
#endif
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (!IsCurrent)
        {
            CurrentDateTime = DateTime.Now.AddDays(-(DateTime.Now - DateTime).Days);
            _currentDateTime.dateTime = CurrentDateTime;   
        }
        else
        {
            CurrentDateTime = DateTime.Now;
            _currentDateTime.dateTime = CurrentDateTime;
        }
#elif UNITY_ANDROID && !UNITY_EDITOR
        CurrentDateTime = DateTime.Now;
        _currentDateTime.dateTime = CurrentDateTime;
#endif
    }

    int unixTimeSave;
    UDateTime dateTimeSave;
    
    void OnValidate()
    {
        unixTime = (int) _dateTime.dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        dateTimeSave = _dateTime;

        if (unixTime != unixTimeSave)
        {
            _dateTime = new DateTime(1970, 1, 1).AddSeconds(unixTime);
            unixTimeSave = unixTime;
        }
    }
}