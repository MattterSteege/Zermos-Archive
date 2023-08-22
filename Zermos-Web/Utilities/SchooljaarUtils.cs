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
        public DateTime vanafDatumDate => DateTime.Parse(vanafDatum);
        public DateTime totDatumDate => DateTime.Parse(totDatum);
    }
    
    private static List<Schooljaar> schooljaren { get; set; }
    
    public static void Initialize()
    {
        //load /wwwroot/data/schooljaar.json
        schooljaren = JsonConvert.DeserializeObject<List<Schooljaar>>(File.ReadAllText("wwwroot/data/schooljaren.json"));
    }
    
    public static Schooljaar GetSchooljaar(DateTime date)
    {
        if (schooljaren == null)
            Initialize();
        
        
        foreach (var schooljaar in schooljaren)
        {
            if (date >= schooljaar.vanafDatumDate && date <= schooljaar.totDatumDate)
            {
                return schooljaar;
            }
        }

        return null;
    }
    
    public static Schooljaar getCurrentSchooljaar()
    {
        return GetSchooljaar(DateTime.Now);
    }
}