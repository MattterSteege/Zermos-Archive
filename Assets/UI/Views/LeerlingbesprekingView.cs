using System.Collections;
using System.Collections.Generic;
using UI.Views;
using UnityEngine;

public class LeerlingbesprekingView : View
{
    public override void Initialize()
    {
        openNavigationButton.onClick.AddListener(() =>
        {
            openNavigationButton.enabled = false;
            ViewManager.Instance.ShowNavigation();
        });
        
        closeButtonWholePage.onClick.AddListener(() =>
        {
            openNavigationButton.enabled = true;
            ViewManager.Instance.HideNavigation();
        });
        
        base.Initialize();
    }
}
