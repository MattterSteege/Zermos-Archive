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
    

    void Start()
    {
        Instance = this;
        
        unixTime = (int) DateTime.Today.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        _dateTime = new DateTime(1970, 1, 1).AddSeconds(unixTime);

        DateTime = _dateTime.dateTime;
        
    }

    private void Update()
    {
        CurrentDateTime = DateTime.Now;
        _currentDateTime.dateTime = CurrentDateTime;
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