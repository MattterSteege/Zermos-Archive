using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public class AuthenticateSomtoday : MonoBehaviour
{
    [SerializeField, Header("The real auth token")]
    private string AutherizationToken;

    [SerializeField, Space] private string authToken;
    [SerializeField] private string authCode;
    [SerializeField] private string CodeVerifier;
    [SerializeField] private string CodeChallenge;
    private object cookies;

    [ContextMenu("Test Authentication")]
    public void test()
    {
        SomtodayAuthentication auth = startAuthentication("c23fbb99-be4b-4c11-bbf5-57e7fc4f4388", "58373@ccg-leerling.nl", "Somduck");
    }

    public void Start()
    {
        RefreshToken();
    }

    #region authenticate user
    public SomtodayAuthentication startAuthentication(string TENANT_UUID, string username, string password)
    {
        return new CoroutineWithData<SomtodayAuthentication>(this, AuthenticateUser(TENANT_UUID, username, password))
            .result;
    }

    public IEnumerator AuthenticateUser(string TENANT_UUID, string username, string password)
    {
        GenerateTokens();

        var handler = new HttpClientHandler()
        {
            AllowAutoRedirect = false
        };

        var client = new HttpClient(handler, false);

        using (client)
        {
            string baseUrl = string.Format(
                "https://inloggen.somtoday.nl/oauth2/authorize?redirect_uri=somtodayleerling://oauth/callback&client_id=D50E0C06-32D1-4B41-A137-A9A850C892C2&response_type=code&state={0}&scope=openid&tenant_uuid={1}&session=no_session&code_challenge={2}&code_challenge_method=S256",
                generateRandomString(8), "c23fbb99-be4b-4c11-bbf5-57e7fc4f4388", CodeChallenge);

            var response = client.GetAsync(baseUrl).Result;

            if (response.StatusCode != HttpStatusCode.Found) yield return null;

            HttpHeaders headers = response.Headers;
            IEnumerable<string> values;
            if (!headers.TryGetValues("Location", out values)) yield return null;

            Uri myUri = new Uri(values.First());

            authToken = HttpUtility.ParseQueryString(myUri.Query).Get("auth");

            FormUrlEncodedContent Content;
            Content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                {"loginLink", "x"},
                {"usernameFieldPanel:usernameFieldPanel_body:usernameField", username}
            });

            headers.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            client.DefaultRequestHeaders.Add("origin", "https://inloggen.somtoday.nl");
            HttpResponseMessage response2 = client
                .PostAsync($"https://inloggen.somtoday.nl/?-1.-panel-signInForm&auth={authToken}", Content).Result;


            Content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                {"passwordFieldPanel:passwordFieldPanel_body:passwordField", password},
                {"loginLink", "x"}
            });

            HttpResponseMessage response3 = client
                .PostAsync($"https://inloggen.somtoday.nl/login?1-1.-passwordForm&auth={authToken}", Content).Result;


            headers = response3.Headers;
            IEnumerable<string> values3;
            if (!headers.TryGetValues("Location", out values3)) yield return null;

            myUri = new Uri(values3.First());

            authCode = HttpUtility.ParseQueryString(myUri.Query).Get("code");
            
            Content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                {"", ""}
            });
            headers.Clear();
            client.DefaultRequestHeaders.Remove("origin");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            HttpResponseMessage response4 = client.PostAsync(
                $"https://inloggen.somtoday.nl/oauth2/token?grant_type=authorization_code&session=no_session&scope=openid&client_id=D50E0C06-32D1-4B41-A137-A9A850C892C2&tenant_uuid=c23fbb99-be4b-4c11-bbf5-57e7fc4f4388&code={authCode}&code_verifier={CodeVerifier}",
                Content).Result;
            SomtodayAuthentication somtodayAuthentication =
                JsonConvert.DeserializeObject<SomtodayAuthentication>(response4.Content.ReadAsStringAsync().Result);
            
            AutherizationToken = somtodayAuthentication.access_token;
            yield return somtodayAuthentication;
        }
    }
    #endregion

    #region Refresh token
    public SomtodayAuthentication RefreshToken()
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("somtoday-refresh_token"))) return null;

        WWWForm form = new WWWForm();
        form.AddField("grant_type", "refresh_token");
        form.AddField("refresh_token", PlayerPrefs.GetString("somtoday-refresh_token"));
        form.AddField("scope", "openid");
        form.AddField("client_id", "D50E0C06-32D1-4B41-A137-A9A850C892C2");
        UnityWebRequest www = UnityWebRequest.Post($"https://inloggen.somtoday.nl/oauth2/token", form);
         www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
         
         www.SendWebRequest();
         while (!www.isDone) { }
         
         if (www.result == UnityWebRequest.Result.ProtocolError)
         {
             Debug.Log(www.error);
         }
         else
         {
             SomtodayAuthentication somtodayAuthentication = JsonConvert.DeserializeObject<SomtodayAuthentication>(www.downloadHandler.text);
             PlayerPrefs.SetString("somtoday-refresh_token", somtodayAuthentication.refresh_token);
             PlayerPrefs.SetString("somtoday-access_token", somtodayAuthentication.access_token);
             www.Dispose();
             return somtodayAuthentication;
         }

         www.Dispose();
         return null;
    }
    #endregion

    #region Check token
    public bool checkToken()
    {
        UnityWebRequest www = UnityWebRequest.Get($"https://api.somtoday.nl/rest/v1/leerlingen");
        www.SetRequestHeader("Authorization", $"Bearer {PlayerPrefs.GetString("somtoday-access_token")}");
        www.SendWebRequest();
        while (!www.isDone) { }
        if (www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
            if (www.error == "401 Unauthorized")
            {
                RefreshToken();
            }
            
            return false;
        }
        else
        {
            return true;
        }
    }
    #endregion

    #region model
    public class SomtodayAuthentication
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string somtoday_api_url { get; set; }
        public string somtoday_oop_url { get; set; }
        public string scope { get; set; }
        public string somtoday_tenant { get; set; }
        public string id_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }
    #endregion

    #region code Verifier/Challenge/random string
    private static string generateRandomString(int count)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[count];

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[Random.Range(0, chars.Length)];
        }

        return new String(stringChars);
    }

    public void GenerateTokens()
    {
        CodeVerifier = GenerateNonce();
        CodeChallenge = GenerateCodeChallenge(CodeVerifier);
    }

    private static string GenerateNonce()
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz123456789";
        var nonce = new char[128];
        for (int i = 0; i < nonce.Length; i++)
        {
            nonce[i] = chars[Random.Range(0, chars.Length)];
        }

        return new string(nonce);
    }

    private static string GenerateCodeChallenge(string codeVerifier)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
        var b64Hash = Convert.ToBase64String(hash);
        var code = Regex.Replace(b64Hash, "\\+", "-");
        code = Regex.Replace(code, "\\/", "_");
        code = Regex.Replace(code, "=+$", "");
        return code;
    }

    #endregion
}