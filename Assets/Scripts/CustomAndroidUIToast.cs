using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CustomAndroidUIToast : MonoBehaviour
{
    [SerializeField] TMP_Text toastText;
    [SerializeField] CanvasGroup canvasGroup;

    public static CustomAndroidUIToast Instance { get; private set; }
    
    void Start()
    {
        canvasGroup.alpha = 0;
        Instance = this;
    }
    
    public void ShowToast(string message)
    {
        canvasGroup.transform.parent.SetAsLastSibling();
        toastText.text = message;
        canvasGroup.DOFade(1, 0.5f).SetDelay(0.5f).onComplete += () =>
        {
            canvasGroup.DOFade(0, 0.5f).SetDelay(2f);
        };
    }
}

public static class AndroidUIToast
{
    public static void ShowToast(string message)
    {
        CustomAndroidUIToast.Instance.ShowToast(message);
    }
}
