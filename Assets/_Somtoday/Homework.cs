using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class Homework : MonoBehaviour
{
    [SerializeField] private CustomHomework _CustomHomework;
    
    [ContextMenu("get homework")]
    public List<Item> getHomework()
    {
        if (PlayerPrefs.GetString("somtoday-access_token") == "")
        {
            return null;
        }

        string json = "";
        var homework = new SomtodayHomework();

        int rangemin = 0;
        int rangemax = 99;

        string baseurl =
            string.Format(
                $"{PlayerPrefs.GetString("somtoday-api_url")}/rest/v1/studiewijzeritemafspraaktoekenningen?begintNaOfOp={DateTime.Now.ToString("yyyy")}-01-01&additional=swigemaaktVinkjes&additional=huiswerkgemaakt&additional=leerlingen");

        
        UnityWebRequest www = UnityWebRequest.Get(baseurl);
        www.SetRequestHeader("authorization", "Bearer " + PlayerPrefs.GetString("somtoday-access_token"));
        www.SetRequestHeader("Accept", "application/json");
        www.SetRequestHeader("Range", $"items={rangemin}-{rangemax}");
        www.SendWebRequest();

        while (!www.isDone)
        {
        }

        json = www.downloadHandler.text
            .Replace("<p>", "").Replace("</p>", "")
            .Replace("<ul>", "").Replace("</ul>", "")
            .Replace("<li>", "• ").Replace("</li>", "")
            .Replace("&amp;", "&")
            .Replace("<strong>", "<b>").Replace("</strong>", "</b>")
            .Replace("<em>", "<i>").Replace("</em>", "</i>");
        
        homework = JsonConvert.DeserializeObject<SomtodayHomework>(json);

        var header = www.GetResponseHeader("Content-Range");
        var total = int.Parse(header.Split('/')[1]);

        while (rangemax < total)
        {
            rangemin += 100;
            rangemax += 100;
            
            www = UnityWebRequest.Get(baseurl);
            www.SetRequestHeader("authorization", "Bearer " + PlayerPrefs.GetString("somtoday-access_token"));
            www.SetRequestHeader("Accept", "application/json");
            www.SetRequestHeader("Range", $"items={rangemin}-{rangemax}");
            www.SendWebRequest();

            while (!www.isDone)
            {
            }

            json += www.downloadHandler.text;
            header = www.GetResponseHeader("Content-Range");
            total = int.Parse(header.Split('/')[1]);

            var extraHomework = JsonConvert.DeserializeObject<SomtodayHomework>(www.downloadHandler.text
                .Replace("<p>", "").Replace("</p>", "")
                .Replace("<ul>", "").Replace("</ul>", "")
                .Replace("<li>", "• ").Replace("</li>", "")
                .Replace("&amp;", "&").Replace("<br>", "\n")
                .Replace("<strong>", "<b>").Replace("</strong>", "</b>")
                .Replace("<em>", "<i>").Replace("</em>", "</i>"));
            
            if (extraHomework != null)
            {
                for (int i = 0; i < extraHomework.items.Count; i++)
                {
                    homework.items.Add(extraHomework.items[i]);
                }
            }
        }
        
        var weekHomework = GetWeekHomework();
        
        homework?.items.AddRange(weekHomework);
        homework?.items.AddRange(_CustomHomework.GetCustomHomeWork());
        
        homework = Sort(homework);

        return homework.items;
    }

    [ContextMenu("get week homework")]
    public List<Item> GetWeekHomework()
    {
        if (PlayerPrefs.GetString("somtoday-access_token") == "")
        {
            return null;
        }

        string json = "";
        var homework = new SomtodayHomework();

        int rangemin = 0;
        int rangemax = 99;

        string baseurl =
            string.Format(
                $"{PlayerPrefs.GetString("somtoday-api_url")}/rest/v1/studiewijzeritemweektoekenningen?schooljaar={PlayerPrefs.GetString("somtoday-schooljaar_id")}&begintNaOfOp={DateTime.Now.ToString("yyyy")}-01-01&additional=swigemaaktVinkjes&additional=huiswerkgemaakt&additional=leerlingen");

        
        UnityWebRequest www = UnityWebRequest.Get(baseurl);
        www.SetRequestHeader("authorization", "Bearer " + PlayerPrefs.GetString("somtoday-access_token"));
        www.SetRequestHeader("Accept", "application/json");
        www.SetRequestHeader("Range", $"items={rangemin}-{rangemax}");
        www.SendWebRequest();

        while (!www.isDone)
        {
        }

        json = www.downloadHandler.text
            .Replace("<p>", "").Replace("</p>", "")
            .Replace("<ul>", "").Replace("</ul>", "")
            .Replace("<li>", "").Replace("</li>", "")
            .Replace("&amp;", "&");
        
        homework = JsonConvert.DeserializeObject<SomtodayHomework>(json);

        var header = www.GetResponseHeader("Content-Range");
        var total = int.Parse(header.Split('/')[1]);

        while (rangemax < total)
        {
            rangemin += 100;
            rangemax += 100;
            
            www = UnityWebRequest.Get(baseurl);
            www.SetRequestHeader("authorization", "Bearer " + PlayerPrefs.GetString("somtoday-access_token"));
            www.SetRequestHeader("Accept", "application/json");
            www.SetRequestHeader("Range", $"items={rangemin}-{rangemax}");
            www.SendWebRequest();

            while (!www.isDone)
            {
            }

            json += www.downloadHandler.text;
            header = www.GetResponseHeader("Content-Range");
            total = int.Parse(header.Split('/')[1]);

            var extraHomework = JsonConvert.DeserializeObject<SomtodayHomework>(www.downloadHandler.text
                                                                                .Replace("<p>", "").Replace("</p>", "")
                                                                                .Replace("<ul>", "").Replace("</ul>", "")
                                                                                .Replace("<li>", "").Replace("</li>", "")
                                                                                .Replace("&amp;", "&"));
            if (extraHomework != null)
            {
                for (int i = 0; i < extraHomework.items.Count; i++)
                {
                    homework.items.Add(extraHomework.items[i]);
                }
            }
        }

        foreach (Item homeworkItem in homework.items)
        {
            homeworkItem.datumTijd = getDateFromWeeknumber(homeworkItem.weeknummerVanaf, DateTime.Now.Year);
        }

        homework = Sort(homework);

        return homework.items;
    }


    public bool SetHomeworkStatus(Item huiwerkItem, bool gemaakt)
    {
        string json = ("{\"leerling\": {\"links\": [{\"id\": {id},\"rel\": \"self\",\"href\": \"{apiUrl}/rest/v1/leerlingen/{id}\"}]},\"gemaakt\": {gemaakt}}")
            .Replace("{id}", huiwerkItem.additionalObjects.leerlingen.items[0].links[0].id.ToString())
            .Replace("{apiUrl}", PlayerPrefs.GetString("somtoday-api_url"))
            .Replace("{gemaakt}", gemaakt.ToString().ToLower());
        
        UnityWebRequest www = UnityWebRequest.Put($"{PlayerPrefs.GetString("somtoday-api_url")}/rest/v1/huiswerkgemaakt/{huiwerkItem.additionalObjects.swigemaaktVinkjes.items[0].links[0].id}", json);
        
        www.SetRequestHeader("authorization", "Bearer " + PlayerPrefs.GetString("somtoday-access_token"));
        
        www.SetRequestHeader("Accept", "application/json");
        www.SetRequestHeader("Content-Type", "application/json");
        www.SendWebRequest();
        
        while (!www.isDone)
        {
        }

        if (www.downloadHandler.text.Contains("\"gemaakt\": true"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public SomtodayHomework Sort(SomtodayHomework homework)
    {
        homework.items = homework.items.OrderBy(x => x.datumTijd).ToList();
        homework.items.RemoveAll(x => x.studiewijzerItem == null);
        homework.items.RemoveAll(x=> x.datumTijd < DateTime.Now.AddDays(-PlayerPrefs.GetInt("numberofdayshomework")));
        homework.items.RemoveAll(x=> x.studiewijzerItem.huiswerkType == "LESSTOF");
        return homework;
    }
    
    public DateTime getDateFromWeeknumber(int weeknumber, int year)
    {
        DateTime jan1 = new DateTime(year, 1, 1);
        int daysOffset = DayOfWeek.Monday - jan1.DayOfWeek;
        DateTime firstMonday = jan1.AddDays(daysOffset);
        int firstWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(jan1, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        if (firstWeek <= 1)
        {
            weeknumber -= 1;
        }
        DateTime result = firstMonday.AddDays(weeknumber * 7);
        return result.AddDays(4);
    }

    #region Models
    public class SomtodayHomework
    {
        public List<Item> items { get; set; }
    }
    
    public class AdditionalObjects
    {
        public SwigemaaktVinkjes swigemaaktVinkjes { get; set; }
        public object huiswerkgemaakt { get; set; }
        public Leerlingen leerlingen { get; set; }
    }

    public class Item
    {
        [JsonProperty("$type")]
        public string Type { get; set; }
        public List<Link> links { get; set; }
        public List<Permission> permissions { get; set; }
        public AdditionalObjects additionalObjects { get; set; }
        public StudiewijzerItem studiewijzerItem { get; set; }
        public int sortering { get; set; }
        public Lesgroep lesgroep { get; set; }
        public DateTime datumTijd { get; set; }
        public DateTime aangemaaktOpDatumTijd { get; set; }
        public Leerling leerling { get; set; }
        public object swiToekenningId { get; set; }
        public bool gemaakt { get; set; }
        public int weeknummerVanaf { get; set; }
        public string UUID { get; set; }
        public int leerlingnummer { get; set; }
        public string roepnaam { get; set; }
        public string voorvoegsel { get; set; }
        public string achternaam { get; set; }
    }

    public class Leerling
    {
        public List<Link> links { get; set; }
        public List<Permission> permissions { get; set; }
        public AdditionalObjects additionalObjects { get; set; }
        public string UUID { get; set; }
        public int leerlingnummer { get; set; }
        public string roepnaam { get; set; }
        public string voorvoegsel { get; set; }
        public string achternaam { get; set; }
    }
    
    public class Leerlingen
    {
        [JsonProperty("$type")]
        public string Type { get; set; }
        public List<Item> items { get; set; }
    }

    public class Lesgroep
    {
        public List<Link> links { get; set; }
        public List<Permission> permissions { get; set; }
        public AdditionalObjects additionalObjects { get; set; }
        public string UUID { get; set; }
        public string naam { get; set; }
        public Schooljaar schooljaar { get; set; }
        public Vak vak { get; set; }
        public bool heeftStamgroep { get; set; }
        public bool examendossierOndersteund { get; set; }
        public Vestiging vestiging { get; set; }
    }

    public class Link
    {
        public object id { get; set; }
        public string rel { get; set; }
        public string type { get; set; }
        public string href { get; set; }
    }

    public class Permission
    {
        public string full { get; set; }
        public string type { get; set; }
        public List<string> operations { get; set; }
        public List<string> instances { get; set; }
    }

    public class Schooljaar
    {
        [JsonProperty("$type")]
        public string Type { get; set; }
        public List<Link> links { get; set; }
        public List<Permission> permissions { get; set; }
        public AdditionalObjects additionalObjects { get; set; }
        public string naam { get; set; }
        public string vanafDatum { get; set; }
        public string totDatum { get; set; }
        public bool isHuidig { get; set; }
    }

    public class StudiewijzerItem
    {
        public List<Link> links { get; set; }
        public List<Permission> permissions { get; set; }
        public AdditionalObjects additionalObjects { get; set; }
        public string onderwerp { get; set; }
        public string huiswerkType { get; set; }
        public string omschrijving { get; set; }
        public bool inleverperiodes { get; set; }
        public bool lesmateriaal { get; set; }
        public bool projectgroepen { get; set; }
        public List<object> bijlagen { get; set; }
        public List<object> externeMaterialen { get; set; }
        public List<object> inlevermomenten { get; set; }
        public bool tonen { get; set; }
        public bool notitieZichtbaarVoorLeerling { get; set; }
        public string leerdoelen { get; set; }
    }

    public class SwigemaaktVinkjes
    {
        [JsonProperty("$type")]
        public string Type { get; set; }
        public List<Item> items { get; set; }
    }

    public class Vak
    {
        public List<Link> links { get; set; }
        public List<Permission> permissions { get; set; }
        public AdditionalObjects additionalObjects { get; set; }
        public string afkorting { get; set; }
        public string naam { get; set; }
    }

    public class Vestiging
    {
        public List<Link> links { get; set; }
        public List<Permission> permissions { get; set; }
        public AdditionalObjects additionalObjects { get; set; }
        public string naam { get; set; }
    }

    #endregion
}