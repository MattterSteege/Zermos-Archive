using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    public int unixTime;
    [SerializeField] private UDateTime _dateTime;
    public DateTime DateTime;

    void Start()
    {
        Instance = this;

#if UNITY_EDITOR
        if (unixTime == 0)
        {
            unixTime = (int) DateTime.Today.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        _dateTime = new DateTime(1970, 1, 1).AddSeconds(unixTime);
#else
        unixTime = (int) DateTime.Today.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        _dateTime = new DateTime(1970, 1, 1).AddSeconds(unixTime);
#endif

        DateTime = _dateTime.dateTime;
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