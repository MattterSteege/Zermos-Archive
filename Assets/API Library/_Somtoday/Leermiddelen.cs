using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine;

public class Leermiddelen : MonoBehaviour
{
    [SerializeField, Tooltip("'*' means Application.persistentDataPath.")]
    public string savePath = "*/Leermiddelen.json";

    public void GetCookies(string username, string password)
    {
        return; 
        HttpClientHandler handler = new HttpClientHandler();
        handler.AllowAutoRedirect = true;
        
        using (HttpClient client = new HttpClient(handler))
        {
            //request 1
            Dictionary<string, string> body = new Dictionary<string, string>();
            body.Add("nextLink", "x");
            body.Add("organisatieSearchField--selected-value-1", "c23fbb99-be4b-4c11-bbf5-57e7fc4f4388");
            body.Add("organisatieSearchFieldPanel:organisatieSearchFieldPanel_body:organisatieSearchField", "Carmelcollege Gouda");
            
            client.BaseAddress = new Uri("https://inloggen.somtoday.nl/?-1.-panel-organisatieSelectionForm");
            var content = new FormUrlEncodedContent(body);
            var response = client.PostAsync("", content).Result;
            
            var location = response.Headers.Location;
            string authCode = ParseQueryString.ParseQuery(location.Query).Get("auth");
            //
            
            //request 2
            body = new Dictionary<string, string>();
            body.Add("loginLink", "x");
            body.Add("usernameFieldPanel:usernameFieldPanel_body:usernameField", username);
            
            client.BaseAddress = new Uri("https://inloggen.somtoday.nl/?-1.-panel-signInForm&auth=" + authCode);
            content = new FormUrlEncodedContent(body);
            response = client.PostAsync("", content).Result;
            //
            
            //request 3
            body = new Dictionary<string, string>();
            body.Add("loginLink", "x");
            body.Add("passwordFieldPanel:passwordFieldPanel_body:passwordField", password);
            
            client.BaseAddress = new Uri("https://inloggen.somtoday.nl/login?1-1.-passwordForm");
            content = new FormUrlEncodedContent(body);
            response = client.PostAsync("", content).Result;
            
            string SAMLResponse = Regex.Match(response.Content.ReadAsStringAsync().Result, "<input *type=\"hidden\" *name=\"SAMLResponse\" *value=\"(.*)\" *\\/>").Groups[1].Value;
            //
            
            //request 4
            body = new Dictionary<string, string>();
            body.Add("SAMLResponse", SAMLResponse);
            
            client.BaseAddress = new Uri("https://elo.somtoday.nl/saml2/acs");
            content = new FormUrlEncodedContent(body);
            
            response = client.PostAsync("", content).Result;
            //
            
            //request 5
            client.BaseAddress = new Uri("https://elo.somtoday.nl/home/leermiddelen");
            response = client.GetAsync("").Result;
            //
            
            //request 6
            client.BaseAddress = new Uri(response.Headers.Location.AbsolutePath);
            response = client.GetAsync("").Result;
            //
            
            //request 7
            client.BaseAddress = new Uri("https://ssonot.aselect.entree.kennisnet.nl/openaselect/profiles/entree?id=https://somtoday.nl&url=https://inloggen.somtoday.nl&redirectUri=https://elo.somtoday.nl/home/leermiddelen");
            response = client.GetAsync("").Result;
            //
            
            //request 8
            client.BaseAddress = new Uri("https://elo.somtoday.nl/home/leermiddelen");
            response = client.GetAsync("").Result;
            //
            
            //request 9
            client.BaseAddress = new Uri(response.Headers.Location.AbsolutePath);
            response = client.GetAsync("").Result;
            //
            
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
            
            var convertedJson = JsonConvert.SerializeObject(
                new SomtodayLeermiddelen
                {
                    items = leermiddelenList,
                    laatsteWijziging = TimeManager.Instance.CurrentDateTime.ToUnixTime()
                },
                Formatting.Indented);
            
            string destination = savePath.Replace("*", Application.persistentDataPath);
            
            File.WriteAllText(destination, "//In dit bestand staan alle leermiddelen.\r\n");
            File.AppendAllText(destination, convertedJson);
        }
    }

    //     string regex = "<a class=\"header\" href=\"\\.\\/leermiddelen.*<\\/a>";
    //
    //     var matches = Regex.Matches(response.downloadHandler.text, regex);
    //
    //     List<string> matchesList = new List<string>();
    //     foreach (Match match in matches)
    //         matchesList.Add(match.Value);
    //
    //     List<Item> leermiddelenList = new List<Item>();
    //     foreach (string match in matchesList)
    //     {
    //         string href = Regex.Match(match, "(?<=hrefinfo=\").*?(?=\">)").Value;
    //         string description = Regex.Match(match, "(?<=<span>).*?(?=<\\/span>)").Value;
    //         leermiddelenList.Add(new Item {href = href, description = description});
    //     }
    //
    //     var convertedJson = JsonConvert.SerializeObject(
    //         new SomtodayLeermiddelen
    //         {
    //             items = leermiddelenList,
    //             laatsteWijziging = TimeManager.Instance.CurrentDateTime.ToUnixTime()
    //         },
    //         Formatting.Indented);
    //
    //     string destination = savePath.Replace("*", Application.persistentDataPath);
    //
    //     File.WriteAllText(destination, "//In dit bestand staan alle leermiddelen.\r\n");
    //     File.AppendAllText(destination, convertedJson);


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