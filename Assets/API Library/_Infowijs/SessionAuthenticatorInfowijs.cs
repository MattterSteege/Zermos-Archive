using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class SessionAuthenticatorInfowijs : BetterHttpClient
{
    public InfowijsAccessToken GetSessionToken()
    {
        string mainAccessToken = LocalPrefs.GetString("infowijs-access_token");

        if (string.IsNullOrEmpty(mainAccessToken))
        {
            AndroidUIToast.ShowToast("Je hebt momenteel geen geldige Infowijs token, herstart de app. Als dit probleem zich blijft voordoen, koppel dan infowijs opnieuw.");
            return null;
        }

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Authorization", "Bearer " + mainAccessToken);
        headers.Add("Accept", "application/vnd.infowijs.v1+json");
        headers.Add("x-infowijs-client", $"nl.infowijs.hoy.android/nl.infowijs.client.antonius");
        return (InfowijsAccessToken) Post("https://api.infowijs.nl/sessions/access_token", null, headers, (response) =>
        {
            InfowijsAccessToken infowijsAccessToken = JsonConvert.DeserializeObject<InfowijsAccessToken>(response.downloadHandler.text);
            
            if (infowijsAccessToken.data != null)
            {
                LocalPrefs.SetString("infowijs-session_token", infowijsAccessToken.data);
                return infowijsAccessToken;
            }
            return null;
        }, _ =>
        {
            if (CheckTokenExpirationDate.CheckToken(mainAccessToken))
                AndroidUIToast.ShowToast("Er is iets fout gegaan bij het ophalen van een nieuwe sessie token, probeer (later) het opnieuw.");
            else
                AndroidUIToast.ShowToast("Je hebt momenteel geen geldige Infowijs token, herstart de app. Als dit probleem zich blijft voordoen, koppel dan infowijs opnieuw.");


            return null;
        });
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