using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectZermeloView : View
{
    [SerializeField] private TMP_InputField authCode;
    [SerializeField] private TMP_InputField schoolCode;
    [SerializeField] private Button connectButton;
    [SerializeField] private AuthenticateZermelo zermeloAuthenticate;

    public override void Initialize()
    {
        connectButton.onClick.AddListener(() =>
        {
            AuthenticateZermelo.ZermeloAuthentication response = zermeloAuthenticate.startAuthentication(schoolCode.text, authCode.text);
            
            if (response.access_token != null)
            {
                zermeloAuthenticate.gameObject.GetComponent<SuccesScreen>().ShowSuccesScreen("Zermelo");
            }
        });

        base.Initialize();
    }
}