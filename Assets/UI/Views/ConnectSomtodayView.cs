using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ConnectSomtodayView : View
{
    [SerializeField] private TMP_InputField username;
    [SerializeField] private TMP_InputField password;
    [SerializeField] private TMP_Dropdown schoolPicker;
    [SerializeField] private Button connectButton;
    [SerializeField] private AuthenticateSomtoday somtodayAuthenticate;
    private Schools schools;
    
    public override void Initialize()
    {
        populateSchoolPicker();

        connectButton.onClick.AddListener(() =>
        {
            AuthenticateSomtoday.SomtodayAuthentication response = somtodayAuthenticate.startAuthentication(schools.instellingen[schoolPicker.value].uuid, username.text, password.text);
            
            if (response.access_token != null)
            {
                somtodayAuthenticate.gameObject.GetComponent<SuccesScreen>().ShowSuccesScreen("Somtoday");
                PlayerPrefs.SetString("somtoday-access_token", response.access_token);
            }
        });

        base.Initialize();
    }
    
    public void populateSchoolPicker()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://servers.somtoday.nl/organisaties.json");
        www.SendWebRequest();
        
        while (!www.isDone) { }
        
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            schools = JsonConvert.DeserializeObject<List<Schools>>(www.downloadHandler.text)?[0];
            schoolPicker.ClearOptions();
            foreach (Instellingen school in schools.instellingen)
            {
                schoolPicker.options.Add(new TMP_Dropdown.OptionData(school.naam));
            }
        }
    }

    #region model
    public class Instellingen
    {
        public string uuid { get; set; }
        public string naam { get; set; }
        public string plaats { get; set; }
        public List<Oidcurl> oidcurls { get; set; }
    }

    public class Oidcurl
    {
        public string omschrijving { get; set; }
        public string url { get; set; }
        public string domain_hint { get; set; }
    }

    public class Schools
    {
        public List<Instellingen> instellingen { get; set; }
    }
    #endregion
}