using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BetterHttpClient : MonoBehaviour
{
    public object Get(string url, Func<UnityWebRequest, object> callback, Func<UnityWebRequest, object> error = null)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SendWebRequest();

        while (!www.isDone)
        {
        }

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            return error?.Invoke(www);
        }

        return callback.Invoke(www);
    }

    public object Get(string url, Dictionary<string, string> headers, Func<UnityWebRequest, object> callback, Func<UnityWebRequest, object> error = null)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        foreach (var header in headers)
        {
            www.SetRequestHeader(header.Key, header.Value);
        }

        www.SendWebRequest();

        while (!www.isDone)
        {
        }

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            return error?.Invoke(www);
        }

        return callback.Invoke(www);
    }
    
    public object Post(string url, WWWForm form, Func<UnityWebRequest, object> callback, Func<UnityWebRequest, object> error = null)
    {
        UnityWebRequest www = UnityWebRequest.Post(url, form);
        www.SendWebRequest();

        while (!www.isDone)
        {
        }

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            return error?.Invoke(www);
        }

        return callback.Invoke(www);
    }
}
