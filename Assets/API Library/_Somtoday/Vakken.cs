using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class Vakken : BetterHttpClient
{
    [SerializeField, Tooltip("'*' means Application.persistentDataPath.")] private string savePath = "*/Subjects.json";
    [SerializeField] private int DagenBeforeRedownload = 7;
    

    public SomtodayVakken getVakken(bool savedIsGood = true)
    {
        string destination = savePath.Replace("*", Application.persistentDataPath);

        if (!File.Exists(destination))
        {
            Debug.LogWarning("File not found, creating new file.");
            return Downloadvakken();
        }

        try
        {
            using (StreamReader r = new StreamReader(destination))
            {
                string json = r.ReadToEnd();
                var vakkenObject = JsonConvert.DeserializeObject<SomtodayVakken>(json);
                if (vakkenObject?.laatsteWijziging.ToDateTime().AddDays(DagenBeforeRedownload) < TimeManager.Instance.CurrentDateTime || savedIsGood == false)
                {
                    r.Close();
                    Debug.LogWarning("Local file is outdated, downloading new file.");
                    return Downloadvakken();
                }

                return vakkenObject;
            }
        }
        catch (Exception)
        {
            return Downloadvakken();
        }
    }

    public SomtodayVakken Downloadvakken()
    {

        if (LocalPrefs.GetString("somtoday-access_token") == null)
        {
            Debug.Log("Missing SOMtoday token");
            return null;
        }

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Authorization", "Bearer " + LocalPrefs.GetString("somtoday-access_token"));
        return (SomtodayVakken) Get($"https://api.somtoday.nl/rest/v1/vakkeuzes?actiefOpPeildatum={TimeManager.Instance.CurrentDateTime:yyyy-MM-dd}", headers, (response) =>
        {
            var vakken = JsonConvert.DeserializeObject<SomtodayVakken>(response.downloadHandler.text);

            if (vakken.items.Count != 0)
            {
                vakken.items = vakken.items.OrderBy(x => x.vak.naam).ToList();

                var convertedJson = JsonConvert.SerializeObject(vakken, Formatting.Indented);

                string destination = savePath.Replace("*", Application.persistentDataPath);
                
                File.WriteAllText(destination, convertedJson);

                return vakken;
            }

            return null;
        }, (error) =>
        {
            AndroidUIToast.ShowToast(
                "Er is iets mis gegaan bij het opvragen van de vakken. hierdoor zullen aardig wat delen van de app niet meer werken. probeer over 10 minuten opnieuw de app op te starten.");
            return null;
        });
    }


    #region models - somtoday

    public class SomtodayVakken
    {
        public int laatsteWijziging { get; set; }
        public List<Item> items { get; set; }
    }

    public class Item
    {
        public Vak vak { get; set; }
    }

    public class Vak
    {
        public string afkorting { get; set; }
        public string naam { get; set; }
    }

    #endregion
}