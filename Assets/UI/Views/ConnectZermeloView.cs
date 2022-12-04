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
        [SerializeField] private TMP_InputField SchoolCode;
        [SerializeField] private Button connectButton;
        [SerializeField] private AuthenticateZermelo zermeloAuthenticate;
        [SerializeField] private User user;

        public override void Initialize()
        {
            connectButton.onClick.AddListener(() =>
            {
                StartCoroutine(Loading(true));

                string gebruikersnaam = Gebruikersnaam.text;
                string wachtwoord = Wachtwoord.text;
                string schoolCode = SchoolCode.text;
                

                if (gebruikersnaam != "" && wachtwoord != "" && schoolCode != "")
                {
                    AuthenticateZermelo.ZermeloAuthentication response =
                        zermeloAuthenticate.startAuthentication(schoolCode, gebruikersnaam, wachtwoord);

                    if (response.access_token != null)
                    {
                        zermeloAuthenticate.gameObject.GetComponent<SuccesScreen>().ShowSuccesScreen("Zermelo");
                        user.startGetUser();
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