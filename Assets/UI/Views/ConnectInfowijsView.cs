using System;
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
    [SerializeField] TMP_Text buttonTextField;
    
    public override void Initialize()
    {
        openNavigationButton.onClick.AddListener(() =>
        {
            ViewManager.Instance.ShowNewView<SettingsView>();
        });
        connectButton.onClick.AddListener(() =>
        {
            StartCoroutine(OnClickConnectButton());
        });

        base.Initialize();
    }

    private IEnumerator OnClickConnectButton()
    {
        yield return null;
        
        var success = authenticateInfowijs.startAuthenticationFase1(emailInputField.text);
        yield return null;
        if (success != null)
        {
            buttonTextField.text = "Check je mail!";
            yield return null;

            bool istweening = false;
            StartCoroutine(FetchToken(true, success.data.id, success.data.customer_product_id, success.data.user_id, success =>
            {
                if (success && !istweening)
                {
                    istweening = true;
                    succesScreen.ShowSuccesScreen("Antonius app");
                    emailInputField.text = "";
                }
                else
                {
                    buttonTextField.text = "Probeer opnieuw";
                }

                return null;
            }));
            
            istweening = false;
        }
        else
        {
            buttonTextField.text = "Probeer opnieuw";
        }
    }
    

    float maxLoadingTime = 30f;
    private IEnumerator FetchToken(bool b, string id = "", string custom_product_id = "", string user_id = "", Func<bool, object> callback = null)
    {
        maxLoadingTime = 30f;
        while (b)
        {
            StartCoroutine(authenticateInfowijs.startAuthenticationCodeFetcher(id, custom_product_id, user_id, success =>
            {
                if (success)
                {
                    StopCoroutine(FetchToken(false));
                    callback(true);
                    buttonTextField.text = "Inloggen!";
                    return true;
                }

                maxLoadingTime -= Time.deltaTime;
                if (maxLoadingTime <= 0)
                {
                    b = false;
                    AndroidUIToast.ShowToast("Sorry je was niet op tijd, probeer het opnieuw");
                    callback(false);
                    return false;

                }
                return false;
            }));

            yield return new WaitForSeconds(1f);
        }
    }
    
    public override void Refresh(object args)
    {
        openNavigationButton.onClick.RemoveAllListeners();
        connectButton.onClick.RemoveAllListeners();
        base.Refresh(args);
    }
}
