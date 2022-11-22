using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Views;
using UnityEngine;
using UnityEngine.UI;

public class ConnectInfowijsView : View
{
    [SerializeField] private Button connectButton;
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] AuthenticateInfowijs authenticateInfowijs;
    [SerializeField] SuccesScreen succesScreen;
    
    public override void Initialize()
    {
        openNavigationButton.onClick.AddListener(() =>
        {
            ViewManager.Instance.ShowNewView<SettingsView>();
        });
        connectButton.onClick.AddListener(() =>
        {
            var success = authenticateInfowijs.startAuthenticationFase1(emailInputField.text);

            if (success)
            {
                emailInputField.placeholder.GetComponent<TextMeshProUGUI>().text = "email link";
                emailInputField.text = "";
                
                connectButton.onClick.RemoveAllListeners();
                connectButton.onClick.AddListener(() =>
                {
                    var success = authenticateInfowijs.startAuthenticationFase2(emailInputField.text);

                    if (success)
                    {
                        succesScreen.ShowSuccesScreen("Antonius app");
                        ViewManager.Instance.Refresh<SchoolNewsView>();
                        emailInputField.placeholder.GetComponent<TextMeshProUGUI>().text = "school email";
                        emailInputField.text = "";
                    }
                    else
                    {
                        connectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Probeer opnieuw";
                    }
                });
            }
            else
            {
                connectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Probeer opnieuw";
            }
        });

        base.Initialize();
    }
    
    private IEnumerator Loading(bool b)
    {
        while (b)
        {
            connectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Loading...";
            yield return new WaitForSeconds(0.5f);
            connectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Loading.";
            yield return new WaitForSeconds(0.5f);
            connectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Loading..";
            yield return new WaitForSeconds(0.5f);
            connectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Loading...";
            yield return new WaitForSeconds(0.5f);
        }
    }
}
