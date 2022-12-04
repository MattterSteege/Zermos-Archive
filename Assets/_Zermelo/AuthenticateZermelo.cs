using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AuthenticateZermelo : MonoBehaviour
{
    #region auth code based
    string school = "";
    string code = "";

    public ZermeloAuthentication startAuthentication(string schoolCode, string authCode)
    {
        return new CoroutineWithData<ZermeloAuthentication>(this, AuthenticateUser(schoolCode, authCode)).result;
    }

    public IEnumerator AuthenticateUser(string schoolCode = "", string authCode = "") 
    {
        if (schoolCode == "" && authCode == "" && Regex.IsMatch(authCode.Replace(" ", ""), "/[0-9]{12}/"))
        {
            yield break;
        }

        school = schoolCode;
        code = authCode.Replace(" ", "");

        string baseURL = "https://{school}.zportal.nl/api/v3/oauth/token?grant_type=authorization_code&code={code}";
        
        baseURL = baseURL.Replace("{school}", school);
        baseURL = baseURL.Replace("{code}", code);

        UnityWebRequest www = HttpRequest(baseURL);

        if(www.result != UnityWebRequest.Result.Success) 
        {
            Debug.Log(www.error);
        }
        else 
        {
            ZermeloAuthentication response = JsonConvert.DeserializeObject<ZermeloAuthentication>(www.downloadHandler.text);

            PlayerPrefs.SetString("zermelo-access_token", response.access_token);
            PlayerPrefs.SetString("zermelo-school_code", schoolCode);
            PlayerPrefs.Save();
            
            yield return response;
        }
    }
    
    private UnityWebRequest HttpRequest(string baseURL)
    {
        UnityWebRequest www = UnityWebRequest.Post(baseURL, "");
        www.SendWebRequest();
        while (!www.isDone)
        {
        }

        return www;
    }

    #endregion

    #region credentials based
    private string username;
    private string password;

    public ZermeloAuthentication startAuthentication(string schoolCode, string username, string password)
    {
        return new CoroutineWithData<ZermeloAuthentication>(this, AuthenticateUser(schoolCode, username, password)).result;
    }
    
    public IEnumerator AuthenticateUser(string schoolCode = "", string username = "", string password = "") 
    {
        if (schoolCode == "" || username == "" || password == "")
        {
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);
        form.AddField("client_id", "OAuthPage");
        form.AddField("redirect_uri", "/main/");
        form.AddField("scope", "");
        form.AddField("state", RandomStateString());
        form.AddField("response_type", "code");
        form.AddField("tenant", schoolCode);
        
        string baseURL = $"https://{schoolCode}.zportal.nl/api/v3/oauth";
        baseURL = baseURL.Replace("{school}", school);
        UnityWebRequest www = UnityWebRequest.Post(baseURL, form);
        
        www.SendWebRequest();

        while (!www.isDone) { }
        
        if(www.result != UnityWebRequest.Result.Success) 
        {
            Debug.Log(www.error);
        }
        else 
        {
            
            string accessToken = Regex.Matches(www.downloadHandler.text, "[a-zA-Z0-9]{20}")[0].Value;

            
            WWWForm form2 = new WWWForm();
            form.AddField("code", accessToken);
            form.AddField("client_id", "ZermeloPortal");
            form.AddField("client_secret", "42");
            form.AddField("grant_type", "authorization_code");
            form.AddField("rememberMe", "true");
        
            baseURL = $"https://{schoolCode}.zportal.nl/api/v3/oauth/token";
            baseURL = baseURL.Replace("{school}", school);
            UnityWebRequest www2 = UnityWebRequest.Post(baseURL, form);
        
            www2.SendWebRequest();

            while (!www2.isDone) { }
        
            if(www2.result != UnityWebRequest.Result.Success) 
            {
                Debug.Log(www2.error);
            }
            else
            {
                ZermeloAuthentication response = JsonConvert.DeserializeObject<ZermeloAuthentication>(www2.downloadHandler.text);

                PlayerPrefs.SetString("zermelo-access_token", response.access_token);
                PlayerPrefs.SetString("zermelo-school_code", schoolCode);
                PlayerPrefs.Save();
            
                yield return response;
            }
            
            www2.Dispose();
        }
        
        www.Dispose();
    }

    private string RandomStateString()
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string result = "";
        for (int i = 0; i < 6; i++)
        {
            result += chars[Random.Range(0, chars.Length)];
        }

        return result;
    }
    #endregion
    
    public class ZermeloAuthentication
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }
}
