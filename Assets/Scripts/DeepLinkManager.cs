using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using UnityEngine;

public class DeepLinkManager : MonoBehaviour
{
    [SerializeField] AuthenticateSomtoday authenticateSomtoday;
    [SerializeField] AuthenticateInfowijs AuthenticateInfowijs;
    
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
        //three types of deeplinks are possible:
        //1. somtodayleerling://oauth:443/callback?code=...&iss=...&state=...
        //2. nl.infowijs.client.antonius://authenticate/[code]/antonius.hoyapp.nl
        //3. zermos://
        //  3.1. zermos://setting?x=y
        //  3.2. zermos://openview?view=...
        //  3.x etc.
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

        authenticateSomtoday.InloggenMetSomtodayAuthenticate(code);
    }
    
    private void processZermos(string url)
    {
        Debug.Log(url);
    }
    
    private void processInfowijs(string url)
    {
        string code;
        
        code = url.Replace("nl.infowijs.client.antonius://authenticate/", "").Replace("/antonius.hoyapp.nl", "");
        
        //base64 decode
        code = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(code));
        
        //use this regex: /"sessionRequest":\s*{\s*"id":\s*"([^"]+)",\s*"expires_at":\s*"([^"]+)",\s*"user_id":\s*"([^"]+)",\s*"customer_product_id":\s*"([^"]+)",\s*"status":\s*"([^"]+)"\s*}/gm
        //and wrap the string in { }
        
        Regex regex = new Regex(@"""sessionRequest"":\s*{\s*""id"":\s*""([^""]+)"",\s*""expires_at"":\s*""([^""]+)"",\s*""user_id"":\s*""([^""]+)"",\s*""customer_product_id"":\s*""([^""]+)"",\s*""status"":\s*""([^""]+)""\s*}", RegexOptions.IgnoreCase);
        Match match = regex.Match(code);
        
        string JSON = "{" + match.Value + "}";
        
        InfowijsSessionRequest infowijsSessionRequest = JsonUtility.FromJson<InfowijsSessionRequest>(JSON);
        
        StartCoroutine(AuthenticateInfowijs.startAuthenticationCodeFetcher(infowijsSessionRequest.sessionRequest.id, infowijsSessionRequest.sessionRequest.customer_product_id, infowijsSessionRequest.sessionRequest.user_id,
            (success) => ViewManager.Instance.GetInstance<ConnectInfowijsView>().onReturnFetchedToken(success)));
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
    
    public class InfowijsSessionRequest
    {
        public SessionRequest sessionRequest { get; set; }
    }

    public class SessionRequest
    {
        public string id { get; set; }
        public string expires_at { get; set; }
        public string user_id { get; set; }
        public string customer_product_id { get; set; }
        public string status { get; set; }
    }
}
