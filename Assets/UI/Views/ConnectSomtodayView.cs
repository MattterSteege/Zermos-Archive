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
        [SerializeField] private TMP_Dropdown schoolPicker;
        [SerializeField] private Button connectButton;
        [SerializeField] private AuthenticateSomtoday somtodayAuthenticate;
        [SerializeField] private Student student;
        private Schools schools;
    
        public override void Initialize()
        {
            openNavigationButton.onClick.AddListener(() =>
            {
                ViewManager.Instance.ShowNewView<SettingsView>();
            });
        
            populateSchoolPicker();

            connectButton.onClick.AddListener(() =>
            {
                StartCoroutine(Loading(true));
                
                AuthenticateSomtoday.SomtodayAuthentication response = somtodayAuthenticate.startAuthentication(schools.instellingen[schoolPicker.value].uuid, username.text, password.text);
            
                if (response.access_token != null)
                {
                    somtodayAuthenticate.gameObject.GetComponent<SuccesScreen>().ShowSuccesScreen("Somtoday");
                    PlayerPrefs.SetString("somtoday-access_token", response.access_token);
                    PlayerPrefs.SetString("somtoday-refresh_token", response.refresh_token);
                    PlayerPrefs.SetString("somtoday-api_url", response.somtoday_api_url);
                    PlayerPrefs.Save();
                
                    Student.SomtodayStudent user = student.getStudent(response.access_token);

                    if (user?.items[0].links[0].id != 0)
                    {
                        PlayerPrefs.SetString("somtoday-student_id", user.items[0].links[0].id.ToString());
                        PlayerPrefs.Save();
                    }
                    
                    StartCoroutine(Loading(false));
                }
                else
                {
                    connectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Inloggen mislukt";
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

        public void populateSchoolPicker()
        {
            UnityWebRequest www = UnityWebRequest.Get("https://servers.somtoday.nl/organisaties.json");
            www.SendWebRequest();
        
            while (!www.isDone) { }
        
            schools = JsonConvert.DeserializeObject<List<Schools>>(www.downloadHandler.text)?[0];
            schoolPicker.ClearOptions();
            foreach (Instellingen school in schools.instellingen)
            {
                schoolPicker.options.Add(new TMP_Dropdown.OptionData(school.naam));
            }
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