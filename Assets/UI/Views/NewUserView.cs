using System.Collections;
using System.Collections.Generic;
using UI.Views;
using UnityEngine;
using UnityEngine.UI;

public class NewUserView : View
{
    [SerializeField] private Button ToZermelo;

    public override void Show(object args = null)
    {
        base.Show(args);

        ToZermelo.onClick.AddListener(() =>
        {
            LocalPrefs.SetBool("first_time", true);
            ViewManager.Instance.ShowNewView<ConnectZermeloView>();
        });
    }
}
