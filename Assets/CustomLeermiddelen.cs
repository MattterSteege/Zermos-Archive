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
    public List<LeermiddelenItem> GetLeermiddelen()
    {
        List<LeermiddelenItem> remapedLeermiddel = new List<LeermiddelenItem>();
        return remapedLeermiddel;
    }

    public void SetLeermiddelen(int id, string url, string vak)
    {
        List<LeermiddelenItem> LeermiddelenItems = JsonConvert.DeserializeObject<List<LeermiddelenItem>>(LoadFile());
        var temp = LeermiddelenItems.Where(x => x.id == id).First();
        temp.url = url;
        temp.vak = vak;

        var convertedJson = JsonConvert.SerializeObject(LeermiddelenItems, Formatting.Indented);

        string destination = savePath.Replace("*", Application.persistentDataPath);

        File.WriteAllText(destination, "//In dit bestand staan alle leermiddelen linkjes.\r\n");
        File.AppendAllText(destination, convertedJson);
    }

    public void DeleteLeermiddelen(int id)
    {
        List<LeermiddelenItem> LeermiddelenItems =
            JsonConvert.DeserializeObject<List<LeermiddelenItem>>(LoadFile());
        LeermiddelenItems.RemoveAll(x => x.id == id);

        var convertedJson = JsonConvert.SerializeObject(LeermiddelenItems, Formatting.Indented);

        string destination = savePath.Replace("*", Application.persistentDataPath);

        File.WriteAllText(destination, "//In dit bestand staan alle leermiddelen linkjes.\r\n");
        File.AppendAllText(destination, convertedJson);
    }


    public void SaveFile(string url, string vak)
    {
        string destination = savePath.Replace("*", Application.persistentDataPath);
        PlayerPrefs.SetString("file_path", destination);
        FileStream file;

        if (!File.Exists(destination))
        {
            File.Create(destination).Dispose();
            using (StreamWriter writer = new StreamWriter(destination))
            {
                writer.WriteLine("//In dit bestand staan alle leermiddelen linkjes.");
                writer.WriteLine("[]");
            }
        }


        var list = JsonConvert.DeserializeObject<List<LeermiddelenItem>>(LoadFile());

        LeermiddelenItem data = new LeermiddelenItem(list.Count, url, vak);
        list.Add(data);
        var convertedJson = JsonConvert.SerializeObject(list, Formatting.Indented);

        file = File.OpenWrite(destination);

        using (StreamWriter writer = new StreamWriter(file))
        {
            writer.WriteLine("//In dit bestand staan alle leermiddelen linkjes.");
            writer.Write(convertedJson);
        }
    }

    public string LoadFile()
    {
        string destination = savePath.Replace("*", Application.persistentDataPath);

        if (!File.Exists(destination))
        {
            Debug.LogError("File not found");
            return "";
        }

        using (StreamReader r = new StreamReader(destination))
        {
            string json = r.ReadToEnd();
            return json;
        }
    }

    [Serializable]
    public class LeermiddelenItem
    {
        public LeermiddelenItem(int id, string url, string vak)
        {
            this.id = id;
            this.url = url;
            this.vak = vak;
        }

        public int id;
        public string url;
        public string vak;
    }
}