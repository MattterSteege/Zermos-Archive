using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddHomeworkView : View
{
    [SerializeField] TMP_InputField titel;    
    [SerializeField] TMP_InputField omschrijving;    
    [SerializeField] Button datum;    
    [SerializeField] Button save;    

    [SerializeField] private CustomHomework _CustomHomework;
    
    //[SerializeField] private Button RefreshButton;

    public override void Initialize()
    {
        save.onClick.AddListener(SaveHomework);
    }

    private void SaveHomework()
    {
        _CustomHomework.SaveFile(titel.text, omschrijving.text, new DateTime());
        ViewManager.Instance.Refresh<HomeworkView>();
        ViewManager.Instance.Show<HomeworkView, NavBarView>();
    }
}
