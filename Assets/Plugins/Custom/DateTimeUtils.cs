using System;

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
}
