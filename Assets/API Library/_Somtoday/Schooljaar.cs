using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class Schooljaar : BetterHttpClient
{
    [SerializeField, Tooltip("'*' means Application.persistentDataPath.")]
    public string savePath = "*/SchoolYears.json";

    public DateTime getCurrentSchooljaarStartDate()
    {
        string destination = savePath.Replace("*", Application.persistentDataPath);

        if (!File.Exists(destination))
        {
            Debug.LogWarning("File not found, creating new file.");
            return DateTime.Parse(DownloadSchooljaar().vanafDatum);
        }

        using (StreamReader r = new StreamReader(destination))
        {
            string json = r.ReadToEnd();
            var vakkenObject = JsonConvert.DeserializeObject<SomtodaySchooljaar>(json);
            if (vakkenObject?.laatsteWijziging.ToDateTime().AddDays(28) < TimeManager.Instance.CurrentDateTime)
            {
                r.Close();
                Debug.LogWarning("Local file is outdated, downloading new file.");
                return DateTime.Parse(DownloadSchooljaar().vanafDatum);
            }
            return DateTime.Parse(vakkenObject.vanafDatum);
        }
    }
    
    public DateTime getCurrentSchooljaarEndDate()
    {
        string destination = savePath.Replace("*", Application.persistentDataPath);

        if (!File.Exists(destination))
        {
            Debug.LogWarning("File not found, creating new file.");
            return DateTime.Parse(DownloadSchooljaar().totDatum);
        }

        using (StreamReader r = new StreamReader(destination))
        {
            string json = r.ReadToEnd();
            var vakkenObject = JsonConvert.DeserializeObject<SomtodaySchooljaar>(json);
            if (vakkenObject?.laatsteWijziging.ToDateTime().AddDays(28) < TimeManager.Instance.CurrentDateTime)
            {
                r.Close();
                Debug.LogWarning("Local file is outdated, downloading new file.");
                return DateTime.Parse(DownloadSchooljaar().totDatum);
            }
            return DateTime.Parse(vakkenObject.totDatum);;
        }
    }
    
    public SomtodaySchooljaar DownloadSchooljaar()
    {
        if (LocalPrefs.GetString("somtoday-access_token") == null)
            return null;
        
        Dictionary<string, string> headers = new Dictionary<string,string>();
        headers.Add("Authorization", "Bearer " + LocalPrefs.GetString("somtoday-access_token"));
        return (SomtodaySchooljaar) Get("https://api.somtoday.nl/rest/v1/schooljaren/huidig", headers, (response) =>
        {
            SomtodaySchooljaar schooljaar = JsonConvert.DeserializeObject<SomtodaySchooljaar>(response.downloadHandler.text);
            schooljaar.laatsteWijziging = TimeManager.Instance.CurrentDateTime.ToUnixTime();
            
            var convertedJson = JsonConvert.SerializeObject(schooljaar, Formatting.Indented);

            string destination = savePath.Replace("*", Application.persistentDataPath);

            File.WriteAllText(destination, "//In dit bestand staan alle vakken die je school aanbied.\r\n");
            File.AppendAllText(destination, convertedJson);

            return schooljaar;
        });
    }

    #region models
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

    public class SomtodaySchooljaar
    {
        public int laatsteWijziging { get; set; }
        [JsonProperty("$type")]
        public string type { get; set; }
        public List<Link> links { get; set; }
        public List<Permission> permissions { get; set; }
        public string naam { get; set; }
        public string vanafDatum { get; set; }
        public string totDatum { get; set; }
        public bool isHuidig { get; set; }
    }
    #endregion
}
