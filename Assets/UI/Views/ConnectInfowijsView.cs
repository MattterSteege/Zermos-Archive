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

            if (success != null)
            {
                var success2 = new CoroutineWithData<bool>(this, FetchToken(true, success.data.id, success.data.customer_product_id, success.data.user_id)).result;
                StopCoroutine(FetchToken(false));
                if (success2)
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
            }
            else
            {
                connectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Probeer opnieuw";
            }
        });

        base.Initialize();
    }
    
    private IEnumerator FetchToken(bool b, string id = "", string custom_product_id = "", string user_id = "")
    {
        while (b)
        {
            connectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Klik op de link in je mail";
            bool success = authenticateInfowijs.startAuthenticationCodeFetcher(id, custom_product_id, user_id);
            if (success)
            {
                yield return success;
            }
            yield return null;
        }
    }
    
    public override void Refresh(object args)
    {
        openNavigationButton.onClick.RemoveAllListeners();
        connectButton.onClick.RemoveAllListeners();
        base.Refresh(args);
    }
}
