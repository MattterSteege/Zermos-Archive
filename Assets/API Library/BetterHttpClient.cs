using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

public class BetterHttpClient : MonoBehaviour
{
    public object Get(string url, Func<UnityWebRequest, object> callback, Func<UnityWebRequest, object> error = null)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("Accept", "application/json");
        www.SendWebRequest();

        while (!www.isDone)
        {
        }

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning(www.error);
            var errored = error.Invoke(www);
            www.Dispose();
            return errored;
        }

        var returned = callback.Invoke(www);
        www.Dispose();
        return returned;
    }

    public object Get(string url, WWWForm form, Func<UnityWebRequest, object> callback = null, Func<UnityWebRequest, object> error = null)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("Accept", "application/json");
        //set headers from 'headers' object
        foreach (KeyValuePair<string, string> header in form.headers)
        {
            www.SetRequestHeader(header.Key, header.Value);
        }

        www.SendWebRequest();

        while (!www.isDone)
        {
        }

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning(www.error);
            var errored = error.Invoke(www);
            www.Dispose();
            return errored;
        }

        var returned = callback.Invoke(www);
        www.Dispose();
        return returned;
    }
    
    public object Post(string url, WWWForm form, Func<UnityWebRequest, object> callback, Func<UnityWebRequest, object> error = null)
    {
        UnityWebRequest www = UnityWebRequest.Post(url, form);
        www.SetRequestHeader("Accept", "application/json");
        www.SendWebRequest();

        while (!www.isDone)
        {
        }

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning(www.error);
            var errored = error.Invoke(www);
            www.Dispose();
            return errored;
        }

        var returned = callback.Invoke(www);
        www.Dispose();
        return returned;
    }
}
