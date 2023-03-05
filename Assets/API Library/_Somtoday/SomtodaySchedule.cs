using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class SomtodaySchedule : BetterHttpClient
{
    [SerializeField, Tooltip("'*' means Application.persistentDataPath.")]
    public string savePath = "*/SomtodaySchedule.json";
    int tries = 0;
    [SerializeField] private int MinutesBeforeRedownload = 60;

    public List<Item> GetSchedule(DateTime date = default, bool ShouldRefreshFile = false)
    {
        DateTime beginDatum;
        DateTime eindDatum;
        
        if (date == default)
        {
            beginDatum = DateTime.Today.AddDays(-7).GetMondayOfWeek();
            eindDatum = DateTime.Today.AddDays(21).GetSundayOfWeek();
        }
        else
        {
            beginDatum = date.AddDays(-7).GetMondayOfWeek();
            eindDatum = date.AddDays(21).GetSundayOfWeek();
        }
        
        string destination = savePath.Replace("*", Application.persistentDataPath);

        if (!File.Exists(destination))
        {
            Debug.LogWarning("File not found, creating new file.");
            return DownloadLessons(beginDatum, eindDatum).items;
        }

        using (StreamReader r = new StreamReader(destination))
        {
            string json = r.ReadToEnd();
            var scheduleObject = JsonConvert.DeserializeObject<SomtodayRooster>(json);
            if ((scheduleObject?.laatsteWijziging.ToDateTime().AddMinutes(MinutesBeforeRedownload) < TimeManager.Instance.CurrentDateTime && tries < 2) || (ShouldRefreshFile && scheduleObject?.laatsteWijziging.ToDateTime().AddSeconds(15) < TimeManager.Instance.CurrentDateTime && tries < 2))
            {
                r.Close();
                Debug.LogWarning("Local file is outdated, downloading new file.");
                return DownloadLessons(beginDatum, eindDatum).items;
            }
            return scheduleObject.items;
        }
    }
    
    //get day schedule
    public List<Item> GetScheduleOfDay(DateTime date, bool shouldRefreshFile)
    {
        List<Item> schedule = GetSchedule(date, shouldRefreshFile);

        if (schedule == null)
        {
            return null;
        }

        List<Item> TodaySchedule = new List<Item>();

        foreach (Item appointment in schedule)
        {
            if (appointment.beginDatumTijd >= date && appointment.beginDatumTijd <= date.AddDays(1))
            {
                TodaySchedule.Add(appointment);
            }
        }

        return TodaySchedule.OrderBy(x => x.beginDatumTijd).ToList();
    }

    private SomtodayRooster DownloadLessons(DateTime beginDatum, DateTime eindDatum)
    {
        if (LocalPrefs.GetString("somtoday-access_token") == null)
            return null;
        
        Dictionary<string, string> headers = new Dictionary<string,string>();
        headers.Add("Authorization", "Bearer " + LocalPrefs.GetString("somtoday-access_token"));
        return (SomtodayRooster) Get($"https://api.somtoday.nl/rest/v1/afspraken?begindatum={beginDatum:yyyy-MM-dd}&einddatum={eindDatum:yyyy-MM-dd}&additional=vak&additional=docentAfkortingen", headers, (response) =>
        {
            SomtodayRooster schooljaar = JsonConvert.DeserializeObject<SomtodayRooster>(response.downloadHandler.text);
            schooljaar.laatsteWijziging = TimeManager.Instance.CurrentDateTime.ToUnixTime();
            
            var convertedJson = JsonConvert.SerializeObject(schooljaar, Formatting.Indented);

            string destination = savePath.Replace("*", Application.persistentDataPath);
            
            File.WriteAllText(destination, convertedJson);

            return schooljaar;
        }, (error) =>
        {
            AndroidUIToast.ShowToast("Er is iets misgegaan tijdens het opvragen van je schoolrooster bij SOMtoday. Probeer het later opnieuw.");
            return null;
        });
        
    }

    #region models
    public class SomtodayRooster
    {
        public int laatsteWijziging { get; set; }
        public List<Item> items { get; set; }
    }
    
    public class AdditionalObjects
    {
        public Vak vak { get; set; }
        public string docentAfkortingen { get; set; }
    }

    public class AfspraakType
    {
        public string naam { get; set; }
        public string omschrijving { get; set; }
        public int standaardKleur { get; set; }
        public string categorie { get; set; }
        public string activiteit { get; set; }
        public int percentageIIVO { get; set; }
        public bool presentieRegistratieDefault { get; set; }
        public bool actief { get; set; }
    }

    public class Item
    {
        public AdditionalObjects additionalObjects { get; set; }
        public AfspraakType afspraakType { get; set; }
        public string locatie { get; set; }
        public DateTime beginDatumTijd { get; set; }
        public DateTime eindDatumTijd { get; set; }
        public int beginLesuur { get; set; }
        public int eindLesuur { get; set; }
        public string titel { get; set; }
        public string omschrijving { get; set; }
        public bool presentieRegistratieVerplicht { get; set; }
        public bool presentieRegistratieVerwerkt { get; set; }
        public string afspraakStatus { get; set; }
        public List<object> bijlagen { get; set; }
    }

    public class Vak
    {
        public string afkorting { get; set; }
        public string naam { get; set; }
    }
    #endregion
}
