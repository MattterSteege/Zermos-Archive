using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HomeworkItemView : View
{
    private Homework.Item homeworkInfo;

    [SerializeField] TMP_Text Vak;
    [SerializeField] TMP_Text Datum;
    [SerializeField] TMP_Text Omschrijving;
    [SerializeField] Toggle Gemaakt;

    [Space, SerializeField] private GameObject HuiswerkPill;
    [SerializeField] private GameObject ToetsPill;
    [SerializeField] private GameObject GroteToetsPill;

    public override void Show(object args = null)
    {
        this.homeworkInfo = (Homework.Item) args;

        Initialize();

        base.Show();
    }

    public override void Initialize()
    {
        if (homeworkInfo == null) return;

        Vak.text = homeworkInfo.lesgroep.vak.naam ?? "";

        Datum.text = homeworkInfo.datumTijd.ToString("d MMMM");

        Omschrijving.text = homeworkInfo.studiewijzerItem.omschrijving;
        if (Omschrijving.text.Length == 0)
            Omschrijving.text = homeworkInfo.studiewijzerItem.onderwerp;
        
        Gemaakt.isOn = homeworkInfo.additionalObjects.swigemaaktVinkjes?.items?[0].gemaakt ?? false;

        if (homeworkInfo.studiewijzerItem.huiswerkType == "HUISWERK")
        {
            HuiswerkPill.SetActive(true); //
            ToetsPill.SetActive(false);
            GroteToetsPill.SetActive(false);
        }
        else if (homeworkInfo.studiewijzerItem.huiswerkType == "TOETS")
        {
            HuiswerkPill.SetActive(false);
            ToetsPill.SetActive(true); //
            GroteToetsPill.SetActive(false);
        }
        else if (homeworkInfo.studiewijzerItem.huiswerkType == "GROTE_TOETS")
        {
            HuiswerkPill.SetActive(false);
            ToetsPill.SetActive(false);
            GroteToetsPill.SetActive(true); //
        }
        
        //Gemaakt.isOn = homeworkInfo.additionalObjects.swigemaaktVinkjes?.items?[0].gemaakt ?? false;
        
        //Gemaakt.onValueChanged.AddListener((bool isOn) =>
        //{
        //    bool succesfull = UpdateGemaaktStatus(homeworkInfo, isOn);
        //       
        //    Gemaakt.SetIsOnWithoutNotify(succesfull);
        //});
    }
    
    /*
    private bool UpdateGemaaktStatus(Homework.Item HomeworkItem, bool gemaakt)
    {
        string json = ("{\"leerling\": {\"links\": [{\"id\": {id},\"rel\": \"self\",\"href\": \"{apiUrl}/rest/v1/leerlingen/{id}\"}]},\"gemaakt\": {gemaakt}}")
            .Replace("{id}", HomeworkItem.additionalObjects.leerlingen.items[0].links[0].id.ToString())
            .Replace("{apiUrl}", PlayerPrefs.GetString("somtoday-api_url"))
            .Replace("{gemaakt}", gemaakt.ToString().ToLower());
        
        UnityWebRequest www = UnityWebRequest.Put($"{PlayerPrefs.GetString("somtoday-api_url")}/rest/v1/swigemaakt/{HomeworkItem.additionalObjects.swigemaaktVinkjes.items[0].links[0].id}", json);
        
        www.SetRequestHeader("authorization", "Bearer " + PlayerPrefs.GetString("somtoday-access_token"));
        
        www.SetRequestHeader("Accept", "application/json");
        www.SetRequestHeader("Content-Type", "application/json");
        www.SendWebRequest();
        
        while (!www.isDone)
        {
        }
        
        if (www.result == UnityWebRequest.Result.Success)
        {
            if (www.downloadHandler.text.Contains("\"gemaakt\":true"))
            {
                www.Dispose();
                return true;
            }
            
            www.Dispose();
            return false;
        }

        www.Dispose();
        return false;
    }
    */
}