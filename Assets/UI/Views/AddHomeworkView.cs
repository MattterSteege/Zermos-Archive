using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddHomeworkView : View
{
    [SerializeField] TMP_InputField titel;    
    [SerializeField] TMP_InputField omschrijving;    
    [SerializeField] TMP_InputField datum;    
    [SerializeField] Button save;    

    [SerializeField] private CustomHomework _CustomHomework;
    
    //[SerializeField] private Button RefreshButton;

    public override void Initialize()
    {
        openNavigationButton.onClick.AddListener(() =>
        {
            ViewManager.Instance.ShowNewView<HomeworkView>();
        });
        
        save.onClick.AddListener(SaveHomework);
    }

    private void SaveHomework()
    {
        _CustomHomework.SaveFile(titel.text, omschrijving.text, new DateTime(Int32.Parse(datum.text.Split("-")[2]), Int32.Parse(datum.text.Split("-")[1]), Int32.Parse(datum.text.Split("-")[0])));
        ViewManager.Instance.Refresh<HomeworkView>();
        ViewManager.Instance.ShowNewView<HomeworkView>();
    }
}
