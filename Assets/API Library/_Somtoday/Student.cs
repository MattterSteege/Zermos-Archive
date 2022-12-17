using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class Student : MonoBehaviour
{
    public SomtodayStudent getStudent(string accessToken = "")
    {
        if (accessToken == "")
        {
            accessToken = LocalPrefs.GetString("somtoday-access_token");
        }
        
        UnityWebRequest www = UnityWebRequest.Get("https://api.somtoday.nl/rest/v1/leerlingen");
        www.SetRequestHeader("Authorization", "Bearer " + accessToken);
        www.SetRequestHeader("Accept", "application/json");
        
        www.SendWebRequest();
        
        while (!www.isDone) { }
        
        SomtodayStudent student = JsonConvert.DeserializeObject<SomtodayStudent>(www.downloadHandler.text);
        
        if (student != null)
        {
            LocalPrefs.SetString("somtoday-student_id", student.items[0].links[0].id.ToString());
            return student;
        }
        return null;
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
        public List<Item> items { get; set; }
    }
    #endregion
}
