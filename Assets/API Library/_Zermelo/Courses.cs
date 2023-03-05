using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;


public class Courses : BetterHttpClient
{
    [Header("Deze manier is obsolete, gebruik vakken.cs!")]
    [Space(10)]
    [SerializeField, Tooltip("'*' means Application.persistentDataPath.")] private string savePath = "*/Courses.json";
    [HideInInspector] public Schooljaar Schooljaar;
    [SerializeField] public int DagenBeforeRedownload;
    
    public ZermeloVakken getVakken(bool savedIsGood = true)
    {
        string destination = savePath.Replace("*", Application.persistentDataPath);

        if (!File.Exists(destination))
        {
            Debug.LogWarning("File not found, creating new file.");
            return downloadVakken();
        }

        try
        {
            using (StreamReader r = new StreamReader(destination))
            {
                string json = r.ReadToEnd();
                var vakkenObject = JsonConvert.DeserializeObject<ZermeloVakken>(json);
                if (vakkenObject?.laatsteWijziging.ToDateTime().AddDays(DagenBeforeRedownload) < TimeManager.Instance.CurrentDateTime || savedIsGood == false)
                {
                    r.Close();
                    Debug.LogWarning("Local file is outdated, downloading new file.");
                    return downloadVakken();
                }

                return vakkenObject;
            }
        }
        catch (Exception)
        {
            return downloadVakken();
        }
    }
    
    public ZermeloVakken downloadVakken()
    {
        return (ZermeloVakken) Get($"https://{LocalPrefs.GetString("zermelo-school_code")}.zportal.nl/api/v3/courses?schoolYear={Schooljaar.getCurrentSchooljaarStartDate().Year}&student=~me&access_token={LocalPrefs.GetString("zermelo-access_token")}", response =>
        {
            var vakken = JsonConvert.DeserializeObject<ZermeloVakken>(response.downloadHandler.text);

            if (vakken.response.data.Count != 0)
            {
                vakken.response.data = vakken.response.data.OrderBy(x => x.subjectName).ThenBy(x => x.subjectCode).ToList();

                var convertedJson = JsonConvert.SerializeObject(vakken, Formatting.Indented);

                string destination = savePath.Replace("*", Application.persistentDataPath);
                
                File.WriteAllText(destination, convertedJson);

                return vakken;
            }

            return null;
        }, _ => AndroidUIToast.ShowToast("Er ging iets mis met het ophalen van je vakkenpakket."));
    }
    
    #region models
    public class ZermeloVakken
    {
        public int laatsteWijziging;
        public Response response { get; set; }
    }
    
    public class Datum
    {
        public int id { get; set; }
        public string groupName { get; set; }
        public string departmentCode { get; set; }
        public string branchCode { get; set; }
        public int schoolYear { get; set; }
        public string subjectCode { get; set; }
        public string subjectName { get; set; }
        public Teachers teachers { get; set; }
        public List<string> educationTypes { get; set; }
        public object iltCode { get; set; }
        public int yearOfEducation { get; set; }
    }

    public class Participation
    {
        public string userCode { get; set; }
        public string involvement { get; set; }
        public string fullName { get; set; }
        public List<string> relations { get; set; }
    }

    public class Response
    {
        public int status { get; set; }
        public string message { get; set; }
        public string details { get; set; }
        public int eventId { get; set; }
        public int startRow { get; set; }
        public int endRow { get; set; }
        public int totalRows { get; set; }
        public List<Datum> data { get; set; }
    }

    public class Teachers
    {
        public List<Participation> participations { get; set; }
    }
    #endregion
}
