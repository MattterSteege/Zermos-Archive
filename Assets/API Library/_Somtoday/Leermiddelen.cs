using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class Leermiddelen : BetterHttpClient
{
    [SerializeField, Tooltip("'*' means Application.persistentDataPath.")]
    public string savePath = "*/Leermiddelen.json";

    public void SetLeermiddelen(string cookie)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Cookie", cookie);
        string response = client.GetAsync("https://elo.somtoday.nl/home/leermiddelen?-1").Result.Content.ReadAsStringAsync().Result;

        // Dictionary<string, string> headers = new Dictionary<string, string>();
        // headers.Add("cookie", $"{cookie}");
        // headers.Add("origin", "https://elo.somtoday.nl");
        // Get("https://elo.somtoday.nl/home/leermiddelen?-1", new WWWForm(), headers, (response) =>
        // {
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
        //
        //     return null;
        // });
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