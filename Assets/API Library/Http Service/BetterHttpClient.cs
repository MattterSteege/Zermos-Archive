using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

public class BetterHttpClient : GlobalHttpVariables
{
    public object Get(string url, Func<UnityWebRequest, object> callback = null, Func<UnityWebRequest, object> error = null)
    {
        IncrementTotalRequests();
        Debug.Log("Request started - " + TotalRequests + " - " + new StackTrace().GetFrame(1).GetMethod().Name);
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("Accept", "application/json");
        www.SendWebRequest();

        while (!www.isDone)
        {
        }

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning(www.error);
            var errored = error?.Invoke(www);
            www.Dispose();
            return errored;
        }

        var returned = callback?.Invoke(www);
        www.Dispose();
        return returned;
    }
    
    public object Get(string url, Dictionary<string, string> headers = null, Func<UnityWebRequest, object> callback = null)
    {
        IncrementTotalRequests();
        Debug.Log("Request started - " + TotalRequests + " - " + new StackTrace().GetFrame(1).GetMethod().Name);
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
            www.Dispose();
            return null;
        }

        var returned = callback.Invoke(www);
        www.Dispose();
        return returned;
    }
    
    public object Get(string url, Dictionary<string, string> headers = null, Func<UnityWebRequest, object> callback = null, Func<UnityWebRequest, object> error = null)
    {
        IncrementTotalRequests();
        Debug.Log("Request started - " + TotalRequests + " - " + new StackTrace().GetFrame(1).GetMethod().Name);
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
            var errored = error?.Invoke(www);
            www.Dispose();
            return errored;
        }

        var returned = callback?.Invoke(www);
        www.Dispose();
        return returned;
    }

    public object Get(string url, WWWForm form, Dictionary<string, string> headers = null, Func<UnityWebRequest, object> callback = null, Func<UnityWebRequest, object> error = null)
    {
        IncrementTotalRequests();
        Debug.Log("Request started - " + TotalRequests + " - " + new StackTrace().GetFrame(1).GetMethod().Name);
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
        IncrementTotalRequests();
        Debug.Log("Request started - " + TotalRequests + " - " + new StackTrace().GetFrame(1).GetMethod().Name);
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

    public object Post(string url, string json, Func<UnityWebRequest, object> callback, Func<UnityWebRequest, object> error = null)
    {
        IncrementTotalRequests();
        Debug.Log("Request started - " + TotalRequests + " - " + new StackTrace().GetFrame(1).GetMethod().Name);
        UnityWebRequest www = UnityWebRequest.Post(url, json);
        www.SetRequestHeader("Content-Type", "application/json");
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

    public object Post(string url, string json, Dictionary<string, string> headers, Func<UnityWebRequest, object> callback, Func<UnityWebRequest, object> error = null)
    {
        IncrementTotalRequests();
        Debug.Log("Request started - " + TotalRequests + " - " + new StackTrace().GetFrame(1).GetMethod().Name);
        UnityWebRequest www = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
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
    
    public object Post(string url, WWWForm form, Dictionary<string, string> headers, Func<UnityWebRequest, object> callback, Func<UnityWebRequest, object> error = null)
    {
        IncrementTotalRequests();
        Debug.Log("Request started - " + TotalRequests + " - " + new StackTrace().GetFrame(1).GetMethod().Name);
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

        while (!www.isDone) { }

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

public class GlobalHttpVariables : MonoBehaviour
{
    public static  int TotalRequests { get; private set; }

    public static  void IncrementTotalRequests()
    {
        TotalRequests++;
    }
}

