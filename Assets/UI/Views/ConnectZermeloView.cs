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
        [SerializeField] private ExtendedToggle PassToggle;

        public override void Initialize()
        {
            connectButton.onClick.AddListener(() =>
            {
                connectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Inloggen!";
                connectButton.interactable = false;
                StartCoroutine(OnClickConnectButton());
            });
            
            PassToggle.onValueChanged.AddListener((value) =>
            {
                Wachtwoord.contentType = value ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
                Wachtwoord.ForceLabelUpdate();
            });

            base.Initialize();
        }

        private IEnumerator OnClickConnectButton()
        {
            yield return null;
            string gebruikersnaam = Gebruikersnaam.text;
            string wachtwoord = Wachtwoord.text;

            if (gebruikersnaam != "" && wachtwoord != "")
            {
                AuthenticateZermelo.ZermeloAuthentication response = zermeloAuthenticate.AuthenticateUser(gebruikersnaam, wachtwoord);

                if (response != null && response.access_token != null)
                {
                    zermeloAuthenticate.gameObject.GetComponent<SuccesScreen>().ShowSuccesScreen("Zermelo");
                    yield return new WaitForSeconds(1f);
                    connectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Inloggen!";
                    connectButton.interactable = true;
                }
                else
                {
                    connectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Inloggen mislukt";
                    connectButton.interactable = true;
                }
            }
            else
            {
                connectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Vul alles in!";
                connectButton.interactable = true;
            }
        }

        public override void Refresh(object args)
        {
            connectButton.onClick.RemoveAllListeners();
            base.Refresh(args);
        }
    }
}