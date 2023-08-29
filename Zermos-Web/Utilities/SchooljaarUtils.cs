using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Zermos_Web.Utilities;

public static class SchooljaarUtils
{
    public class Schooljaar
    {
        public string naam { get; set; }
        public string vanafDatum { get; set; }
        public string totDatum { get; set; }
        public DateTime vanafDatumDate { get; set; }
        public DateTime totDatumDate { get; set; }
    }
    
    private static List<Schooljaar> schooljaren { get; set; }
    
    public static void Initialize()
    {
        //load /wwwroot/data/schooljaar.json
        schooljaren = JsonConvert.DeserializeObject<List<Schooljaar>>(File.ReadAllText("wwwroot/data/schooljaren.json"));
    }
    
    public static Schooljaar GetSchooljaar(DateTime date)
    {
        //current date, check in which schoolyear it is.
        //a schoolyear goes from 1 of august till 31 of july
        //check in which schoolyear the dateTime is.
        DateTime start = new DateTime(date.Year, 8, 1);
        DateTime end = new DateTime(date.Year + 1, 7, 31);
        //but what if the current date is 1 of january 2021?, the schoolyear end's in 2021, so we need to check if the date is before the start of the schoolyear.
        if (date < start)
        {
            //the date is before the start of the schoolyear, so we need to check the previous schoolyear.
            start = new DateTime(date.Year - 1, 8, 1);
            end = new DateTime(date.Year, 7, 31);
        }

        return new Schooljaar
        {
            naam = $"{start.Year}/{end.Year}",
            vanafDatum = start.ToString("yyyy-MM-dd"),
            totDatum = end.ToString("yyyy-MM-dd"),
            vanafDatumDate = start,
            totDatumDate = end
        };
    }
    
    public static Schooljaar getCurrentSchooljaar()
    {
        return GetSchooljaar(DateTime.Now);
    }
}