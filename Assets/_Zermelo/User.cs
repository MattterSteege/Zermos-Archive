using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class User : MonoBehaviour
{
    public ZermeloUser startGetUser()
    {
        return new CoroutineWithData<ZermeloUser>(this, GetUser()).result;
    }

    public IEnumerator GetUser() 
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("zermelo-access_token")))
        {
            yield break;
        }
        
        string baseURL = "https://{school}.zportal.nl/api/v3/users/~me?access_token={access_token}";
        
        baseURL = baseURL.Replace("{school}", PlayerPrefs.GetString("zermelo-school_code"));
        baseURL = baseURL.Replace("{access_token}", PlayerPrefs.GetString("zermelo-access_token"));

        UnityWebRequest www = UnityWebRequest.Get(baseURL);
        www.SendWebRequest();
        
        while (!www.isDone) { }
 
        if(www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        }
        else 
        {
            ZermeloUser response = JsonConvert.DeserializeObject<ZermeloUser>(www.downloadHandler.text);

            PlayerPrefs.SetString("zermelo-user_code", response.Response.Data[0].Code);
            PlayerPrefs.SetString("zermelo-full_name", response.Response.Data[0].FirstName + " " + response.Response.Data[0].Prefix + " " + response.Response.Data[0].LastName);
            PlayerPrefs.Save();

            yield return response;
        }
    }
    
    public class Datum
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("roles")]
        public List<object> Roles { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("schoolInSchoolYears")]
        public List<int> SchoolInSchoolYears { get; set; }

        [JsonProperty("isApplicationManager")]
        public bool IsApplicationManager { get; set; }

        [JsonProperty("archived")]
        public bool Archived { get; set; }

        [JsonProperty("isStudent")]
        public bool IsStudent { get; set; }

        [JsonProperty("isEmployee")]
        public bool IsEmployee { get; set; }

        [JsonProperty("isFamilyMember")]
        public bool IsFamilyMember { get; set; }

        [JsonProperty("hasPassword")]
        public bool HasPassword { get; set; }

        [JsonProperty("isSchoolScheduler")]
        public bool IsSchoolScheduler { get; set; }

        [JsonProperty("isSchoolLeader")]
        public bool IsSchoolLeader { get; set; }

        [JsonProperty("isStudentAdministrator")]
        public bool IsStudentAdministrator { get; set; }

        [JsonProperty("isTeamLeader")]
        public bool IsTeamLeader { get; set; }

        [JsonProperty("isSectionLeader")]
        public bool IsSectionLeader { get; set; }

        [JsonProperty("isMentor")]
        public bool IsMentor { get; set; }

        [JsonProperty("isParentTeacherNightScheduler")]
        public bool IsParentTeacherNightScheduler { get; set; }

        [JsonProperty("isDean")]
        public bool IsDean { get; set; }
    }

    public class Response
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("details")]
        public string Details { get; set; }

        [JsonProperty("eventId")]
        public int EventId { get; set; }

        [JsonProperty("startRow")]
        public int StartRow { get; set; }

        [JsonProperty("endRow")]
        public int EndRow { get; set; }

        [JsonProperty("totalRows")]
        public int TotalRows { get; set; }

        [JsonProperty("data")]
        public List<Datum> Data { get; set; }
    }

    public class ZermeloUser
    {
        [JsonProperty("response")]
        public Response Response { get; set; }
    }
}
