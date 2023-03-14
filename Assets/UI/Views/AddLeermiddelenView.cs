using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Views;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AddLeermiddelenView : SubView
{
    [SerializeField] TMP_InputField url;
    [SerializeField] TMP_Dropdown vak;
    [SerializeField] TMP_InputField beschrijving;
    [SerializeField] Button save;
    [SerializeField] GameObject filling;

    [SerializeField] private CustomLeermiddelen _customLeermiddelen;
    [SerializeField] private Vakken _vakken;

    public override void Initialize()
    {
        backButton.onClick.AddListener(() =>
        {
            gameObject.GetComponentInParent<SubViewManager>().ShowParentView();
        });

        save.onClick.AddListener(SaveHomework);

        foreach (Vakken.Item vak in _vakken.getVakken()?.items ?? new List<Vakken.Item>())
        {
            this.vak.AddOptions(new List<string>( new[] { vak.vak.naam }));
        }

        this.vak.captionText.text = "Kies een vak";
    }

    private void SaveHomework()
    {
        if (vak.value == -1)
            return;
        
        
        _customLeermiddelen.AddLeermiddelen(url.text, vak.options[vak.value].text, beschrijving.text);
        ViewManager.Instance.Refresh<LeermiddelenView>();
        ViewManager.Instance.Show<LeermiddelenView, NavBarView>();
    }

    public override void Refresh(object args)
    {
        backButton.onClick.RemoveAllListeners();
        save.onClick.RemoveAllListeners();
        base.Refresh(args);
    }
}
