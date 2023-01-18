using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class Vakken : BetterHttpClient
{
    [SerializeField, Tooltip("'*' means Application.persistentDataPath.")]
    private string savePath = "*/Subjects.json";

    [ContextMenu("donwload vakken")]
    public JsonSomtodayVakken Downloadvakken()
    {

        if (LocalPrefs.GetString("somtoday-access_token") == null)
        {
            Debug.Log("Missing SOMtoday token");
            return null;
        }
        
        //somtoday -- end\
        
        // Dictionary<string, string> headers = new Dictionary<string,string>();
        // headers.Add("Authorization", "Bearer " + LocalPrefs.GetString("somtoday-access_token"));
        // return (SomtodayVakken) Get("https://api.somtoday.nl/rest/v1/vakken", headers, (response) =>
        // {
        //     vakSom = JsonConvert.DeserializeObject<SomtodayVakken>(response.downloadHandler.text);
        //     
        //     Dictionary<string, string> headers = new Dictionary<string,string>();
        //     headers.Add("Authorization", "Bearer " + LocalPrefs.GetString("zermelo-access_token"));
        //     return (SomtodayVakken)Get($"https://ccg.zportal.nl/api/v3/courses?schoolYear={GetComponent<Schooljaar>().getCurrentSchooljaarStartDate().Year}&student=~me", headers, (response) =>
        //     {
        //         ZermeloVakken vakZer = JsonConvert.DeserializeObject<ZermeloVakken>(response.downloadHandler.text);
        //         
        //         List<Item> vakken = new List<Item>();
        //
        //         foreach (Item somVak in vakSom.items)
        //         {
        //             foreach (Datum zerVak in vakZer.response.data)
        //             {
        //                 if (somVak.afkorting == zerVak.subjectCode)
        //                 {
        //                     vakken.Add(new Item {afkorting = somVak.afkorting, naam = somVak.naam});
        //                 }
        //             }
        //         }
        //
        //         if (vakken.Count != 0)
        //         {
        //             var convertedJson = JsonConvert.SerializeObject(
        //                 new SomtodayVakken()
        //                 {
        //                     items = vakken.OrderBy(x => x.afkorting).ToList(),
        //                     laatsteWijziging = TimeManager.Instance.CurrentDateTime.ToUnixTime()
        //                 },
        //                 Formatting.Indented);
        //
        //             string destination = savePath.Replace("*", Application.persistentDataPath);
        //
        //             File.WriteAllText(destination, "//In dit bestand staan alle vakken die je school aanbied.\r\n");
        //             File.AppendAllText(destination, convertedJson);
        //
        //             return new SomtodayVakken
        //             {
        //                 items = vakken.OrderBy(x => x.afkorting).ToList(),
        //                 laatsteWijziging = TimeManager.Instance.CurrentDateTime.ToUnixTime()
        //             };
        //         }
        //
        //         return null;
        //     },
        //     (error) =>
        //     {
        //         Debug.LogWarning(error.error);
        //         return null;
        //     });
        // },
        // (error) =>
        // {
        //     Debug.LogWarning(error.error);
        //     return null;
        // });
        
        Dictionary<string, string> headers = new Dictionary<string,string>();
        headers.Add("Authorization", "Bearer " + LocalPrefs.GetString("somtoday-access_token"));
        return (JsonSomtodayVakken) Get($"https://api.somtoday.nl/rest/v1/vakkeuzes?actiefOpPeildatum={TimeManager.Instance.CurrentDateTime:yyyy-MM-dd}", headers, (response) =>
        {
            var vakken = JsonConvert.DeserializeObject<SomtodayVakken>(response.downloadHandler.text)?.items;
            
            if (vakken.Count != 0)
            {
                
                List<JsonItem> jsonItems = new List<JsonItem>();
                foreach (Item vak in vakken)
                {
                    jsonItems.Add(new JsonItem {afkorting = vak.vak.afkorting, naam = vak.vak.naam});
                }
                
                var convertedJson = JsonConvert.SerializeObject(
                new JsonSomtodayVakken
                    {
                        items = jsonItems.OrderBy(x => x.afkorting).ToList(),
                        laatsteWijziging = TimeManager.Instance.CurrentDateTime.ToUnixTime()
                    }, 
                    Formatting.Indented);
            
                string destination = savePath.Replace("*", Application.persistentDataPath);
            
                File.WriteAllText(destination, "//In dit bestand staan alle vakken die je school aanbied.\r\n");
                File.AppendAllText(destination, convertedJson);
            
                return new JsonSomtodayVakken
                {
                    items = jsonItems.OrderBy(x => x.afkorting).ToList(),
                    laatsteWijziging = TimeManager.Instance.CurrentDateTime.ToUnixTime()
                };
            }
            
            return null;
        });
    }
    
    public JsonSomtodayVakken getVakken()
    {
        string destination = savePath.Replace("*", Application.persistentDataPath);

        if (!File.Exists(destination))
        {
            Debug.LogWarning("File not found, creating new file.");
            return Downloadvakken();
        }

        using (StreamReader r = new StreamReader(destination))
        {
            string json = r.ReadToEnd();
            var vakkenObject = JsonConvert.DeserializeObject<JsonSomtodayVakken>(json);
            if (vakkenObject?.laatsteWijziging.ToDateTime().AddDays(7) < TimeManager.Instance.CurrentDateTime)
            {
                r.Close();
                Debug.LogWarning("Local file is outdated, downloading new file.");
                return Downloadvakken();
            }
            return vakkenObject;
        }
    }

    #region models - somtoday
    public class SomtodayVakken
    {
        public List<Item> items { get; set; }
    }
    
    public class Item
    {
        public Vak vak { get; set; }
        public bool vrijstelling { get; set; }
        public Lichting lichting { get; set; }
        public RelevanteCijferLichting relevanteCijferLichting { get; set; }
    }

    public class Lichting
    {
        public List<Permission> permissions { get; set; }
        public string naam { get; set; }
        public List<LichtingSchooljaren> lichtingSchooljaren { get; set; }
        public Onderwijssoort onderwijssoort { get; set; }
    }

    public class LichtingSchooljaren
    {
        public List<object> permissions { get; set; }
        public Schooljaar schooljaar { get; set; }
        public int leerjaar { get; set; }
        public bool heeftExamendossier { get; set; }
    }

    public class Onderwijssoort
    {
        public List<object> permissions { get; set; }
        public string afkorting { get; set; }
        public bool isOnderbouw { get; set; }
    }

    public class Permission
    {
        public string full { get; set; }
        public string type { get; set; }
        public List<string> operations { get; set; }
        public List<string> instances { get; set; }
    }

    public class RelevanteCijferLichting
    {
        public List<Permission> permissions { get; set; }
        public string naam { get; set; }
        public List<LichtingSchooljaren> lichtingSchooljaren { get; set; }
        public Onderwijssoort onderwijssoort { get; set; }
    }

    public class Schooljaar
    {
        public List<Permission> permissions { get; set; }
        public string naam { get; set; }
        public string vanafDatum { get; set; }
        public string totDatum { get; set; }
        public bool isHuidig { get; set; }
    }

    public class Vak
    {
        public List<Permission> permissions { get; set; }
        public string afkorting { get; set; }
        public string naam { get; set; }
    }
    
    public class JsonSomtodayVakken
    {
        public int laatsteWijziging { get; set; }
        public List<JsonItem> items { get; set; }
    }
    
    public class JsonItem
    {
        public string afkorting { get; set; }
        public string naam { get; set; }
    }
    #endregion
}
