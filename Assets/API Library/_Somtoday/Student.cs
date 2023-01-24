using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class Student : BetterHttpClient
{
    [SerializeField, Tooltip("'*' means Application.persistentDataPath.")]
    private string savePath = "*/Student.json";
    
    public SomtodayStudent getStudent(bool fetchNew = false)
    {
        string destination = savePath.Replace("*", Application.persistentDataPath);

        if (!File.Exists(destination) || fetchNew)
        {
            Debug.LogWarning("File not found, creating new file.");
            return DownloadStudent();
        }

        using (StreamReader r = new StreamReader(destination))
        {
            string json = r.ReadToEnd();
            var vakkenObject = JsonConvert.DeserializeObject<SomtodayStudent>(json);
            if (vakkenObject?.laatsteWijziging.ToDateTime().AddDays(50) < TimeManager.Instance.CurrentDateTime)
            {
                r.Close();
                Debug.LogWarning("Local file is outdated, downloading new file.");
                return DownloadStudent();
            }
            return vakkenObject;
        }
    }
    
    public SomtodayStudent DownloadStudent()
    {
        if (LocalPrefs.GetString("somtoday-access_token") == null)
            return null;

        Dictionary<string, string> headers = new Dictionary<string,string>();
        headers.Add("Authorization", "Bearer " + LocalPrefs.GetString("somtoday-access_token"));
        return (SomtodayStudent)Get("https://api.somtoday.nl/rest/v1/leerlingen", headers, (response) =>
        {

            SomtodayStudent student = JsonConvert.DeserializeObject<SomtodayStudent>(response.downloadHandler.text);
        
            if (student != null)
            {
                LocalPrefs.SetString("somtoday-student_id", student.items[0].links[0].id.ToString());
                
                var convertedJson = JsonConvert.SerializeObject(
                    new SomtodayStudent()
                    {
                        items = student.items,
                        laatsteWijziging = TimeManager.Instance.CurrentDateTime.ToUnixTime()
                    },
                    Formatting.Indented);

                string destination = savePath.Replace("*", Application.persistentDataPath);

                File.WriteAllText(destination, "//In dit bestand staat al jouw informatie.\r\n");
                File.AppendAllText(destination, convertedJson);

                return student;
            }
            return null;        
        }, (error) =>
        {
            AndroidUIToast.ShowToast("Er is iets misgegaan tijdens het opvragen van je gegevens. Probeer het later opnieuw.");
            return null;
        });
    }
    
    #region models
    public class Item
    {
        [JsonProperty("$type")]
        public string Type { get; set; }
        public List<Link> links { get; set; }
        public List<Permission> permissions { get; set; }
        public string UUID { get; set; }
        public int leerlingnummer { get; set; }
        public string roepnaam { get; set; }
        public string voorvoegsel { get; set; }
        public string achternaam { get; set; }
        public string email { get; set; }
        public string geboortedatum { get; set; }
        public string geslacht { get; set; }
    }

    public class Link
    {
        public int id { get; set; }
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

    public class SomtodayStudent
    {
        public int laatsteWijziging { get; set; }
        public List<Item> items { get; set; }
    }
    #endregion
}
