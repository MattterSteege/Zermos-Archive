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
        [SerializeField] private Button credentialsConnectButton;
        [SerializeField] private Button somtodayConnectButton;
        [SerializeField] private AuthenticateSomtoday somtodayAuthenticate;
        [SerializeField] private Student student;
        [SerializeField] private ExtendedToggle PassToggle;
        private Schools schools;
        
        bool usingCredentials = false;
        bool usingSomtoday = false;
    
        public override void Initialize()
        {
            credentialsConnectButton.onClick.AddListener(() =>
            {
                credentialsConnectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Inloggen!";
                credentialsConnectButton.interactable = false;
                usingCredentials = true;
                StartCoroutine(OnClickCredentialsConnectButton());
            });
            
            somtodayConnectButton.onClick.AddListener(() =>
            {
                somtodayConnectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Inloggen met SOMtoday!";
                somtodayConnectButton.interactable = false;
                usingSomtoday = true;
                StartCoroutine(OnClickSomtodayConnectButton());
            });
            
            PassToggle.onValueChanged.AddListener((value) =>
            {
                password.contentType = value ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
                password.ForceLabelUpdate();
            });
            
            somtodayAuthenticate.onAuthenticateSomtoday.AddListener(processSuccessOrFailure);

            base.Initialize();
        }

        private IEnumerator OnClickCredentialsConnectButton()
        {
            yield return null;
            somtodayAuthenticate.startAuthentication("c23fbb99-be4b-4c11-bbf5-57e7fc4f4388", username.text, password.text);
        }
        
        private IEnumerator OnClickSomtodayConnectButton()
        {
            yield return null;
            Application.OpenURL(somtodayAuthenticate.InloggenMetSomtodayURL());
        }
        
        private void processSuccessOrFailure(AuthenticateSomtoday.SomtodayAuthentication somtodayAuthentication)
        {
            if (usingCredentials)
            {
                if (somtodayAuthentication?.access_token != null)
                {
                    SuccesScreen.Instance.ShowSuccesScreen(SuccesScreen.LoginType.somtoday);
                    LocalPrefs.SetString("somtoday-access_token", somtodayAuthentication.access_token);
                    LocalPrefs.SetString("somtoday-refresh_token", somtodayAuthentication.refresh_token);
                    LocalPrefs.SetString("somtoday-api_url", somtodayAuthentication.somtoday_api_url);

                    Student.SomtodayStudent user = student.getStudent(true);

                    if (user?.items[0].links[0].id != 0)
                    {
                        LocalPrefs.SetString("somtoday-student_id", user.items[0].links[0].id.ToString());
                    }

                    credentialsConnectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Inloggen!";
                    credentialsConnectButton.interactable = true;
                }
                else
                {
                    credentialsConnectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Inloggen mislukt";
                    credentialsConnectButton.interactable = true;
                }
            }
            if (usingSomtoday)
            {
                if (somtodayAuthentication?.access_token != null)
                {
                    SuccesScreen.Instance.ShowSuccesScreen(SuccesScreen.LoginType.somtoday);
                    LocalPrefs.SetString("somtoday-access_token", somtodayAuthentication.access_token);
                    LocalPrefs.SetString("somtoday-refresh_token", somtodayAuthentication.refresh_token);
                    LocalPrefs.SetString("somtoday-api_url", somtodayAuthentication.somtoday_api_url);

                    Student.SomtodayStudent user = student.getStudent(true);

                    if (user?.items[0].links[0].id != 0)
                    {
                        LocalPrefs.SetString("somtoday-student_id", user.items[0].links[0].id.ToString());
                    }

                    somtodayConnectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Inloggen met SOMtoday!";
                    somtodayConnectButton.interactable = true;
                }
                else
                {
                    somtodayConnectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Inloggen mislukt";
                    somtodayConnectButton.interactable = true;
                }
            }
        }

        public override void Refresh(object args)
        {
            openNavigationButton.onClick.RemoveAllListeners();
            credentialsConnectButton.onClick.RemoveAllListeners();
            somtodayConnectButton.onClick.RemoveAllListeners();
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