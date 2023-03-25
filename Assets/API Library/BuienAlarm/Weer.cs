
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class Weer : BetterHttpClient
{ 
    
    public static string GetBuienalarmUrl(string latLong = "52.011194, 4.726696")
    {
        string[] parts = latLong.Split(',');
        string lat = parts[1].Trim();
        string lon = parts[0].Trim();
        return $"https://cdn-secure.buienalarm.nl/api/3.4/forecast.php?lat={lat}&lon={lon}&region=nl&unit=mm/u";
    }
    
    [SerializeField, Tooltip("'*' means Application.persistentDataPath.")]
    private string savePath = "*/Weer.json";
    
    public BuienAlarmWeer getWeer()
    {
        WorldCoordinate coordinateOfSchool = new WorldCoordinate(52.011194, 4.726696);
        WorldCoordinate coordinateOfHomeOrSchool = new WorldCoordinate(LocalPrefs.GetString("saved_weather_location", "4.726696, 52.011194"));
        
        WorldCoordinate center = WorldCoordinateUtils.GetAbsoluteCenter(coordinateOfSchool, coordinateOfHomeOrSchool);
        
        string destination = savePath.Replace("*", Application.persistentDataPath);

        if (!File.Exists(destination))
        {
            Debug.LogWarning("File not found, creating new file.");
            return DownloadWeer(center.ToString());
        }

        using (StreamReader r = new StreamReader(destination))
        {
            string json = r.ReadToEnd();
            var weerObject = JsonConvert.DeserializeObject<BuienAlarmWeer>(json);
            
            if (RoundDownToLast10Minutes(weerObject.start.ToDateTime()).AddMinutes(10) < TimeManager.Instance.CurrentDateTime)
            {
                r.Close();
                Debug.LogWarning("Local file is outdated, downloading new file.");
                return DownloadWeer(center.ToString());
            }
            return weerObject;
        }
    }

    private BuienAlarmWeer DownloadWeer(string latLong = "52.011194, 4.726696")
    {
        return (BuienAlarmWeer)Get(GetBuienalarmUrl(latLong), (response) =>
        {
            BuienAlarmWeer weer = JsonConvert.DeserializeObject<BuienAlarmWeer>(response.downloadHandler.text);
        
            if (weer != null)
            {
                string destination = savePath.Replace("*", Application.persistentDataPath);
                File.WriteAllText(destination, response.downloadHandler.text);
                return weer;
            }
            return null;        
        });
    }
    
    public static DateTime RoundDownToLast10Minutes(DateTime dateTime)
    {
        DateTime roundedDown = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
        int minuteModulus = roundedDown.Minute % 10;
        roundedDown = roundedDown.AddMinutes(-minuteModulus);
        return roundedDown;
    }

    public class BuienAlarmWeer
    {
        public bool success { get; set; }
        public int start { get; set; }
        public string start_human { get; set; }
        public int temp { get; set; }
        public int delta { get; set; }
        public List<double> precip { get; set; }
        public Levels levels { get; set; }
        public Grid grid { get; set; }
        public string source { get; set; }
        public Bounds bounds { get; set; }
    }
    
    public class Bounds
    {
        public double N { get; set; }
        public double E { get; set; }
        public double S { get; set; }
        public int W { get; set; }
    }

    public class Grid
    {
        public int x { get; set; }
        public int y { get; set; }
    }

    public class Levels
    {
        public double light { get; set; }
        public int moderate { get; set; }
        public double heavy { get; set; }
    }
}
