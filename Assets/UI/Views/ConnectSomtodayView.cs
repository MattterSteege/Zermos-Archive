using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UI.Views
{
    public class ConnectSomtodayView : View
    {
        [SerializeField] private TMP_InputField username;
        [SerializeField] private TMP_InputField password;
        [SerializeField] private Button connectButton;
        [SerializeField] private AuthenticateSomtoday somtodayAuthenticate;
        [SerializeField] private Student student;
        [SerializeField] private Toggle PassToggle;
        private Schools schools;
    
        public override void Initialize()
        {
            openNavigationButton.onClick.AddListener(() =>
            {
                ViewManager.Instance.ShowNewView<SettingsView>();
            });

            connectButton.onClick.AddListener(() =>
            {
                connectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Inloggen!";
                connectButton.interactable = false;
                StartCoroutine(OnClickConnectButton());
            });
            
            PassToggle.onValueChanged.AddListener((value) =>
            {
                password.contentType = value ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
                password.ForceLabelUpdate();
            });

            base.Initialize();
        }

        private IEnumerator OnClickConnectButton()
        {
            
            yield return null;
            AuthenticateSomtoday.SomtodayAuthentication response = somtodayAuthenticate.startAuthentication("c23fbb99-be4b-4c11-bbf5-57e7fc4f4388", username.text, password.text);
            
            if (response?.access_token != null)
            {
                somtodayAuthenticate.gameObject.GetComponent<SuccesScreen>().ShowSuccesScreen("Somtoday");
                LocalPrefs.SetString("somtoday-access_token", response.access_token);
                LocalPrefs.SetString("somtoday-refresh_token", response.refresh_token);
                LocalPrefs.SetString("somtoday-api_url", response.somtoday_api_url);

                Student.SomtodayStudent user = student.getStudent(true);

                if (user?.items[0].links[0].id != 0)
                {
                    LocalPrefs.SetString("somtoday-student_id", user.items[0].links[0].id.ToString());
                }
                
                connectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Inloggen!";
                connectButton.interactable = true;
            }
            else
            {
                connectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Inloggen mislukt";
                connectButton.interactable = true;
            }
        }

        public override void Refresh(object args)
        {
            openNavigationButton.onClick.RemoveAllListeners();
            connectButton.onClick.RemoveAllListeners();
            base.Refresh(args);
        }

        #region model
        public class Instellingen
        {
            public string uuid { get; set; }
            public string naam { get; set; }
            public string plaats { get; set; }
            public List<Oidcurl> oidcurls { get; set; }
        }

        public class Oidcurl
        {
            public string omschrijving { get; set; }
            public string url { get; set; }
            public string domain_hint { get; set; }
        }

        public class Schools
        {
            public List<Instellingen> instellingen { get; set; }
        }
        #endregion
    }
}