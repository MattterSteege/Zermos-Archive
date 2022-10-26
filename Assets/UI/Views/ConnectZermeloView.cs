using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectZermeloView : View
{
    [SerializeField] private TMP_InputField Gebruikersnaam;
    [SerializeField] private TMP_InputField wachtwoord;
    [SerializeField] private TMP_InputField schoolCode;
    [SerializeField] private TMP_InputField authCode;
    [SerializeField] private TMP_InputField schoolCode2;
    [SerializeField] private Button connectButton;
    [SerializeField] private AuthenticateZermelo zermeloAuthenticate;

    public override void Initialize()
    {
        connectButton.onClick.AddListener(() =>
        {
            
            
            string gebruikersnaam = this.Gebruikersnaam.text;
            string wachtwoord = this.wachtwoord.text;
            string schoolCode = this.schoolCode.text;
        
            if (schoolCode == "")
                schoolCode = this.schoolCode2.text;
        
            string authCode = this.authCode.text;
            
            
            
            if (authCode != "" && schoolCode != "")
            {
                AuthenticateZermelo.ZermeloAuthentication response = zermeloAuthenticate.startAuthentication(schoolCode, authCode);
            
                if (response.access_token != null)
                {
                    zermeloAuthenticate.gameObject.GetComponent<SuccesScreen>().ShowSuccesScreen("Zermelo");
                }
            }
            
            if (gebruikersnaam != "" && wachtwoord != "" && schoolCode != "")
            {
                AuthenticateZermelo.ZermeloAuthentication response = zermeloAuthenticate.startAuthentication(schoolCode, gebruikersnaam, wachtwoord);
            
                if (response.access_token != null)
                {
                    zermeloAuthenticate.gameObject.GetComponent<SuccesScreen>().ShowSuccesScreen("Zermelo");
                }
            }
        });

        base.Initialize();
    }
}