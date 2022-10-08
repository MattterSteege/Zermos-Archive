using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class Homework : MonoBehaviour
{
    [SerializeField] private int maxDaysBack = 14;
    
    [ContextMenu("get homework")]
    public SomtodayHomework getHomework()
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
                $"{PlayerPrefs.GetString("somtoday-api_url")}/rest/v1/studiewijzeritemafspraaktoekenningen?begintNaOfOp={DateTime.Now.ToString("yyyy")}-01-01&additional=swigemaaktVinkjes&additional=huiswerkgemaakt");

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
            .Replace("<li>", "").Replace("</li>", "");
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
                                                                                .Replace("<li>", "").Replace("</li>", ""));
            if (extraHomework != null)
            {
                for (int i = 0; i < extraHomework.items.Count; i++)
                {
                    homework.items.Add(extraHomework.items[i]);
                }
            }
        }

        
        return startSort(homework);
    }
    
    public SomtodayHomework startSort(SomtodayHomework homework)
    {
        return new CoroutineWithData<SomtodayHomework>(this, sort(homework)).result;
    }

    public IEnumerator sort(SomtodayHomework homework)
    {
        homework.items.RemoveAll(x => x.studiewijzerItem == null);
        homework.items.RemoveAll(x=> x.datumTijd < DateTime.Now.AddDays(-maxDaysBack));
        homework.items = homework.items.OrderBy(x => x.datumTijd).ToList();
        yield return homework;
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