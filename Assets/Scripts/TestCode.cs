using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using Debug = UnityEngine.Debug;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Converters;

public class TestCode : MonoBehaviour
{
    [SerializeField, Tooltip("'*' means Application.persistentDataPath.")]
    public string savePath = "*/test.json";

#if UNITY_EDITOR
    [ContextMenu("open explorer")]
    private void openExplorer()
    {
        Process.Start(Application.persistentDataPath);
    }
#endif

    [ContextMenu("try load")]
    public void LoadFile()
    {
        string destination = savePath.Replace("*", Application.persistentDataPath);

        if (!File.Exists(destination))
        {
            Debug.LogError("File not found");
            return;
        }

        using (StreamReader r = new StreamReader(destination))
        {
            string json = r.ReadToEnd();
            var items = JsonConvert.DeserializeObject<SomtodayRooster>(json);
            IOrderedEnumerable<Item> OrderedRooster = items.items.OrderBy(x => x.beginDatumTijd);
            // foreach (var item in OrderedRooster)
            // {
            //     Debug.Log(item.titel + " - " + item.beginDatumTijd + " - " + item.eindDatumTijd + " - " + item.beginLesuur);
            // }
            
            r.Close();
            
            //write to file
            var convertedJson = JsonConvert.SerializeObject(OrderedRooster, Formatting.Indented);
            File.WriteAllText(destination, convertedJson);
        }
    }

    #region models
    public class SomtodayRooster
    {
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

