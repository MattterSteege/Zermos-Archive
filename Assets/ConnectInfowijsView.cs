using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Views;
using UnityEngine;
using UnityEngine.UI;

public class ConnectInfowijsView : View
{
    [SerializeField] private Button connectButton;
    [SerializeField] private TMP_InputField TokenInputField;
    [SerializeField] AuthenticateInfowijs authenticateInfowijs;
    
    public override void Initialize()
    {
        openNavigationButton.onClick.AddListener(() =>
        {
            ViewManager.Instance.ShowNewView<SettingsView>();
        });
        connectButton.onClick.AddListener(() =>
        {
            var auth = authenticateInfowijs.GetAccesToken(TokenInputField.text);
            
            if (auth.data != null)
            {
                PlayerPrefs.SetString("infowijs-session_token", auth.data);
                PlayerPrefs.SetString("infowijs-access_token", TokenInputField.text);
                PlayerPrefs.Save();
            }
            
            ViewManager.Instance.Refresh<NavBarView>();
            ViewManager.Instance.ShowNavigation();
        });

        base.Initialize();
    }
}
