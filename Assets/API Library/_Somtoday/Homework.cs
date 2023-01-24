using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class Homework : BetterHttpClient
{
    [SerializeField] private CustomHomework _CustomHomework;
    [SerializeField] private int StartFrom = 0;
    
    #region appointment homework
    [ContextMenu("get homework")]
    public List<Item> getHomework()
    {
        if (string.IsNullOrEmpty(LocalPrefs.GetString("somtoday-access_token"))) return null;

        string json = "";
        var homework = new SomtodayHomework();

        int rangemin = StartFrom + 0;
        int rangemax = StartFrom + 99;

        string startDate = GetComponent<global::Schooljaar>().getCurrentSchooljaarStartDate().ToString("yyyy-MM-dd");
        
        string baseurl = $"{LocalPrefs.GetString("somtoday-api_url")}/rest/v1/studiewijzeritemafspraaktoekenningen?begintNaOfOp={startDate}&additional=swigemaaktVinkjes&additional=huiswerkgemaakt&additional=leerlingen&additional=studiewijzerId";

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("authorization", "Bearer " + LocalPrefs.GetString("somtoday-access_token"));
        headers.Add("Accept", "application/json");
        headers.Add("Range", $"items={rangemin}-{rangemax}");
        return (List<Item>) Get(baseurl, headers, (response) =>
        {
            json = response.downloadHandler.text
                .Replace("<p>", "").Replace("</p>", "")
                .Replace("<ul>", "").Replace("</ul>", "")
                .Replace("<li>", "\n• ").Replace("</li>", "")
                .Replace("&amp;", "&")
                .Replace("<strong>", "<b>").Replace("</strong>", "</b>")
                .Replace("<em>", "<i>").Replace("</em>", "</i>")
                .Replace("&nbsp;", " ").Replace("&gt;", ">");;
            
            homework = JsonConvert.DeserializeObject<SomtodayHomework>(json);

            var header = response.GetResponseHeader("Content-Range");
            var total = int.Parse(header.Split('/')[1]);

            while (rangemax < total)
            {
                rangemin += 100;
                rangemax += 100;

                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("authorization", "Bearer " + LocalPrefs.GetString("somtoday-access_token"));
                headers.Add("Accept", "application/json");
                headers.Add("Range", $"items={rangemin}-{rangemax}");
                
                Get(baseurl, headers, (response) =>
                {
                    json = response.downloadHandler.text
                        .Replace("<p>", "").Replace("</p>", "")
                        .Replace("<ul>", "").Replace("</ul>", "")
                        .Replace("<li>", "\n• ").Replace("</li>", "")
                        .Replace("&amp;", "&")
                        .Replace("<strong>", "<b>").Replace("</strong>", "</b>")
                        .Replace("<em>", "<i>").Replace("</em>", "</i>")
                        .Replace("&nbsp;", " ").Replace("&gt;", ">");;
                    
                    homework = JsonConvert.DeserializeObject<SomtodayHomework>(json);
                    
                    if (homework != null)
                    {
                        for (int i = 0; i < homework.items.Count; i++)
                        {
                            homework.items.AddRange(homework.items);
                        }
                    }

                    return null;
                }, (error) =>
                {
                    AndroidUIToast.ShowToast("Er is iets fout gegaan bij het ophalen van je huiswerk. Probeer het later opnieuw.");
                    return null;
                });
            }

            homework?.items.AddRange(GetWeekHomework());
            homework?.items.AddRange(GetDagHomework());
            homework?.items.AddRange(_CustomHomework.GetCustomHomeWork());

            homework = Sort(homework);

            return homework.items;
        }, (error) =>
        {
            AndroidUIToast.ShowToast("Er is iets fout gegaan bij het ophalen van je huiswerk. Probeer het later opnieuw.");
            return null;
        });
    }
    #endregion

    #region week homework
    [ContextMenu("get week homework")]
    public List<Item> GetWeekHomework()
    {
        if (string.IsNullOrEmpty(LocalPrefs.GetString("somtoday-access_token"))) return null;

        string json = "";
        var homework = new SomtodayHomework();

        int rangemin = StartFrom + 0;
        int rangemax = StartFrom + 99;

        string startDate = GetComponent<global::Schooljaar>().getCurrentSchooljaarStartDate().ToString("yyyy-MM-dd");
        string baseurl = $"{LocalPrefs.GetString("somtoday-api_url")}/rest/v1/studiewijzeritemweektoekenningen?schooljaar=&begintNaOfOp={startDate}&additional=swigemaaktVinkjes&additional=huiswerkgemaakt&additional=leerlingen";

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("authorization", "Bearer " + LocalPrefs.GetString("somtoday-access_token"));
        headers.Add("Accept", "application/json");
        headers.Add("Range", $"items={rangemin}-{rangemax}");
        return (List<Item>) Get(baseurl, headers, (response) =>
        {
            json = response.downloadHandler.text
                .Replace("<p>", "").Replace("</p>", "")
                .Replace("<ul>", "").Replace("</ul>", "")
                .Replace("<li>", "\n• ").Replace("</li>", "")
                .Replace("&amp;", "&")
                .Replace("<strong>", "<b>").Replace("</strong>", "</b>")
                .Replace("<em>", "<i>").Replace("</em>", "</i>")
                .Replace("&nbsp;", " ").Replace("&gt;", ">");;
            
            homework = JsonConvert.DeserializeObject<SomtodayHomework>(json);
            
            var header = response.GetResponseHeader("Content-Range");
            var total = int.Parse(header.Split('/')[1]);
            
            while (rangemax < total)
            {
                rangemin += 100;
                rangemax += 100;

                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("authorization", "Bearer " + LocalPrefs.GetString("somtoday-access_token"));
                headers.Add("Accept", "application/json");
                headers.Add("Range", $"items={rangemin}-{rangemax}");
                
                Get(baseurl, headers, (response) =>
                {
                    json = response.downloadHandler.text
                        .Replace("<p>", "").Replace("</p>", "")
                        .Replace("<ul>", "").Replace("</ul>", "")
                        .Replace("<li>", "\n• ").Replace("</li>", "")
                        .Replace("&amp;", "&")
                        .Replace("<strong>", "<b>").Replace("</strong>", "</b>")
                        .Replace("<em>", "<i>").Replace("</em>", "</i>")
                        .Replace("&nbsp;", " ").Replace("&gt;", ">");;
                    
                    homework = JsonConvert.DeserializeObject<SomtodayHomework>(json);
                    
                    if (homework != null)
                    {
                        for (int i = 0; i < homework.items.Count; i++)
                        {
                            homework.items.AddRange(homework.items);
                        }
                    }

                    return null;

                }, (error) =>
                {
                    AndroidUIToast.ShowToast("Er is iets fout gegaan bij het ophalen van je week huiswerk. Probeer het later opnieuw.");
                    return null;
                });
            }
            
            return homework.items;
        }, (error) =>
        {
            AndroidUIToast.ShowToast("Er is iets fout gegaan bij het ophalen van je week huiswerk. Probeer het later opnieuw.");
            return null;
        });
    }
    #endregion

    #region Dag Huiswerk
    [ContextMenu("get dag homework")]
    public List<Item> GetDagHomework()
    {
        
        if (string.IsNullOrEmpty(LocalPrefs.GetString("somtoday-access_token"))) return null;

        string json = "";
        var homework = new SomtodayHomework();

        int rangemin = StartFrom + 0;
        int rangemax = StartFrom + 99;

        string startDate = GetComponent<global::Schooljaar>().getCurrentSchooljaarStartDate().ToString("yyyy-MM-dd");
        string baseurl = $"{LocalPrefs.GetString("somtoday-api_url")}/rest/v1/studiewijzeritemdagtoekenningen?schooljaar=&begintNaOfOp={startDate}&additional=swigemaaktVinkjes&additional=huiswerkgemaakt&additional=leerlingen";

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("authorization", "Bearer " + LocalPrefs.GetString("somtoday-access_token"));
        headers.Add("Accept", "application/json");
        headers.Add("Range", $"items={rangemin}-{rangemax}");
        return (List<Item>) Get(baseurl, headers, (response) =>
        {
            json = response.downloadHandler.text
                .Replace("<p>", "").Replace("</p>", "")
                .Replace("<ul>", "").Replace("</ul>", "")
                .Replace("<li>", "\n• ").Replace("</li>", "")
                .Replace("&amp;", "&")
                .Replace("<strong>", "<b>").Replace("</strong>", "</b>")
                .Replace("<em>", "<i>").Replace("</em>", "</i>")
                .Replace("&nbsp;", " ").Replace("&gt;", ">");;
            
            homework = JsonConvert.DeserializeObject<SomtodayHomework>(json);
            
            var header = response.GetResponseHeader("Content-Range");
            var total = int.Parse(header.Split('/')[1]);
            
            while (rangemax < total)
            {
                rangemin += 100;
                rangemax += 100;

                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("authorization", "Bearer " + LocalPrefs.GetString("somtoday-access_token"));
                headers.Add("Accept", "application/json");
                headers.Add("Range", $"items={rangemin}-{rangemax}");
                
                Get(baseurl, headers, (response) =>
                {
                    json = response.downloadHandler.text
                        .Replace("<p>", "").Replace("</p>", "")
                        .Replace("<ul>", "").Replace("</ul>", "")
                        .Replace("<li>", "\n• ").Replace("</li>", "")
                        .Replace("&amp;", "&").
                        Replace("<strong>", "<b>").Replace("</strong>", "</b>")
                        .Replace("<em>", "<i>").Replace("</em>", "</i>")
                        .Replace("&nbsp;", " ").Replace("&gt;", ">");;
                    
                    homework = JsonConvert.DeserializeObject<SomtodayHomework>(json);
                    
                    if (homework != null)
                    {
                        for (int i = 0; i < homework.items.Count; i++)
                        {
                            homework.items.AddRange(homework.items);
                        }
                    }
                    
                    return null;
                }, (error) =>
                {
                    AndroidUIToast.ShowToast("Er is iets fout gegaan bij het ophalen van je dag huiswerk. Probeer het later opnieuw.");
                    return null;
                });
            }
            
            return homework.items;
        }, (error) =>
        {
            AndroidUIToast.ShowToast("Er is iets fout gegaan bij het ophalen van je dag huiswerk. Probeer het later opnieuw.");
            return null;
        });
    }
    #endregion

    public SomtodayHomework Sort(SomtodayHomework homework)
    {
        homework.items = homework.items.OrderBy(x => x.datumTijd).ToList();
        homework.items.RemoveAll(x => x.studiewijzerItem == null);
        homework.items.RemoveAll(x=> x.datumTijd < TimeManager.Instance.DateTime.AddDays(-LocalPrefs.GetInt("homework_stays_till", 14)));
        homework.items.RemoveAll(x=> x.studiewijzerItem.huiswerkType == "LESSTOF");
        return homework;
    }

    #region Models
    public class SomtodayHomework
    {
        public List<Item> items { get; set; }
    }
    
    public class AdditionalObjects
    {
        public SwigemaaktVinkjes swigemaaktVinkjes { get; set; }
        public Leerlingen leerlingen { get; set; }
        public object huiswerkgemaakt { get; set; }
        public object studiewijzerId { get; set; }
    }
    
    public class AssemblyResult
    {
        public List<Link> links { get; set; }
        public List<object> permissions { get; set; }
        public AdditionalObjects additionalObjects { get; set; }
        public string assemblyFileType { get; set; }
        public string fileExtension { get; set; }
        public string mimeType { get; set; }
        public float fileSize { get; set; }
        public string fileType { get; set; }
        public string fileUrl { get; set; }
        public string sslUrl { get; set; }
        public string fileName { get; set; }
    }

    public class Bijlagen
    {
        public List<Link> links { get; set; }
        public List<object> permissions { get; set; }
        public AdditionalObjects additionalObjects { get; set; }
        public string omschrijving { get; set; }
        public UploadContext uploadContext { get; set; }
        public List<AssemblyResult> assemblyResults { get; set; }
        public int sortering { get; set; }
        public bool zichtbaarVoorLeerling { get; set; }
    }
    
    public class UploadContext
    {
        public List<Link> links { get; set; }
        public List<object> permissions { get; set; }
        public AdditionalObjects additionalObjects { get; set; }
        public string fileState { get; set; }
        public string assemblyId { get; set; }
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
        public List<Bijlagen> bijlagen { get; set; }
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