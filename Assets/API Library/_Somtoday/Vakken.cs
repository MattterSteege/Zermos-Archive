using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class Vakken : BetterHttpClient
{
    [SerializeField, Tooltip("'*' means Application.persistentDataPath.")]
    private string savePath = "*/Vakken.json";
    
    
    [ContextMenu("donwload vakken")]
    public SomtodayVakken Downloadvakken()
    {

        if (LocalPrefs.GetString("somtoday-access_token") == null || LocalPrefs.GetString("zermelo-access_token") == null)
        {
            Debug.Log("Missing token(s)");
            return null;
        }

        SomtodayVakken vakSom;
        //somtoday -- end\
        
        Dictionary<string, string> headers = new Dictionary<string,string>();
        headers.Add("Authorization", "Bearer " + LocalPrefs.GetString("somtoday-access_token"));
        return (SomtodayVakken) Get("https://api.somtoday.nl/rest/v1/vakken", headers, (response) =>
        {
            vakSom = JsonConvert.DeserializeObject<SomtodayVakken>(response.downloadHandler.text);
            
            Dictionary<string, string> headers = new Dictionary<string,string>();
            headers.Add("Authorization", "Bearer " + LocalPrefs.GetString("zermelo-access_token"));
            return (SomtodayVakken)Get($"https://ccg.zportal.nl/api/v3/courses?year={TimeManager.Instance.DateTime.Year}", headers, (response) =>
            {
                ZermeloVakken vakZer = JsonConvert.DeserializeObject<ZermeloVakken>(response.downloadHandler.text);
                
                List<Item> vakken = new List<Item>();
        
                foreach (Item somVak in vakSom.items)
                {
                    foreach (Course zerVak in vakZer.response.data[0].courses)
                    {
                        if (somVak.afkorting == zerVak.abbreviation)
                        {
                            vakken.Add(new Item {afkorting = somVak.afkorting, naam = somVak.naam});
                        }
                    }
                }

                if (vakken.Count != 0)
                {
                    var convertedJson = JsonConvert.SerializeObject(
                        new SomtodayVakken()
                        {
                            items = vakken.OrderBy(x => x.afkorting).ToList(),
                            laatsteWijziging = TimeManager.Instance.CurrentDateTime.ToUnixTime()
                        },
                        Formatting.Indented);

                    string destination = savePath.Replace("*", Application.persistentDataPath);

                    File.WriteAllText(destination, "//In dit bestand staan alle vakken die je school aanbied.\r\n");
                    File.AppendAllText(destination, convertedJson);

                    return new SomtodayVakken
                    {
                        items = vakken.OrderBy(x => x.afkorting).ToList(),
                        laatsteWijziging = TimeManager.Instance.CurrentDateTime.ToUnixTime()
                    };
                }

                return null;
            },
            (error) =>
            {
                Debug.LogWarning(error.error);
                return null;
            });
        },
        (error) =>
        {
            Debug.LogWarning(error.error);
            return null;
        });
    }
    
    public SomtodayVakken getVakken()
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
            var vakkenObject = JsonConvert.DeserializeObject<SomtodayVakken>(json);
            if (vakkenObject?.laatsteWijziging.ToDateTime().AddDays(5) < TimeManager.Instance.CurrentDateTime)
            {
                r.Close();
                Debug.LogWarning("Local file is outdated, downloading new file.");
                return Downloadvakken();
            }
            return vakkenObject;
        }
    }

    #region models - somtoday
    [Serializable]
    public class Item
    {
        public string afkorting { get; set; }
        public string naam { get; set; }
    }

    [Serializable]
    public class SomtodayVakken
    {
        public int laatsteWijziging { get; set; }
        public List<Item> items { get; set; }
    }
    #endregion
    
    #region models - zermelo
    public class Course
    {
        public string abbreviation { get; set; }
    }

    public class Datum
    {
        public List<Course> courses { get; set; }
    }

    public class Response
    {
        public List<Datum> data { get; set; }
    }

    [Serializable]
    public class ZermeloVakken
    {
        public Response response { get; set; }
    }
    #endregion
}
