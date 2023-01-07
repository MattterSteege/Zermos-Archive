using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RestSharp;
using UnityEngine;

public class Leermiddelen : MonoBehaviour
{
    [SerializeField, Tooltip("'*' means Application.persistentDataPath.")]
    public string savePath = "*/Leermiddelen.json";

    public void GetCookies(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("nextLink", "x");
        form.AddField("organisatieSearchField--selected-value-1", "c23fbb99-be4b-4c11-bbf5-57e7fc4f4388");
        form.AddField("organisatieSearchFieldPanel:organisatieSearchFieldPanel_body:organisatieSearchField", "Carmelcollege Gouda");

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("referer", "https://inloggen.somtoday.nl");

        Post("https://inloggen.somtoday.nl/?-1.-panel-organisatieSelectionForm", form, headers, (response) =>
        {
            Uri myUri = new Uri(response.Headers.ToList().Find(x => x.Name?.ToLower() == "location").Value?.ToString() ?? string.Empty);
            string auth = HttpUtility.ParseQueryString(myUri.Query).Get("auth");
            
            WWWForm form = new WWWForm();
            form.AddField("loginLink", "x");
            form.AddField("usernameFieldPanel:usernameFieldPanel_body:usernameField", username);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("referer", "https://inloggen.somtoday.nl");

            Post("https://inloggen.somtoday.nl/?-1.-panel-signInForm&auth=" + auth, form, headers, (response) =>
            {
                string location = response.Headers.ToList().Find(x => x.Name == "userId").Value?.ToString();
            
            
            
                return null;
            });
            
            return null;
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
    
    //with headers
    public object Post(string url, WWWForm form, Dictionary<string, string> headers, Func<IRestResponse, object> callback, Func<IRestResponse, object> error = null)
    {
        Debug.Log("Request started");
        
        var client = new RestClient(url);
        client.Timeout = -1;
        client.FollowRedirects = false;
        var request = new RestRequest(Method.POST);

        if (headers != null)
            foreach (var header in headers)
                request.AddHeader(header.Key, header.Value);
        
        request.AddParameter("application/x-www-form-urlencoded", form.data, ParameterType.RequestBody);
        
        
        IRestResponse response = client.Execute(request);
        
        if (!response.IsSuccessful)
        {
            Debug.LogWarning(response.ErrorMessage);
            var errored = error.Invoke(response);
            return errored;
        }
        var returned = callback.Invoke(response);
        return returned;
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