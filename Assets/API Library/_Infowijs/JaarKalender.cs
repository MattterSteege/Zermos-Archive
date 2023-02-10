using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UI.Views;
using UnityEngine;
using UnityEngine.Networking;

public class JaarKalender : MonoBehaviour
{
    [SerializeField] SessionAuthenticatorInfowijs authenticator;

    public IEnumerator getEvents()
    {
        yield return new CoroutineWithData<InfowijsKalender>(this, DownloadMessages()).result;
    }
    
    
    public IEnumerator DownloadMessages()
    {
        string baseUrl = $"https://antonius.hoyapp.nl/hoy/v1/events";
        
        string sessionToken = authenticator.GetSessionToken().data;
        if (sessionToken == null)
        {
            AndroidUIToast.ShowToast("Er ging iets mis tijdens het ophalen van een nieuwe sessie token, probeer het (later) opnieuw.");
            yield return null;
        }

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Authorization", $"Bearer {sessionToken}");
        yield return new CoroutineWithData<InfowijsKalender>(this, CustomGet(baseUrl, headers, (response) =>
        {
            InfowijsKalender infowijsMessage = JsonConvert.DeserializeObject<InfowijsKalender>(response.downloadHandler.text);
            return infowijsMessage;
        }, _ =>
        {
            AndroidUIToast.ShowToast("Er ging iets mis tijdens het ophalen van de jaarkalender, probeer het (later) opnieuw.");
            return null;
        })).result;
    }

    private IEnumerator CustomGet(string url, Dictionary<string, string> headers, Func<UnityWebRequest, object> callback, Func<UnityWebRequest, object> error = null)
    {
        Debug.Log("Request started");
        UnityWebRequest www = UnityWebRequest.Get(url);
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

    #region Models
    public class InfowijsKalender

    {
        public List<Datum> data { get; set; }
        public Meta meta { get; set; }
    }
    public class Datum
    {
        public string id { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public int startsAt { get; set; }
        public int endsAt { get; set; }
        public bool isAllDay { get; set; }
        public bool isSubscribed { get; set; }
        public string group { get; set; }
        public int createdAt { get; set; }
        public int updatedAt { get; set; }
    }

    public class Meta
    {
        public int statusCode { get; set; }
        public int timestamp { get; set; }
    }
    #endregion
}