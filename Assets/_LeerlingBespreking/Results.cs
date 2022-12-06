using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class Results : MonoBehaviour
{
    [ContextMenu("Test")]
    public void Test()
    {
        GetResults("102686");
    }

    
    public List<LeerlingBesprekingResults> GetResults(string userId)
    {
        UnityWebRequest www = UnityWebRequest.Get("https://laravel.leerlingbespreking.nl/api/student/" + userId + "/ratings");   
        www.SetRequestHeader("Authorization", "Bearer " + LocalPrefs.GetString("leerlingbespreking-access_token"));
        www.SetRequestHeader("Accept", "application/json");
        
        www.SendWebRequest();

        while (!www.isDone) { }

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            return null;
        }
        var response = JsonConvert.DeserializeObject<List<LeerlingBesprekingResults>>(www.downloadHandler.text);
        return response;
    }
    
    public class Item
    {
        public int rating_id { get; set; }
        public string subject_shortage { get; set; }
        public string teacher_shortage { get; set; }
        public int? rating { get; set; }
        public string rating_remark { get; set; }
        public string rating_action { get; set; }
        public bool has_pva { get; set; }
        public List<object> action_plan { get; set; }
    }

    public class Report
    {
        public int id { get; set; }
        public int school_id { get; set; }
        public int meeting_id { get; set; }
        public int student_id { get; set; }
        public int visible_for_student { get; set; }
        public int invite { get; set; }
        public int feedback_seen { get; set; }
        public object summary { get; set; }
        public object student_summary { get; set; }
        public string unique_id { get; set; }
        public object temp_student_number { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public List<Summary> summaries { get; set; }
    }

    public class LeerlingBesprekingResults
    {
        public int id { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public string mail_student_summary_text { get; set; }
        public string mail_student_text { get; set; }
        public List<Item> items { get; set; }
        public Report report { get; set; }
    }

    public class Summary
    {
        public int id { get; set; }
        public int report_id { get; set; }
        public int mentor_user_id { get; set; }
        public string teacher_shortage { get; set; }
        public string summary { get; set; }
        public object created_at { get; set; }
        public object updated_at { get; set; }
        public object temp_meeting_id { get; set; }
        public object temp_student_number { get; set; }
    }


}
