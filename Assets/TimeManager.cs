using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [SerializeField] public int unixTime;
    [SerializeField] private UDateTime _dateTime;    /*<- inspector field | public field -> */ public DateTime DateTime;
    
    [SerializeField] bool TimeShouldRun = true;

    void Start()
    {
        Instance = this;

#if UNITY_EDITOR
        if (unixTime == 0)
        {
            unixTime = (int) DateTime.Today.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        _dateTime = new DateTime(1970, 1, 1).AddSeconds(unixTime);
        StartCoroutine(UpdateDateTime());

#else
        unixTime = (int) DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        _dateTime = new DateTime(1970, 1, 1).AddSeconds(unixTime);
#endif

        DateTime = _dateTime.dateTime;
        
    }
    
#if UNITY_EDITOR
    private IEnumerator UpdateDateTime()
    {
        while (TimeShouldRun)
        {
            yield return new WaitForSeconds(1);
            unixTime++;
            _dateTime = new DateTime(1970, 1, 1).AddSeconds(unixTime);
            DateTime = _dateTime.dateTime;
        }
    }
#endif

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