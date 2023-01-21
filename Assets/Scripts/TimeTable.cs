using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeTable : MonoBehaviour
{
    [Header("starting time")]
    [SerializeField] int startHour = 8;
    [SerializeField] int startMinute = 0;
    
    [Header("ending time")]
    [SerializeField] int endHour = 17;
    [SerializeField] int endMinute = 0;

    [Space, SerializeField] private float minuteIsHeightValue = 0f;
    
    public void Initialize()
    {
        minuteIsHeightValue = GetComponent<RectTransform>().rect.height / ((endHour - startHour) * 60 + (endMinute - startMinute));
    }

    [ContextMenu("add item")]
    public void addItem(RectTransform TimeItem, TimeTableItem item)
    {
        float heightFromTop =  ((item.startTimeHour + item.startTimeMinute / 60f) - (startHour + startMinute / 60f)) * 60f * minuteIsHeightValue;
        float heightNeeded = ((item.endTimeHour + item.endTimeMinute / 60f) - (item.startTimeHour + item.startTimeMinute / 60f)) * 60f * minuteIsHeightValue;
        float heightFromBottom = GetComponent<RectTransform>().rect.height - heightFromTop - heightNeeded;
        //set rect to stretch-stretch
        TimeItem.anchorMin = new Vector2(0, 0);
        TimeItem.anchorMax = new Vector2(1, 1);
        TimeItem.offsetMin = new Vector2(0, heightFromBottom);
        TimeItem.offsetMax = new Vector2(0, -heightFromTop);
    }
    
    [ContextMenu("Hour lines")]
    public void HourLines()
    {
        for (int i = startHour; i < endHour; i++)
        {
            GameObject go = new GameObject("line");
            go.transform.SetParent(transform);
            go.transform.localScale = Vector3.one;
            go.AddComponent<Image>().color = Color.black;
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.offsetMin = new Vector2(0, (i - startHour) * 60 * minuteIsHeightValue);
            rt.offsetMax = new Vector2(0, (i - startHour) * 60 * minuteIsHeightValue + 1);
        }
    }
    
    public class TimeTableItem
    {
        public int startTimeHour { get; set; }
        public int startTimeMinute { get; set; }
        public int endTimeHour { get; set; }
        public int endTimeMinute { get; set; }
    }
    
}
