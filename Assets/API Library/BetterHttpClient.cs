using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

public class BetterHttpClient : MonoBehaviour
{
    public object Get(string url, Func<UnityWebRequest, object> callback = null, Func<UnityWebRequest, object> error = null)
    {
        Debug.Log("Request started");
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
    
    public object Get(string url, Dictionary<string, string> headers = null, Func<UnityWebRequest, object> callback = null, Func<UnityWebRequest, object> error = null)
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

    public object Get(string url, WWWForm form, Dictionary<string, string> headers = null, Func<UnityWebRequest, object> callback = null, Func<UnityWebRequest, object> error = null)
    {
        Debug.Log("Request started");
        UnityWebRequest www = UnityWebRequest.Get(url);
        //www.SetRequestHeader("Accept", "application/json");
        if (headers != null)
        {
            foreach (var header in headers)
            {
                www.SetRequestHeader(header.Key, header.Value);
            }
        }

        www.SendWebRequest();

        while (!www.isDone)
        {
        }

        if (www.result != UnityWebRequest.Result.Success)
        {
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
        Debug.Log("Request started");
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
    
    //with headers
    public IEnumerator Post(string url, WWWForm form, Dictionary<string, string> headers, Func<UnityWebRequest, object> callback, Func<UnityWebRequest, object> error = null)
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
}
