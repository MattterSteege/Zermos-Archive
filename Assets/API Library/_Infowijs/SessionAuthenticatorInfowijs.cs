using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class SessionAuthenticatorInfowijs : MonoBehaviour
{
    public InfowijsAccessToken GetAccesToken(string access_token = null)
    {
        UnityWebRequest www = UnityWebRequest.PostWwwForm("https://api.infowijs.nl/sessions/access_token", "");
        www.SetRequestHeader("authorization", "Bearer " + access_token);
        www.SetRequestHeader("Accept", "application/vnd.infowijs.v1+json");
        www.SetRequestHeader("x-infowijs-client", $"nl.infowijs.hoy.android/nl.infowijs.client.antonius");
        www.SendWebRequest();

        while (!www.isDone)
        {
        }

        var response = JsonConvert.DeserializeObject<InfowijsAccessToken>(www.downloadHandler.text);
        
        www.Dispose();
        
        if (response.data != null)
        {
            LocalPrefs.SetString("infowijs-session_token", response.data);
            return response;
        }
        
        return null;
        
    }
    
    public class Error
    {
        public int status { get; set; }
        public string title { get; set; }
    }

    public class InfowijsAccessToken
    {
        [CanBeNull] public List<Error> errors { get; set; }
        public string data { get; set; }
    }
}

