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

        string baseUrl = "https://api.infowijs.nl/sessions/customer-products";
        string json = "{\"username\": \"" + username + "\"}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("accept", "application/vnd.infowijs.v1+json");
        headers.Add("content-type", "application/json");
        return (InfowijsAuthenticateFase1_2) Post(baseUrl, json, headers, (response) =>
        {
            auth1_1 = JsonConvert.DeserializeObject<InfowijsAuthenticateFase1_1>(response.downloadHandler.text);

            string baseUrl = "https://api.infowijs.nl/sessions";
            string json = "{\"customerProductId\": \"" + auth1_1.data[0].id + "\", \"username\": \"" + username + "\"}";
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("accept", "application/vnd.infowijs.v1+json");
            headers.Add("x-infowijs-client", "nl.infowijs.hoy.android/nl.infowijs.client.antonius");
            headers.Add("content-type", "application/json");
            
            return (InfowijsAuthenticateFase1_2) Post(baseUrl, json, headers, (response) =>
            {
                auth1_2 = JsonConvert.DeserializeObject<InfowijsAuthenticateFase1_2>(response.downloadHandler.text);

                return auth1_2;
            }, (error) =>
            {
                AndroidUIToast.ShowToast("Er is iets misgegaan tijdens stap 2 van het inloggen. Probeer het later opnieuw.");
                return null;
            });
        }, (error) =>
        {
            AndroidUIToast.ShowToast("Er is iets misgegaan tijdens stap 1 van het inloggen. Probeer het later opnieuw.");
            return null;
        });
    }
    
    public IEnumerator startAuthenticationCodeFetcher(string id, string custom_product_id, string user_id, Func<bool, object> callback)
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("accept", "application/vnd.infowijs.v1+json");
        headers.Add("x-infowijs-client", "nl.infowijs.hoy.android/nl.infowijs.client.antonius");
        yield return StartCoroutine(CustomPost($"https://api.infowijs.nl/sessions/{id}/{custom_product_id}/{user_id}", new WWWForm(), headers, (response) =>
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
    
    public IEnumerator CustomPost(string url, WWWForm form, Dictionary<string, string> headers, Func<UnityWebRequest, object> callback, Func<UnityWebRequest, object> error = null)
    {
        Debug.Log("Request started");
        UnityWebRequest www = UnityWebRequest.Post(url, form);
        www.SetRequestHeader("Accept", "application/json");
        if (headers != null)
        {
            foreach (var header in headers)
            {
                www.SetRequestHeader(header.Key, header.Value);
            }
        }
        www.SendWebRequest();

        while (!www.isDone)
            yield return null;

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning(www.error);
            var errored = error.Invoke(www);
            www.Dispose();
            yield return errored;
        }

        var returned = callback.Invoke(www);
        www.Dispose();
        yield return returned;
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