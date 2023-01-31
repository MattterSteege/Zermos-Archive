using System.Collections.Generic;
using UnityEngine;

public class Leermiddelen : BetterHttpLibrary
{
    [SerializeField, Tooltip("'*' means Application.persistentDataPath.")]
    public string savePath = "*/Leermiddelen.json";

    public void GetCookies(string username, string password)
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("nextLink", "x");
        parameters.Add("organisatieSearchField--selected-value-1", "c23fbb99-be4b-4c11-bbf5-57e7fc4f4388");
        parameters.Add("organisatieSearchFieldPanel:organisatieSearchFieldPanel_body:organisatieSearchField", "Carmelcollege Gouda");
        
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Origin", "https://inloggen.somtoday.nl/");
        
        string url = "https://inloggen.somtoday.nl/?-1.-panel-organisatieSelectionForm";

        Get(url)
            .AddHeader("Origin", "https://inloggen.somtoday.nl/")
            .AddBody(parameters)
            .AllowCustomSetting()
            .DisallowRedirects()
            .SendRequest()
            .OnSuccess(response =>
            {

            });
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