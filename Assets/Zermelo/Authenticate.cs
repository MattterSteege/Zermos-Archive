using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Authenticate : MonoBehaviour
{
    [SerializeField] private string school = "";
    [SerializeField] private string code = "";

    public ZermeloAuthentication startAuthentication(string schoolCode, string authCode)
    {
        StartCoroutine(AuthenticateUser());
        
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
            print("succesvol ingelogd");
            
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

    public class ZermeloAuthentication
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }

}
