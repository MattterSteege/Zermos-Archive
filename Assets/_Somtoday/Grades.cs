using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class Grades : MonoBehaviour
{
    [SerializeField] private Student Student;
    [SerializeField] private AuthenticateSomtoday authentication;

    [ContextMenu("get Grades")]
    public SomtodayGrades getGrades()
    {
        if (PlayerPrefs.GetString("somtoday-access_token") == "")
        {
            return null;
        }
        
        Student.getStudent();
        authentication.AuthenticateUser(PlayerPrefs.GetString("somtoday-tenant_uuid"),PlayerPrefs.GetString("somtoday-username"), PlayerPrefs.GetString("somtoday-password"));
        
        string baseurl = string.Format("{0}/rest/v1/resultaten/huidigVoorLeerling/{1}",
            PlayerPrefs.GetString("somtoday-api_url"), PlayerPrefs.GetString("somtoday-student_id"));

        baseurl += $"?begintNaOfOp={DateTime.Now.ToString("yyyy")}-01-01";
        
        UnityWebRequest www = UnityWebRequest.Get(baseurl);
        www.SetRequestHeader("authorization", "Bearer " + PlayerPrefs.GetString("somtoday-access_token"));
        www.SetRequestHeader("Accept", "application/json");

        www.SendWebRequest();

        while (!www.isDone)
        {
        }
        
        return JsonConvert.DeserializeObject<SomtodayGrades>(www.downloadHandler.text);
    }
    
    private void CopyToClipboard(string str) {
        TextEditor textEditor = new TextEditor();
        textEditor.text = str;
        textEditor.SelectAll();
        textEditor.Copy();
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