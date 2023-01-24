using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class Grades : BetterHttpClient
{
    [SerializeField, Tooltip("'*' means Application.persistentDataPath.")]
    private string savePath = "*/Grades.json";

    [ContextMenu("get Grades")]
    public SomtodayGrades getGrades(bool savedIsGood = true)
    {
        if (savedIsGood)
        {
            var savedGrades = getSavedGrades();
            if (savedGrades != null)
            {
                return savedGrades;
            }
        }
        
        if (string.IsNullOrEmpty(LocalPrefs.GetString("somtoday-access_token"))) return null;

        string baseUrl = $"{LocalPrefs.GetString("somtoday-api_url")}/rest/v1/resultaten/huidigVoorLeerling/{LocalPrefs.GetString("somtoday-student_id")}?begintNaOfOp={TimeManager.Instance.DateTime:yyyy}-01-01";
        
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Authorization", "Bearer " + LocalPrefs.GetString("somtoday-access_token"));
        headers.Add("Range", "items=0-99");
        
        return (SomtodayGrades) Get(baseUrl, headers, (response) =>
        {
            var sortedGrades = JsonConvert.DeserializeObject<SomtodayGrades>(response.downloadHandler.text) ?? new SomtodayGrades(){items = new List<Item>()};

            int total = int.Parse(response.GetResponseHeader("Content-Range").Split('/')[1]);

            int requests = Mathf.CeilToInt(total / 100f) * 100 - 1;

            for (int i = 100; i < requests; i += 100)
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("Authorization", "Bearer " + LocalPrefs.GetString("somtoday-access_token"));
                headers.Add("Range", $"items={i}-{i + 99}");

                Get(baseUrl, headers, (response) =>
                {
                    var collection = JsonConvert.DeserializeObject<SomtodayGrades>(response.downloadHandler.text)?.items;
                    if (collection != null)
                        sortedGrades.items.AddRange(collection);
                    return null;
                }, (error) =>
                {
                    AndroidUIToast.ShowToast("Er is iets fout gegaan bij het ophalen van je cijfers. Probeer het later opnieuw.");
                    return null;
                });
            }
            
            sortedGrades = Sort(sortedGrades);
            SaveGrades(sortedGrades);
            return sortedGrades;
        }, (error) =>
        {
            AndroidUIToast.ShowToast("Er is iets fout gegaan bij het ophalen van je cijfers. Probeer het later opnieuw.");
            return null;
        });
    }
    
    private SomtodayGrades getSavedGrades()
    {
        string destination = savePath.Replace("*", Application.persistentDataPath);
        
        if (!File.Exists(destination))
        {
            Debug.LogWarning("File not found, creating new file.");
            return getGrades(false);
        }
        
        using (StreamReader r = new StreamReader(destination))
        {
            string json = r.ReadToEnd();
            try
            {
                SomtodayGrades gradesObject = JsonConvert.DeserializeObject<SomtodayGrades>(json);
                if (gradesObject?.laatsteWijziging.ToDateTime().AddMinutes(10) < TimeManager.Instance.CurrentDateTime)
                {
                    r.Close();
                    Debug.LogWarning("Local file is outdated, downloading new file.");
                    return getGrades(false);
                }

                return gradesObject;
            }
            catch(Exception)
            {
                r.Close();
                Debug.LogWarning("Local file is corrupt, downloading new file.");
                return getGrades(false);
            }
        }
    }

    private void SaveGrades(SomtodayGrades grades)
    {
        if (grades.items.Count != 0)
        {
            var convertedJson = JsonConvert.SerializeObject(
                new SomtodayGrades()
                {
                    items = grades.items.OrderBy(x => x.datumInvoer).ToList(),
                    laatsteWijziging = TimeManager.Instance.CurrentDateTime.ToUnixTime()
                }, 
                Formatting.Indented);

            string destination = savePath.Replace("*", Application.persistentDataPath);

            File.WriteAllText(destination, "//In dit bestand staan alle cijfers die je dit jaar hebt gehaald.\r\n");
            File.AppendAllText(destination, convertedJson);
        }
    }

    public SomtodayGrades Sort(SomtodayGrades grades)
    {
        grades.items.RemoveAll(x => x.geldendResultaat == null);
        grades.items.RemoveAll(x => string.IsNullOrEmpty(x.omschrijving) && x.weging == 0);
        grades.items = grades.items.OrderBy(x => x.datumInvoer).ToList();
        return grades;
    }


    #region model

    public class SomtodayGrades
    {
        public int laatsteWijziging { get; set; }
        public List<Item> items { get; set; }
    }

    public class Item
    {
        //[JsonProperty("$type")] public string Type { get; set; }
        public List<Link> links { get; set; }
        public List<Permission> permissions { get; set; }
        public string herkansingstype { get; set; }
        public DateTime datumInvoer { get; set; }
        public bool teltNietmee { get; set; }
        public bool toetsNietGemaakt { get; set; }
        public int leerjaar { get; set; }
        public int periode { get; set; }
        public int weging { get; set; }
        public int examenWeging { get; set; }
        public bool isExamendossierResultaat { get; set; }
        public bool isVoortgangsdossierResultaat { get; set; }
        public string type { get; set; }
        public string omschrijving { get; set; }
        public Vak vak { get; set; }
        public int volgnummer { get; set; }
        public bool vrijstelling { get; set; }
        public string resultaat { get; set; }
        public string geldendResultaat { get; set; }
    }

    public class Link
    {
        public object id { get; set; }
        public string rel { get; set; }
        public string type { get; set; }
        public string href { get; set; }
    }

    public class Permission
    {
        public string full { get; set; }
        public string type { get; set; }
        public List<string> operations { get; set; }
        public List<string> instances { get; set; }
    }


    public class Vak
    {
        public List<Link> links { get; set; }
        public List<Permission> permissions { get; set; }
        public string afkorting { get; set; }
        public string naam { get; set; }
    }

    #endregion
}