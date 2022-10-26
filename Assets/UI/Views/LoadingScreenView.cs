using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class LoadingScreenView : View
{
    [SerializeField] TMP_Text loadingText;

    [SerializeField] GameObject loadingScreen;
    
    bool done = false;
    
    IEnumerator Start()
    {
        ViewManager.onLoadedView += OnLoadedView;
        ViewManager.onInitializeComplete += Complete;

        yield return null;
    }

    private void OnLoadedView(float loadingcomplete)
    {
        loadingScreen.SetActive(true);
        loadingText.text = "Laden: " + loadingcomplete.ToString("P0");
        
        if (Math.Abs(loadingcomplete - 1) < 0.05f)
        {
            ViewManager.onLoadedView -= OnLoadedView;
            ViewManager.onInitializeComplete -= Complete;
            Complete(true);
        }
    }

    private void Complete(bool done)
    {
        done = true;
        Destroy(loadingScreen);
        
        ViewManager.Instance.Hide<LoadingScreenView>();
    }
}
