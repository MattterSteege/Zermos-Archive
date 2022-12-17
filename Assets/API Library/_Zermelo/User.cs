using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class User : BetterHttpClient
{
    public void GetUser() 
    {
        if (string.IsNullOrEmpty(LocalPrefs.GetString("zermelo-access_token")))
            return;
        
        string baseURL = $"https://{LocalPrefs.GetString("zermelo-school_code")}.zportal.nl/api/v3/users/~me?access_token={LocalPrefs.GetString("zermelo-access_token")}";
        Get(baseURL, response =>
        {
            ZermeloUser zermeloUser = JsonConvert.DeserializeObject<ZermeloUser>(response.downloadHandler.text);
            
            LocalPrefs.SetString("zermelo-user_code", zermeloUser.Response.Data[0].Code);
            LocalPrefs.SetString("zermelo-full_name", zermeloUser.Response.Data[0].FirstName + " " + zermeloUser.Response.Data[0].Prefix + " " + zermeloUser.Response.Data[0].LastName);
            return null;
        });
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
