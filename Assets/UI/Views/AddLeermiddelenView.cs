using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Views;
using UnityEngine;
using UnityEngine.UI;

public class AddLeermiddelenView : View
{
    [SerializeField] TMP_InputField url;    
    [SerializeField] TMP_Dropdown vak;
    [SerializeField] Button save;    

    [SerializeField] private CustomLeermiddelen _customLeermiddelen;
    [SerializeField] private Vakken _vakken;
    
    
    //[SerializeField] private Button RefreshButton;

    public override void Initialize()
    {
        openNavigationButton.onClick.AddListener(() =>
        {
            ViewManager.Instance.ShowNewView<LeermiddelenView>();
        });
        
        //save.onClick.AddListener(SaveHomework);

        //foreach (Vakken.Item vak in _vakken.getVakken().items)
        //{
        //    this.vak.AddOptions(new List<string>( new[] { vak.naam }));
        //}
    }

    private void SaveHomework()
    {
        _customLeermiddelen.SaveFile(url.text, vak.options[vak.value].text);
        ViewManager.Instance.Refresh<LeermiddelenView>();
        ViewManager.Instance.Show<LeermiddelenView, NavBarView>();
    }
}
