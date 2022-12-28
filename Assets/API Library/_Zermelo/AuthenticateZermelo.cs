using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public class AuthenticateZermelo : BetterHttpClient
{
    private string username;
    private string password;

    public ZermeloAuthentication AuthenticateUser(string username = "", string password = "") 
    {
        if (username == "" || password == "")
        {
            return null;
        }

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);
        form.AddField("client_id", "OAuthPage");
        form.AddField("redirect_uri", "/main/");
        form.AddField("scope", "");
        form.AddField("state", RandomStateString());
        form.AddField("response_type", "code");
        form.AddField("tenant", "ccg");

        string baseURL = $"https://ccg.zportal.nl/api/v3/oauth";

        return (ZermeloAuthentication) Post(baseURL, form,www =>
        {
            string accessToken = Regex.Matches(www.downloadHandler.text, "[a-zA-Z0-9]{20}")[0].Value;

            
            WWWForm form = new WWWForm();
            form.AddField("code", accessToken);
            form.AddField("client_id", "ZermeloPortal");
            form.AddField("client_secret", "42");
            form.AddField("grant_type", "authorization_code");
            form.AddField("rememberMe", "true");
        
            baseURL = $"https://ccg.zportal.nl/api/v3/oauth/token";
            return (ZermeloAuthentication) Post(baseURL, form, www =>
            {

                ZermeloAuthentication response = JsonConvert.DeserializeObject<ZermeloAuthentication>(www.downloadHandler.text);

                LocalPrefs.SetString("zermelo-access_token", response.access_token);
                LocalPrefs.SetString("zermelo-school_code", "ccg");

                GetComponent<User>().GetUser();

                return response;
            },
            error =>
            {
                Debug.Log(error.error);
                return null;
            });
        }, 
        error =>
        {
            Debug.Log(error.error);
            return null;
        });
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

    public class ZermeloAuthentication
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }
}
