using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Views;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NewUserView : View
{
    [SerializeField] private Button ToZermelo;
    [SerializeField] private CanvasGroup cg;

    public override void Show(object args = null)
    {
        base.Show(args);

        if (LocalPrefs.GetString("zermelo-access_token", "") == "")
        {
            ToZermelo.GetComponentInChildren<TMP_Text>().text = "Ik wil inloggen!";
            ToZermelo.onClick.AddListener(() =>
            {
                LocalPrefs.SetBool("first_time", true);
                cg.blocksRaycasts = false;
                cg.interactable = false;
                cg.DOFade(1f, 1f).onComplete += () => { ViewManager.Instance.Hide<NewUserView>(); };
                ViewManager.Instance.ShowNewView<ConnectZermeloView>();
            });
        }
        else
        {
            ToZermelo.GetComponentInChildren<TMP_Text>().text = "Ik heb het gelezen...";
            ToZermelo.onClick.AddListener(() =>
            {
                LocalPrefs.SetBool("first_time", true);
                cg.blocksRaycasts = false;
                cg.interactable = false;
                cg.DOFade(0f, 0.5f).onComplete += () => { ViewManager.Instance.Hide<NewUserView>(); };
            });
        }
    }
}
