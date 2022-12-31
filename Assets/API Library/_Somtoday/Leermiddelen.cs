using System.Collections.Generic;
using System.IO;
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
        Dictionary<string, string> headers = new Dictionary<string,string>();
        headers.Add("cookie", $"{cookie}");
        headers.Add("origin", "https://elo.somtoday.nl");
        Post("https://elo.somtoday.nl/home/leermiddelen", new WWWForm(), headers, (response) =>
        {
            string regex = "<a class=\"header\" href=\"\\.\\/leermiddelen\\?[0-9]-[0-9]\\.-tweeluikPanel-tweeluikDetail-leermiddelenDetail-leermiddelenContainer-eduRouteContainer-eduroute-[0-9]+-leermiddelLink\" hrefinfo=\".*\"><span>.*<\\/span><\\/a>";

            var matches = Regex.Matches(response.downloadHandler.text, regex);
            
            List<string> matchesList = new List<string>();
            foreach (Match match in matches)
                matchesList.Add(match.Value);

            List<Item> leermiddelenList = new List<Item>();
            foreach (string match in matchesList)
            {
                string href = Regex.Match(match, "(?<=hrefinfo=\").*?(?=\">)").Value;
                string description = Regex.Match(match, "(?<=<span>).*?(?=<\\/span>)").Value;
                leermiddelenList.Add(new Item { href = href, description = description });
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

            return null;
        });
    }
    
    public SomtodayLeermiddelen GetLeermiddelen()
    {
        string destination = savePath.Replace("*", Application.persistentDataPath);
        string json = File.ReadAllText(destination);
        return JsonConvert.DeserializeObject<SomtodayLeermiddelen>(json);
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
