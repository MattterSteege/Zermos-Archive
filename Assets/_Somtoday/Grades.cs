using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class Grades : MonoBehaviour
{
    [SerializeField] private AuthenticateSomtoday authentication;

    [ContextMenu("get Grades")]
    public SomtodayGrades getGrades()
    {
        if (PlayerPrefs.GetString("somtoday-access_token") == "")
        {
            return null;
        }

        string json = "";
        
        int rangemin = 0;
        int rangemax = 99;
        
        string baseurl = string.Format($"{PlayerPrefs.GetString("somtoday-api_url")}/rest/v1/resultaten/huidigVoorLeerling/{PlayerPrefs.GetString("somtoday-student_id")}?begintNaOfOp={TimeManager.Instance.DateTime:yyyy}-01-01");

        UnityWebRequest www = UnityWebRequest.Get(baseurl);
        www.SetRequestHeader("authorization", "Bearer " + PlayerPrefs.GetString("somtoday-access_token"));
        www.SetRequestHeader("Accept", "application/json");
        www.SetRequestHeader("Range", $"items={rangemin}-{rangemax}");
        www.SendWebRequest();

        while (!www.isDone)
        {
        }
        
        json = www.downloadHandler.text;
        
        var header = www.GetResponseHeader("Content-Range");
        var total = int.Parse(header.Split('/')[1]);

        while (rangemax < total)
        {
            rangemin += 100;
            rangemax += 100;
            baseurl = string.Format($"{PlayerPrefs.GetString("somtoday-api_url")}/rest/v1/resultaten/huidigVoorLeerling/{PlayerPrefs.GetString("somtoday-student_id")}?begintNaOfOp={TimeManager.Instance.DateTime:yyyy}-01-01");

            www = UnityWebRequest.Get(baseurl);
            www.SetRequestHeader("authorization", "Bearer " + PlayerPrefs.GetString("somtoday-access_token"));
            www.SetRequestHeader("Accept", "application/json");
            www.SetRequestHeader("Range", $"items={rangemin}-{rangemax}");
            www.SendWebRequest();

            while (!www.isDone)
            {
            }
            
            json += www.downloadHandler.text;
        }
        
        return JsonConvert.DeserializeObject<SomtodayGrades>(www.downloadHandler.text);
    }

    #region model
    public class SomtodayGrades
    {
        public List<Item> items { get; set; }
    }
    
    public class Item
    {
        [JsonProperty("$type")]
        public string Type { get; set; }
        public List<Link> links { get; set; }
        public List<Permission> permissions { get; set; }
        public string herkansingstype { get; set; }
        public DateTime datumInvoer { get; set; }
        public bool teltNietmee { get; set; }
        public bool toetsNietGemaakt { get; set; }
        public int leerjaar { get; set; }
        public int periode { get; set; }
        public int weging { get; set; }
        public int examenWeging { get; set; }
        public bool isExamendossierResultaat { get; set; }
        public bool isVoortgangsdossierResultaat { get; set; }
        public string type { get; set; }
        public string omschrijving { get; set; }
        public Vak vak { get; set; }
        public int volgnummer { get; set; }
        public bool vrijstelling { get; set; }
        public string resultaat { get; set; }
        public string geldendResultaat { get; set; }
    }

    public class Link
    {
        public object id { get; set; }
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


    public class Vak
    {
        public List<Link> links { get; set; }
        public List<Permission> permissions { get; set; }
        public string afkorting { get; set; }
        public string naam { get; set; }
    }
    #endregion
}