using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class ConnectZermeloView : View
    {
        [SerializeField] private TMP_InputField Gebruikersnaam;
        [SerializeField] private TMP_InputField Wachtwoord;
        [SerializeField] private Button connectButton;
        [SerializeField] private AuthenticateZermelo zermeloAuthenticate;

        public override void Initialize()
        {
            connectButton.onClick.AddListener(() =>
            {
                StartCoroutine(Loading(true));

                string gebruikersnaam = Gebruikersnaam.text;
                string wachtwoord = Wachtwoord.text;


                if (gebruikersnaam != "" && wachtwoord != "")
                {
                    AuthenticateZermelo.ZermeloAuthentication response = zermeloAuthenticate.AuthenticateUser(gebruikersnaam, wachtwoord);

                    if (response.access_token != null)
                    {
                        zermeloAuthenticate.gameObject.GetComponent<SuccesScreen>().ShowSuccesScreen("Zermelo");
                    }
                    else
                    {
                        StartCoroutine(Loading(false));
                        connectButton.GetComponentInChildren<TextMeshProUGUI>().text = "inloggen mislukt";
                    }
                }
                else
                {
                    StartCoroutine(Loading(false));
                    connectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Vul alle velden in";
                }
                
                StartCoroutine(Loading(false));
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
}