using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

                    request = new HttpRequestMessage(HttpMethod.Get, $"https://elo.somtoday.nl/home/leermiddelen");
                    response = client.SendAsync(request).Result;



                    string htmlClass = "m-wrapper";
                    List<string> components = GetHtmlComponents(response.Content.ReadAsStringAsync().Result, htmlClass);
                    List<Item> Items = GetHtmlurls(components);
                    
                    
                    
                    string regex = "<a class=\"header\" href=\"\\.\\/leermiddelen.*<\\/a>";
                    List<Item> leermiddelenList = new List<Item>();

                    for (var i = 0; i < Items.Count; i++)
                    {
                        request = new HttpRequestMessage(HttpMethod.Get, Items[i].href);
                        response = client.SendAsync(request).Result;
                        string html = response.Content.ReadAsStringAsync().Result;

                        var matches = Regex.Matches(html, regex);

                        foreach (Match match in matches)
                        {
                            string href = Regex.Match(match.Value, "(?<=hrefinfo=\").*?(?=\">)").Value;

                            string description = Regex.Match(match.Value, "(?<=<span>).*?(?=<\\/span>)").Value;
                            leermiddelenList.Add(new Item {href = href, description = description.Replace("&amp;", "&"), vak = GetComponent<Vakken>().GetVakNaam(Items[i].vak)});
                        }
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

                    if (leermiddelenList.Count > 0)
                        LocalPrefs.SetBool("leermiddelen_activated", true);
                }
            }
        }
    }
    

    public SomtodayLeermiddelen GetLeermiddelen()
    {
        string destination = savePath.Replace("*", Application.persistentDataPath);
        string json = File.ReadAllText(destination);
        var leermiddelen = JsonConvert.DeserializeObject<SomtodayLeermiddelen>(json);
        return leermiddelen;
    }

    public static List<string> GetHtmlComponents(string htmlPage, string htmlClass)
    {
        List<string> URLS = new List<string>();

        Regex URLRegex = new Regex($"(?<=<div class=\"{htmlClass}\">)((.|\n)*?)(?=<\\/div>)", RegexOptions.Singleline);
        MatchCollection URLMatches = URLRegex.Matches(htmlPage);
            
        foreach (Match match in URLMatches)
            URLS.Add(match.Groups[1].Value);

        return URLS;
    } 
        
    public static List<Item> GetHtmlurls(List<string> components)
    {
        List<Item> items = new List<Item>();

        for (var i = 0; i < components.Count; i++)
        {
            Regex hrefRegex = new Regex("(?<=href=\")((.|\n)*?)(?=\">)", RegexOptions.Singleline);
            items.Add(new Item {href = hrefRegex.Match(components[i]).Groups[1].Value.Replace("-overview=&amp;+", "").Replace("&amp;-display=", "")});
                
            Regex subjectRegex = new Regex($"(?<=<span>)((.|\n)*?)(?=<\\/span>)", RegexOptions.Singleline);
            items[i].vak = subjectRegex.Match(components[i]).Groups[1].Value;
        }

        return items;
    }

    public class Item
    {
        public string href { get; set; }
        public string description { get; set; }
        public string vak { get; set; }
    }

    public class SomtodayLeermiddelen
    {
        public int laatsteWijziging { get; set; }
        public List<Item> items { get; set; }
    }
}