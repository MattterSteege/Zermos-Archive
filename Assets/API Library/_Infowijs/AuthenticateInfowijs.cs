using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class AuthenticateInfowijs : MonoBehaviour
{
    private InfowijsAuthenticateFase1_1 auth1_1;
    private InfowijsAuthenticateFase1_2 auth1_2;

    public bool startAuthenticationFase1(string username)
    {
        string url = "https://api.infowijs.nl/sessions/customer-products";
        string json = "{\"username\": \"" + username + "\"}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        
        UnityWebRequest www = new UnityWebRequest(url, "POST");
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("accept", "application/vnd.infowijs.v1+json");
        www.SetRequestHeader("Content-Type", "application/json");
        www.SendWebRequest();
        
        while (!www.isDone) { /*wait*/ }
        
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            return false;
        }
        auth1_1 = JsonConvert.DeserializeObject<InfowijsAuthenticateFase1_1>(www.downloadHandler.text);
        
        
            
        string url2 = "https://api.infowijs.nl/sessions";
        string json2 = "{\"customerProductId\": \"" + auth1_1.data[0].id + "\", \"username\": \"" + username + "\"}";
        byte[] bodyRaw2 = System.Text.Encoding.UTF8.GetBytes(json2);
            
        UnityWebRequest www2 = new UnityWebRequest(url2, "POST");
        www2.uploadHandler = new UploadHandlerRaw(bodyRaw2);
        www2.downloadHandler = new DownloadHandlerBuffer();
        www2.SetRequestHeader("accept", "application/vnd.infowijs.v1+json");
        www2.SetRequestHeader("x-infowijs-client", "nl.infowijs.hoy.android/nl.infowijs.client.antonius");
        www2.SetRequestHeader("Content-Type", "text/plain");
        www2.SendWebRequest();
            
        while (!www2.isDone) { /*wait*/ }
            
        if (www2.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www2.error);
            return false;
        }
        
        auth1_2 = JsonConvert.DeserializeObject<InfowijsAuthenticateFase1_2>(www2.downloadHandler.text);
        
        www.Dispose();
        www2.Dispose();
        return true;
    }

    public bool startAuthenticationFase2(string emailLink)
    {
        emailLink = emailLink.Substring(emailLink.IndexOf("url=", StringComparison.Ordinal) + 4).Replace("%3A", ":").Replace("%2F", "/").Replace("%3D", "=").Replace("%3F", "?").Replace("%7C", "|");

        string token = emailLink.Substring(emailLink.IndexOf("token=", StringComparison.Ordinal) + 6);
        token = Regex.Match(token, @"(^[\w-]*\.[\w-]*\.[\w-]*)").Value;

        if (string.IsNullOrEmpty(token)) return false;

        UnityWebRequest www1 = UnityWebRequest.Post("https://api.infowijs.nl/sessions/validate", new WWWForm());
        www1.SetRequestHeader("accept", "application/vnd.infowijs.v1+json");
        www1.SetRequestHeader("authorization", $"Bearer {token}");
        www1.SendWebRequest();

        while (!www1.isDone) { }

        if (www1.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www1.error);
            return false;
        }
        else
        {
            UnityWebRequest www2 = UnityWebRequest.Post($"https://api.infowijs.nl/sessions/{auth1_2.data.id}/{auth1_2.data.customer_product_id}/{auth1_2.data.user_id}", new WWWForm());
            www2.SetRequestHeader("accept", "application/vnd.infowijs.v1+json");
            www2.SetRequestHeader("authorization", $"Bearer {token}");
            www2.SendWebRequest();

            while (!www2.isDone) { }

            if (www2.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www2.error);
                return false;
            }

            InfowijsAuthenticateToken authenticateToken = JsonConvert.DeserializeObject<InfowijsAuthenticateToken>(www2.downloadHandler.text);
            LocalPrefs.SetString("infowijs-access_token", authenticateToken.data);
            return true;
        }
    }
    
    public class Datum
    {
        public string id { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public string domain { get; set; }
        public string logo { get; set; }
        public string expires_at { get; set; }
        public string user_id { get; set; }
        public string customer_product_id { get; set; }
        public string status { get; set; }
        public int community_id { get; set; }
    }
    public class InfowijsAuthenticateFase1_1
    {
        public List<Datum> data { get; set; }
    }
    public class InfowijsAuthenticateFase1_2
    {
        public Datum data { get; set; }
    }
    
    public class InfowijsAuthenticateToken
    {
        public string data { get; set; }
    }
}