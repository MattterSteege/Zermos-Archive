using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class SettingsView : View
    {
        [Header("settings")]
        [SerializeField] private Toggle ShowTussenUren;
        [SerializeField] private DagRoosterView dagRoosterView;
        [SerializeField] private WeekRoosterView weekRoosterView;
    
        [Space]
        [SerializeField] private TMP_InputField NumberOfDaysHomework;
        [SerializeField] private Button NumberOfDaysHomeworkPlus;
        [SerializeField] private Button NumberOfDaysHomeworkMinus;

        [Space]
        [SerializeField] private TMP_InputField minutesBeforeClass;
        [SerializeField] private Button minutesBeforeClassPlus;
        [SerializeField] private Button minutesBeforeClassMinus;


        [Header("Koppelingen")]
        [SerializeField] private Button SomtodayKoppeling;
        [SerializeField] private Image Somtodaygekoppeld;
        [SerializeField] private Student student;
        [SerializeField] private Button ZermeloKoppeling;
        [SerializeField] private Image Zermelogekoppeld;
        [SerializeField] private User user;
        [SerializeField] private Button InfowijsKoppeling;
        [SerializeField] private Image Infowijsgekoppeld;
        [SerializeField] private SessionAuthenticatorInfowijs sessionAuthenticatorInfowijs;
    
        [Header("User info")]
        [SerializeField] private Button userInfo;
    
        [Header("Secret Settings")]
        [SerializeField] private Button SecretSettingsButton;
#if !UNITY_EDITOR
        private int ClicksNeeded = 5;
#endif
    
        [Header("Hulp")]
        [SerializeField] private Button openDocumentatie;

        public override void Initialize()
        {
            openNavigationButton.onClick.AddListener(() =>
            {
                openNavigationButton.enabled = false;
                ViewManager.Instance.ShowNavigation();
            });
        
            closeButtonWholePage.onClick.AddListener(() =>
            {
                openNavigationButton.enabled = true;
                ViewManager.Instance.HideNavigation();
            });
            
#if !UNITY_EDITOR
            int timesCLicked = 0;

            SecretSettingsButton.onClick.AddListener(() =>
            {
            timesCLicked++;
            if (timesCLicked >= ClicksNeeded)
                {
                    timesCLicked = 0;
                    ViewManager.Instance.ShowNewView<SecretSettingsView>();
                }
            });
#else
            SecretSettingsButton.onClick.AddListener(() =>
            {
                ViewManager.Instance.ShowNewView<SecretSettingsView>();
            });
#endif
            ShowTussenUren.isOn = PlayerPrefs.GetInt("ShowTussenUren", 1) == 1;
            ShowTussenUren.onValueChanged.AddListener((bool isOn) =>
            {
                PlayerPrefs.SetInt("ShowTussenUren", isOn ? 1 : 0);

                if (isOn)
                {
                    dagRoosterView.showTussenUren();
                    weekRoosterView.showTussenUren();
                }
                else
                {
                    dagRoosterView.hideTussenUren();
                    weekRoosterView.hideTussenUren();
                }
            });
        
            //--------------------------------------------------------------------------------
            if(PlayerPrefs.GetInt("numberofdayshomework") == 0)
            {
                PlayerPrefs.SetInt("numberofdayshomework", 14);
            }

            NumberOfDaysHomework.text = PlayerPrefs.GetInt("numberofdayshomework").ToString();
            NumberOfDaysHomework.onValueChanged.AddListener((string value) =>
            {
                int number;
                if (int.TryParse(value, out number))
                {
                    PlayerPrefs.SetInt("numberofdayshomework", number);
                }
            });
            NumberOfDaysHomeworkPlus.onClick.AddListener(() =>
            {
                int numberOfDaysHomework = int.Parse(NumberOfDaysHomework.text);
                numberOfDaysHomework++;
                NumberOfDaysHomework.text = numberOfDaysHomework.ToString();
                PlayerPrefs.SetInt("numberofdayshomework", numberOfDaysHomework);
            });
            NumberOfDaysHomeworkMinus.onClick.AddListener(() =>
            {
                int numberOfDaysHomework = int.Parse(NumberOfDaysHomework.text);
                if(numberOfDaysHomework <= 1)
                {
                    return;
                }
                numberOfDaysHomework--;
                NumberOfDaysHomework.text = numberOfDaysHomework.ToString();
                PlayerPrefs.SetInt("numberofdayshomework", numberOfDaysHomework);
            });
        

            //--------------------------------------------------------------------------------
            minutesBeforeClass.text = PlayerPrefs.GetInt("minutesbeforeclass").ToString();
            minutesBeforeClass.onValueChanged.AddListener((string value) =>
            {
                int number;
                if (int.TryParse(value, out number))
                {
                    PlayerPrefs.SetInt("minutesbeforeclass", number);
                }
            });
            minutesBeforeClassPlus.onClick.AddListener(() =>
            {
                int minutes = int.Parse(minutesBeforeClass.text);
                minutes++;
                minutesBeforeClass.text = minutes.ToString();
                PlayerPrefs.SetInt("minutesbeforeclass", minutes);
            });
            minutesBeforeClassMinus.onClick.AddListener(() =>
            {
                int minutes = int.Parse(minutesBeforeClass.text);
                if(minutes <= 1)
                {
                    return;
                }
                minutes--;
                minutesBeforeClass.text = minutes.ToString();
                PlayerPrefs.SetInt("minutesbeforeclass", minutes);
            });
        
            //--------------------------------------------------------------------------------

            SomtodayKoppeling.onClick.AddListener(() =>
            {
                ViewManager.Instance.ShowNewView<ConnectSomtodayView>();
            });
        
            ZermeloKoppeling.onClick.AddListener(() =>
            {
                ViewManager.Instance.ShowNewView<ConnectZermeloView>();
            });
            
            InfowijsKoppeling.onClick.AddListener(() =>
            {
                ViewManager.Instance.ShowNewView<ConnectInfowijsView>();
            });
        
            userInfo.onClick.AddListener(() =>
            {
                ViewManager.Instance.ShowNewView<SavedInformationView>();
            });

            openDocumentatie.onClick.AddListener(() =>
            {
                Application.OpenURL(@"https://mjtsgamer.github.io/Zermos/");
            });

            PlayerPrefs.Save();
        
            base.Initialize();
            
            CheckKoppelingen();
        }

        private void CheckKoppelingen()
        {
            bool SomtodayIslinked = !string.IsNullOrEmpty(student.getStudent(PlayerPrefs.GetString("somtoday-access_token"))?.items[0].UUID ?? "");
            bool ZermeloIslinked = !string.IsNullOrEmpty(user.startGetUser()?.Response?.Data[0]?.Code ?? "");
            bool InfowijsIslinked = !string.IsNullOrEmpty(sessionAuthenticatorInfowijs.GetAccesToken(PlayerPrefs.GetString("infowijs-access_token", ""))?.data ?? "");

            if (SomtodayIslinked)
            {
                Somtodaygekoppeld.color = new Color(0.172549f, 0.9333333f, 0.5568628f);
            }
            else
            {
                Somtodaygekoppeld.color = new Color(0.9921569f, 0.4509804f, 0.4431373f);
            }
            
            if (ZermeloIslinked)
            {
                Zermelogekoppeld.color = new Color(0.172549f, 0.9333333f, 0.5568628f);
            }
            else
            {
                Zermelogekoppeld.color = new Color(0.9921569f, 0.4509804f, 0.4431373f);
            }
            
            if (InfowijsIslinked)
            {
                Infowijsgekoppeld.color = new Color(0.172549f, 0.9333333f, 0.5568628f);
            }
            else
            {
                Infowijsgekoppeld.color = new Color(0.9921569f, 0.4509804f, 0.4431373f);
            }
        }
    }
}