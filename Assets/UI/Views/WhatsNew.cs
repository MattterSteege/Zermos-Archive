using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UI.Views;
using UnityEngine;
using UnityEngine.UI;

public class WhatsNew : View
{
    [SerializeField] private Button IHaveReadItButton;
    
    public override void Initialize()
    {
        IHaveReadItButton.onClick.AddListener(() =>
        {
            ViewManager.Instance.currentView = this;
            LocalPrefs.SetString("what_new_version", Application.version);
            SwitchView.Instance.Show<DagRoosterView>();
        });
        base.Initialize();
    }
}
