using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class DeepLinkManager : MonoBehaviour
{
    [SerializeField] AuthenticateSomtoday authenticateSomtoday;
    
    public static DeepLinkManager Instance { get; private set; }
    public string deeplinkURL;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;                
            Application.deepLinkActivated += onDeepLinkActivated;
            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                // Cold start and Application.absoluteURL not null so process Deep Link.
                onDeepLinkActivated(Application.absoluteURL);
            }
            // Initialize DeepLink Manager global variable.
            else deeplinkURL = "[none]";
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
 
    private void onDeepLinkActivated(string url)
    {
        //two types of deeplinks are possible:
        //1. somtodayleerling://oauth:443/callback?code=...&iss=...&state=...
        //2. zermos://
        //  2.1. zermos://setting?x=y
        //  2.2. zermos://openview?view=...
        //  2.x etc.
        //  The zermos deeplink is used for setting settings in the app, opening views, etc.
        
        deeplinkURL = url;
        StartCoroutine(process(url));
    }

    private IEnumerator process(string url)
    {
        yield return new WaitForEndOfFrame();
        
        Debug.Log("Deep link activated: " + url);
        
        if (url.StartsWith("somtodayleerling://oauth:443/callback"))
        {
            processSomtoday(url);
        }
        else if (url.StartsWith("zermos://"))
        {
            processZermos(url);
        }
        else if (url.StartsWith("nl.infowijs.client.antonius://"))
        {
            processInfowijs(url);
        }
    }

    private void processSomtoday(string url)
    {
        string code;

        NameValueCollection nameValueCollection = ParseQuery(url.Replace("somtodayleerling://oauth:443/callback", ""));
        code = nameValueCollection["code"];

        Debug.Log(code);

        authenticateSomtoday.InloggenMetSomtodayAuthenticate(code);
    }
    
    private void processZermos(string url)
    {
        Debug.Log(url);
    }
    
    private void processInfowijs(string url)
    {
        //Alleen nodig om de app te openen.
    }
    
    public static NameValueCollection ParseQuery(string query)
    {
        NameValueCollection queryParameters = new NameValueCollection();
        if (!string.IsNullOrEmpty(query))
        {
            if (query.StartsWith("?"))
            {
                query = query.Remove(0, 1);
            }

            foreach (string parameter in query.Split('&'))
            {
                string[] parts = parameter.Split('=');
                string key = parts[0];
                string value = parts.Length > 1 ? parts[1] : "";
                queryParameters.Add(key, value);
            }
        }
        return queryParameters;
    }
}
