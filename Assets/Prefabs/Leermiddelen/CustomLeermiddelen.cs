using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class CustomLeermiddelen : MonoBehaviour
{
    [SerializeField, Tooltip("'*' means Application.persistentDataPath.")]
    public string savePath = "*/Leermiddelen.json";

#if UNITY_EDITOR
    [ContextMenu("open explorer")]
    private void openExplorer()
    {
        Process.Start(Application.persistentDataPath);
    }
#endif

    [ContextMenu("try load")]
    public List<Item> GetLeermiddelen()
    {
        string destination = savePath.Replace("*", Application.persistentDataPath);

        if (!File.Exists(destination))
            return null;


        return LoadFile();
    }

    public void AddLeermiddelen(string url, string vak, string description)
    {
        List<Item> LeermiddelenItems = LoadFile() ?? new List<Item>();
        int id = LeermiddelenItems.Count + 1;
        LeermiddelenItems.Add(new Item(id, url, vak, description));

        var convertedJson = JsonConvert.SerializeObject(LeermiddelenItems, Formatting.Indented);

        string destination = savePath.Replace("*", Application.persistentDataPath);

        File.AppendAllText(destination, convertedJson);
    }
    
    int Tries = 0;

    private List<Item> LoadFile()
    {
        string destination = savePath.Replace("*", Application.persistentDataPath);

        if (!File.Exists(destination))
        {
            File.Create(destination).Close();
            return new List<Item>();
        }
        
        string json = File.ReadAllText(destination);
        return JsonConvert.DeserializeObject<List<Item>>(json);
    }

    public void SetLeermiddel(int id, string href = "", string vak = "", string description = "")
    {
        List<Item> LeermiddelenItems = LoadFile();
        Item itemToEdit = LeermiddelenItems.FirstOrDefault(x => x.id == id);
        if (itemToEdit != null)
        {
            if (!string.IsNullOrEmpty(href))
            {
                itemToEdit.href = href;
            }
            if (!string.IsNullOrEmpty(vak))
            {
                itemToEdit.vak = vak;
            }
            if (!string.IsNullOrEmpty(description))
            {
                itemToEdit.description = description;
            }
            var convertedJson = JsonConvert.SerializeObject(LeermiddelenItems, Formatting.Indented);
            string destination = savePath.Replace("*", Application.persistentDataPath);
            File.WriteAllText(destination, convertedJson);
        }
        else
        {
            Debug.LogError("Item with id " + id + " not found");
        }
    }

    public void DeleteLeermiddel(int id)
    {
        List<Item> LeermiddelenItems = LoadFile();
        Item itemToDelete = LeermiddelenItems.FirstOrDefault(x => x.id == id);
        if (itemToDelete != null)
        {
            LeermiddelenItems.Remove(itemToDelete);
            var convertedJson = JsonConvert.SerializeObject(LeermiddelenItems, Formatting.Indented);
            string destination = savePath.Replace("*", Application.persistentDataPath);
            File.WriteAllText(destination, convertedJson);
        }
        else
        {
            Debug.LogError("Item with id " + id + " not found");
        }
    }
    
    //delete/id
    
    // public void SetLeermiddelen(int id, string url, string vak)
    // {
    //     List<Item> LeermiddelenItems = JsonConvert.DeserializeObject<List<Item>>(LoadFile());
    //     var temp = LeermiddelenItems.Where(x => x.id == id).First();
    //     temp.href = url;
    //     temp.vak = vak;
    //
    //     var convertedJson = JsonConvert.SerializeObject(LeermiddelenItems, Formatting.Indented);
    //
    //     string destination = savePath.Replace("*", Application.persistentDataPath);
    //
    //     File.WriteAllText(destination, "//In dit bestand staan alle leermiddelen linkjes.\r\n");
    //     File.AppendAllText(destination, convertedJson);
    // }
    //
    // public void DeleteLeermiddelen(int id)
    // {
    //     List<Item> LeermiddelenItems = JsonConvert.DeserializeObject<List<Item>>(LoadFile());
    //     LeermiddelenItems.RemoveAll(x => x.id == id);
    //
    //     var convertedJson = JsonConvert.SerializeObject(LeermiddelenItems, Formatting.Indented);
    //
    //     string destination = savePath.Replace("*", Application.persistentDataPath);
    //     
    //     File.AppendAllText(destination, convertedJson);
    // }
    // public void SaveFile(string url, string vak)
    // {
    //     string destination = savePath.Replace("*", Application.persistentDataPath);
    //     FileStream file;
    //
    //     if (!File.Exists(destination))
    //     {
    //         File.Create(destination).Dispose();
    //         using (StreamWriter writer = new StreamWriter(destination))
    //         {
    //             writer.WriteLine("//In dit bestand staan alle leermiddelen linkjes.");
    //             writer.WriteLine("[]");
    //         }
    //     }
    //
    //
    //     var list = JsonConvert.DeserializeObject<List<Item>>(LoadFile());
    //
    //     Item data = new Item(list.Count, url, vak);
    //     list.Add(data);
    //     var convertedJson = JsonConvert.SerializeObject(list, Formatting.Indented);
    //
    //     file = File.OpenWrite(destination);
    //
    //     using (StreamWriter writer = new StreamWriter(file))
    //     {
    //         writer.WriteLine("//In dit bestand staan alle leermiddelen linkjes.");
    //         writer.Write(convertedJson);
    //     }
    // }
    //
    // public string LoadFile()
    // {
    //     string destination = savePath.Replace("*", Application.persistentDataPath);
    //
    //     if (!File.Exists(destination))
    //     {
    //         Debug.LogError("File not found");
    //         return "";
    //     }
    //
    //     using (StreamReader r = new StreamReader(destination))
    //     {
    //         string json = r.ReadToEnd();
    //         return json;
    //     }
    // }

    [Serializable]
    public class Item
    {

        public Item(int id, string href, string vak, string description)
        {
            this.id = id;
            this.href = href;
            this.vak = vak;
            this.description = description;
        }

        public int id { get; set; }
        public string href { get; set; }
        public string description { get; set; }
        public string vak { get; set; }
    }

    public class SomtodayLeermiddelen
    {
        public int laatsteWijziging { get; set; }
        public List<Item> items { get; set; }
    }
}