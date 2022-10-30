using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class Vakken : MonoBehaviour
{
    [SerializeField, Tooltip("'*' means Application.persistentDataPath.")]
    private string savePath = "*/Vakken.json";
    
    
    [ContextMenu("donwload vakken")]
    public void Downloadvakken()
    {
        //somtoday
        UnityWebRequest www = UnityWebRequest.Get("https://api.somtoday.nl/rest/v1/vakken");
        www.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("somtoday-access_token"));
        www.SetRequestHeader("Accept", "application/json");
        
        www.SendWebRequest();
        
        while (!www.isDone) { }
        
        SomtodayVakken vakSom = JsonConvert.DeserializeObject<SomtodayVakken>(www.downloadHandler.text);
        //somtoday -- end
        
        
        
        //zermelo
        UnityWebRequest www2 = UnityWebRequest.Get($"https://ccg.zportal.nl/api/v3/courses?year={TimeManager.Instance.DateTime.Year}");
        www2.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("zermelo-access_token"));
        www2.SetRequestHeader("Accept", "application/json");
        
        www2.SendWebRequest();
        
        while (!www2.isDone) { }
        
        ZermeloVakken vakZer = JsonConvert.DeserializeObject<ZermeloVakken>(www2.downloadHandler.text);
        //zermelo -- end
        
        // find all the similar vakken
        
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
            var convertedJson = JsonConvert.SerializeObject(vakken.OrderBy(x => x.afkorting), Formatting.Indented);

            string destination = savePath.Replace("*", Application.persistentDataPath);

            File.WriteAllText(destination, "//In dit bestand staan alle vakken die je school aanbied.\r\n");
            File.AppendAllText(destination, convertedJson);
        }
    }
    
    //https://ccg.zportal.nl/api/v3/courses?year=2022
    public SomtodayVakken getVakken()
    {
        string destination = savePath.Replace("*", Application.persistentDataPath);

        if (!File.Exists(destination))
        {
            Debug.LogError("File not found");
            return null;
        }

        using (StreamReader r = new StreamReader(destination))
        {
            string json = r.ReadToEnd();
            return new SomtodayVakken {items = JsonConvert.DeserializeObject<List<Item>>(json)};
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
