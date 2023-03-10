using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine;

public class Leermiddelen : MonoBehaviour
{
    [SerializeField, Tooltip("'*' means Application.persistentDataPath.")]
    public string savePath = "*/Leermiddelen.json";

    public void GetCookies(string username, string password)
    {
        string uuid = "c23fbb99-be4b-4c11-bbf5-57e7fc4f4388";   
        string SchoolName = "Carmelcollege Gouda";   
        
        var handler = new HttpClientHandler();
        handler.AllowAutoRedirect = false;
        handler.CookieContainer = new CookieContainer();
        
        var client = new HttpClient(handler);

        var request = new HttpRequestMessage(HttpMethod.Post, "https://inloggen.somtoday.nl/?-1.-panel-organisatieSelectionForm");
        request.Headers.Add("Origin", "https://inloggen.somtoday.nl");
        request.Content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("nextLink", "x"),
            new KeyValuePair<string, string>("organisatieSearchField--selected-value-1", uuid),
            new KeyValuePair<string, string>("organisatieSearchFieldPanel:organisatieSearchFieldPanel_body:organisatieSearchField", SchoolName)
        });

        var response = client.SendAsync(request).Result;

        var locationHeader = response.Headers.Location;
        if (locationHeader != null)
        {
            var queryString = locationHeader.Query;
            var authValue = ParseQueryString.ParseQuery(queryString).Get("auth");
            if (authValue != null)
            {
                request = new HttpRequestMessage(HttpMethod.Post, $"https://inloggen.somtoday.nl/?-1.-panel-signInForm&auth={authValue}");
                request.Headers.Add("Origin", "https://inloggen.somtoday.nl");
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"loginLink", "x"},
                    {"usernameFieldPanel:usernameFieldPanel_body:usernameField", username}
                });
                response = client.SendAsync(request).Result;

                request = new HttpRequestMessage(HttpMethod.Post, $"https://inloggen.somtoday.nl/login?1-1.-passwordForm&auth={authValue}");
                request.Headers.Add("Origin", "https://inloggen.somtoday.nl");
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"loginLink", "x"},
                    {"passwordFieldPanel:passwordFieldPanel_body:passwordField", password}
                });
                response = client.SendAsync(request).Result;

                //<input type="hidden" name="SAMLResponse" value="([^"]+)"

                string SAMLResponse = Regex.Match(response.Content.ReadAsStringAsync().Result, "<input type=\"hidden\" name=\"SAMLResponse\" value=\"([^\"]+)\"").Groups[1].Value;
                
                
                if (SAMLResponse.Length > 200)
                {
                    request = new HttpRequestMessage(HttpMethod.Post, $"https://elo.somtoday.nl/saml2/acs");
                    
                    //add body: SAMLResponse: SAMLResponse
                    request.Headers.Add("Origin", "https://inloggen.somtoday.nl");
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                    request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        {"SAMLResponse", SAMLResponse},
                    });
                    response = client.SendAsync(request).Result;

                    HttpClientHandler handler2 = new HttpClientHandler();
                    handler2.CookieContainer = handler.CookieContainer;
                    client = new HttpClient(handler2);
                    
                    request = new HttpRequestMessage(HttpMethod.Get, $"https://ssonot.entree.kennisnet.nl/?id=https://somtoday.nl&url=https://inloggen.somtoday.nl&redirectUri=https://elo.somtoday.nl/home/news");
                    response = client.SendAsync(request).Result;

                    request = new HttpRequestMessage(HttpMethod.Get, $"https://elo.somtoday.nl/home/leermiddelen?3");
                    response = client.SendAsync(request).Result;
                    
                    string regex = "<a class=\"header\" href=\"\\.\\/leermiddelen.*<\\/a>";
                        
                    var matches = Regex.Matches(response.Content.ReadAsStringAsync().Result, regex);
                        
                    List<string> matchesList = new List<string>();
                    foreach (Match match in matches)
                        matchesList.Add(match.Value);
                        
                    List<Item> leermiddelenList = new List<Item>();
                    foreach (string match in matchesList)
                    {
                        string href = Regex.Match(match, "(?<=hrefinfo=\").*?(?=\">)").Value;
                        string description = Regex.Match(match, "(?<=<span>).*?(?=<\\/span>)").Value;
                        leermiddelenList.Add(new Item {href = href, description = description});
                    }
                        
                    string convertedJson = JsonConvert.SerializeObject(
                        new SomtodayLeermiddelen()
                        {
                            items = leermiddelenList,
                            laatsteWijziging = TimeManager.Instance.CurrentDateTime.ToUnixTime()
                        },
                        Formatting.Indented);

                    string destination = savePath.Replace("*", Application.persistentDataPath);
                    File.WriteAllText(destination, convertedJson);
                }
            }
        }
    }
    

public SomtodayLeermiddelen GetLeermiddelen()
    {
        //string destination = savePath.Replace("*", Application.persistentDataPath);
        //string json = File.ReadAllText(destination);
        //return JsonConvert.DeserializeObject<SomtodayLeermiddelen>(json);
        return null;
    }

    public class Item
    {
        public string href { get; set; }
        public string description { get; set; }
    }

    public class SomtodayLeermiddelen
    {
        public int laatsteWijziging { get; set; }
        public List<Item> items { get; set; }
    }
}