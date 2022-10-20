using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using UnityEngine;

public class CustomHomework : MonoBehaviour
{
    [SerializeField, Tooltip("'*' means Application.persistentDataPath.")] private string savePath = "*/CustomHomework.json";

    [ContextMenu("try save")]
    public void AddCustomHomeWorkItem()
    {
        SaveFile("titel", "omschrijving", new DateTime(2022, 10, 25), false);
    }
    
    [ContextMenu("try load")]
    public List<Homework.Item> GetCustomHomeWork()
    {
        List<Homework.Item> remapedHomework = new List<Homework.Item>();
        List<CustomHomeworkItem> customHomeworkItems = JsonConvert.DeserializeObject<List<CustomHomeworkItem>>(LoadFile()) ?? new List<CustomHomeworkItem>();
        
        foreach (CustomHomeworkItem customHomeworkItem in customHomeworkItems)
        {
            remapedHomework.Add(new Homework.Item()
            {
                datumTijd = customHomeworkItem.deadline,
                studiewijzerItem = new Homework.StudiewijzerItem()
                {
                    omschrijving = customHomeworkItem.omschrijving
                },
                lesgroep = new Homework.Lesgroep()
                {
                    vak = new Homework.Vak()
                    {
                        naam = customHomeworkItem.titel
                    }
                },
                additionalObjects = new Homework.AdditionalObjects()
                {
                    swigemaaktVinkjes = new Homework.SwigemaaktVinkjes()
                    {
                        items = new List<Homework.Item>()
                        {
                            new()
                            {
                                gemaakt = customHomeworkItem.gemaakt
                            }
                        }
                    }
                },
                gemaakt = true,
                UUID = customHomeworkItem.id.ToString()
            });
        }

        return remapedHomework;
    }

    public void SetCustomHomework(int id, string titel, string omschrijving, DateTime deadline, bool gemaakt)
    {
        List<CustomHomeworkItem> customHomeworkItems = JsonConvert.DeserializeObject<List<CustomHomeworkItem>>(LoadFile());
        var temp = customHomeworkItems.Where(x => x.id == id).First();
        temp.titel = titel;
        temp.omschrijving = omschrijving;
        temp.deadline = deadline;
        temp.gemaakt = gemaakt;
        
        var convertedJson = JsonConvert.SerializeObject(customHomeworkItems, Formatting.Indented);

        string destination = savePath.Replace("*", Application.persistentDataPath);
        
        File.WriteAllText(destination,"//In dit bestand staan alle zelf aangemaakte huiswerk items.\r\n");
        File.AppendAllText(destination, convertedJson);
    }


    public void SaveFile(string titel, string omschrijving, DateTime deadline, bool gemaakt)
    {
        string destination = savePath.Replace("*", Application.persistentDataPath);
        FileStream file;

        if (!File.Exists(destination))
        {
            File.Create(destination).Dispose();
            using (StreamWriter writer = new StreamWriter(destination))
            {
                writer.WriteLine("//In dit bestand staan alle zelf aangemaakte huiswerk items.");
                writer.WriteLine("[]");
            }
        }

        
        var list = JsonConvert.DeserializeObject<List<CustomHomeworkItem>>(LoadFile());
        
        CustomHomeworkItem data = new CustomHomeworkItem(titel, omschrijving, deadline, gemaakt, list.Count);
        list.Add(data);
        var convertedJson = JsonConvert.SerializeObject(list, Formatting.Indented);

        file = File.OpenWrite(destination);
        
        using (StreamWriter writer = new StreamWriter(file))
        {
            writer.WriteLine("//In dit bestand staan alle zelf aangemaakte huiswerk items.");
            writer.Write(convertedJson);
        }
    }
 
    public string LoadFile()
    {
        string destination = savePath.Replace("*", Application.persistentDataPath);

        if(!File.Exists(destination))
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
    public class CustomHomeworkItem
    {
        public string titel;
        public string omschrijving;
        public DateTime deadline;
        public bool gemaakt;
        public int id;
        
        public CustomHomeworkItem(string titel, string omschrijving, DateTime deadline, bool gemaakt, int id = -1)
        {
            this.titel = titel;
            this.omschrijving = omschrijving;
            this.deadline = deadline;
            this.gemaakt = gemaakt;
            this.id = id;
        }
    }
}
