using System;
using System.Globalization;

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
            return Current;
        return New;
    }

    public static DateTime GetMondayOfWeek(this DateTime date)
    {
        var delta = DayOfWeek.Monday - date.DayOfWeek;
        if (delta > 0) delta -= 7;
        return date.AddDays(delta).Date;
    }

    public static DateTime GetSundayOfWeek(this DateTime date)
    {
        var delta = DayOfWeek.Sunday - date.DayOfWeek;
        if (delta < 0) delta += 7;
        return date.AddDays(delta);
    }

    public static bool IsSameWeek(this DateTime date, DateTime date2)
    {
        return date.GetMondayOfWeek() == date2.GetMondayOfWeek();
    }

    // public static int GetWeekOfYear(this DateTime date)
    // {
    //     // Get the day of the year for the date
    //     int dayOfYear = date.DayOfYear;
    //
    //     // Get the day of the week for the first day of the year
    //     DateTime firstDayOfYear = new DateTime(date.Year, 1, 1);
    //     DayOfWeek firstDayOfWeek = firstDayOfYear.DayOfWeek;
    //
    //     // Calculate the number of days between the first day of the year and the date
    //     int daysSinceFirstDay = dayOfYear - 1;
    //     if (firstDayOfWeek > DayOfWeek.Sunday)
    //     {
    //         daysSinceFirstDay -= (int)firstDayOfWeek;
    //     }
    //
    //     // Calculate the week of the year
    //     int week = daysSinceFirstDay / 7 + 1;
    //     if (firstDayOfWeek > DayOfWeek.Sunday)
    //     {
    //         week += 1;
    //     }
    //
    //     return week;
    // }

    //when monday is the first day of the week
    public static int GetWeekNumber(this DateTime date)
    {
        // Set the culture to use Netherlands (Dutch) culture
        var culture = new CultureInfo("nl-NL");
        culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;

        // Calculate the week number using Netherlands (Dutch) calendar rules
        var weekNum = culture.Calendar.GetWeekOfYear(date, culture.DateTimeFormat.CalendarWeekRule,
            culture.DateTimeFormat.FirstDayOfWeek);

        return weekNum;
    }

    public static DateTime GetMondayOfWeekAndYear(string week, string year)
    {
        var weekNumber = int.Parse(week);
        var yearNumber = int.Parse(year);

        // Calculate the date of the first day of the year
        var jan1 = new DateTime(yearNumber, 1, 1);

        // Calculate the date of the first Monday of the year
        var daysToFirstMonday = (int) DayOfWeek.Monday - (int) jan1.DayOfWeek;
        if (daysToFirstMonday < 0) daysToFirstMonday += 7;
        var firstMonday = jan1.AddDays(daysToFirstMonday);

        // Calculate the date of the Monday of the specified week and year
        var result = firstMonday.AddDays((weekNumber - 1) * 7);
        return result;
    }

    //diff
    public static TimeSpan GetDateDifference(this DateTime date, DateTime date2)
    {
        return date > date2 ? date - date2 : date2 - date;
    }

    //timespan to string
    //ex: 20 min, 1 uur, 1 dag, 2 dagen, 1 week, 2 weken, 1 maand, 2 maanden, 1 jaar, ...
    public static string GetTimeDifferenceString(this TimeSpan diff)
    {
        if (diff.TotalSeconds < 60)
            return "nu";
        if (diff.TotalMinutes < 60)
            return $"{diff.Minutes} min";
        if (diff.TotalHours < 24)
            return $"{diff.Hours} uur";
        if (diff.TotalDays < 7)
            return $"{diff.Days} dag{(diff.Days > 1 ? "en" : "")}";
        if (diff.TotalDays < 31)
            return $"{diff.Days / 7} we{(diff.Days / 7 > 1 ? "ken" : "ek")}";
        if (diff.TotalDays < 365)
            return $"{diff.Days / 30} maand{(diff.Days / 30 > 1 ? "en" : "")}";
        return $"{diff.Days / 365} jaar";
    }

    public static DateTime ToDayTimeSavingDate(this DateTime date)
    {
        var summertime = TimeZoneInfo.Local.IsDaylightSavingTime(date);
        if (summertime)
        {
            return date;
        }
        
        return date.AddHours(1);
    }
}