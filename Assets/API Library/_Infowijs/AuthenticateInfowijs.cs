using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class AuthenticateInfowijs : BetterHttpClient
{
    private InfowijsAuthenticateFase1_1 auth1_1;
    private InfowijsAuthenticateFase1_2 auth1_2;

    public InfowijsAuthenticateFase1_2 startAuthenticationFase1(string username)
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
            return null;
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
            return null;
        }
        
        auth1_2 = JsonConvert.DeserializeObject<InfowijsAuthenticateFase1_2>(www2.downloadHandler.text);

        www.Dispose();
        www2.Dispose();
        return auth1_2;
    }
    
    public IEnumerator startAuthenticationCodeFetcher(string id, string custom_product_id, string user_id, Func<bool, object> callback)
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("accept", "application/vnd.infowijs.v1+json");
        headers.Add("x-infowijs-client", "nl.infowijs.hoy.android/nl.infowijs.client.antonius");
        yield return StartCoroutine(Post($"https://api.infowijs.nl/sessions/{id}/{custom_product_id}/{user_id}", new WWWForm(), headers, (response) =>
        {
            if (int.Parse(response.GetResponseHeader("content-length")) > 500)
            {
                LocalPrefs.SetString("infowijs-access_token", JsonConvert.DeserializeObject<InfowijsAuthenticateToken>(response.downloadHandler.text)?.data);
                callback(true);
                return true;
            }
            callback(false);
            return false;
        }, _ => false));
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