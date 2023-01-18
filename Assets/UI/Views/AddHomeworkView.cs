using System;
using TMPro;
using UI.Dates;
using UI.Views;
using UnityEngine;
using UnityEngine.UI;

public class AddHomeworkView : View
{
    [SerializeField] TMP_InputField titel;    
    [SerializeField] TMP_InputField omschrijving;    
    [SerializeField] DatePicker datum;    
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
        _CustomHomework.SaveFile(titel.text, omschrijving.text, datum.SelectedDate.Date);
        ViewManager.Instance.Refresh<HomeworkView>();
        ViewManager.Instance.ShowNewView<HomeworkView>();
    }

    public override void Refresh(object args)
    {
        openNavigationButton.onClick.RemoveAllListeners();
        save.onClick.RemoveAllListeners();
        base.Refresh(args);
    }
}
