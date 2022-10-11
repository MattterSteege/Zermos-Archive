using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
    }
}