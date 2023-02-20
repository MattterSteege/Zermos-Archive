using System;
using UnityEngine;

public static class DateTimeUtils
{
    public static int ToUnixTime(this DateTime dateTime)
    {
        return (int) ((DateTimeOffset) dateTime).ToUnixTimeSeconds();
    }
    
    public static DateTime ToDateTime(this int unixTimeStamp)
    {
        return new DateTime(1970, 1, 1, 1, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimeStamp); 
    }

    public static DateTime ToDateTime(this long unixTimeStamp)
    {
        return new DateTime(1970, 1, 1, 1, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimeStamp); 
    }
    
    public static DateTime IsDateCloser(this DateTime Current, DateTime New)
    {
        //check which of the dates is closer to the current time, and return that one
        if (Math.Abs((Current - DateTime.Now).TotalSeconds) < Math.Abs((New - DateTime.Now).TotalSeconds))
        {
            return Current;
        }
        return New;
    }
}
