using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class SecretSettingsView : View
    {
        [SerializeField] TMP_Text output;
        [SerializeField] Button deletePlayerPrefsButton;
        [SerializeField] Button EnableLeermiddelen;
        [SerializeField] Button DisableLeermiddelen;
        [SerializeField] Button ConnectInfowijs;
        [SerializeField] Vakken _vakken;
        
        [Header("Send notif")]
        [SerializeField] private Button SendNotifButton;
        [SerializeField] private LessonNotificationManager lessonNotificationManager;
    
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
        
        
            int timesClicked = 0;
            deletePlayerPrefsButton.onClick.AddListener(() =>
            {
                if (timesClicked < 5)
                {
                    timesClicked++;
                }
                else
                {
                    PlayerPrefs.DeleteAll();
                    PlayerPrefs.Save();
            
                    ViewManager.Instance.ShowNewView<ConnectZermeloView>();

                    timesClicked = 0;
                }
            });
        
            EnableLeermiddelen.onClick.AddListener(() =>
            {
                string settings = PlayerPrefs.GetString("SecretSettings", "1");
                
                settings.ToCharArray()[0] = '1';
                
                PlayerPrefs.SetString("SecretSettings", settings);
                PlayerPrefs.Save();
            
                _vakken.Downloadvakken();
            
                ViewManager.Instance.Refresh<NavBarView>();
            });
        
            DisableLeermiddelen.onClick.AddListener(() =>
            {
                string settings = PlayerPrefs.GetString("SecretSettings", "1");
                
                settings.ToCharArray()[0] = '0';
                
                PlayerPrefs.SetString("SecretSettings", settings);
                PlayerPrefs.Save();
            
                ViewManager.Instance.Refresh<NavBarView>();
            });
            
            SendNotifButton.onClick.AddListener(() =>
            {
                lessonNotificationManager.SendTestNotification();
            });
            
            ConnectInfowijs.onClick.AddListener(() =>
            {
                ViewManager.Instance.ShowNewView<ConnectInfowijsView>();
            });
            
            output.text = "Log:\n";
            Application.logMessageReceived += HandleLog;
        }
    
        // file path regex: ((([A-Za-z]+\/)+)?[A-Z-a-z]+?(.[A-Za-z]+:[0-9]+))

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Log)
            {
                output.text += $"<color=black>{logString}</color>\n\n ";

                string stacktrace = stackTrace;
                var m1 = Regex.Matches(stacktrace, @"((([A-Za-z]+\/)+)?[A-Z-a-z]+?(.[A-Za-z]+:[0-9]+))");

                foreach (Match match in m1)
                {
                    stacktrace = stacktrace.Replace(match.Value, $"<color=blue>{match.Value}</color>");
                }

                output.text += $"{stacktrace}";
                output.text += $"\n-----------------\n";
            }
            else if (type == LogType.Warning)
            {
                output.text += $"<color=yellow>{logString}</color>\n\n ";

                string stacktrace = stackTrace;
                var m1 = Regex.Matches(stacktrace, @"((([A-Za-z]+\/)+)?[A-Z-a-z]+?(.[A-Za-z]+:[0-9]+))");

                foreach (Match match in m1)
                {
                    stacktrace = stacktrace.Replace(match.Value, $"<color=blue>{match.Value}</color>");
                }

                output.text += $"{stacktrace}";
                output.text += $"\n-----------------\n";
            }
            else if (type == LogType.Error)
            {
                output.text += $"<color=red>{logString}</color>\n\n ";

                string stacktrace = stackTrace;
                var m1 = Regex.Matches(stacktrace, @"((([A-Za-z]+\/)+)?[A-Z-a-z]+?(.[A-Za-z]+:[0-9]+))");

                foreach (Match match in m1)
                {
                    stacktrace = stacktrace.Replace(match.Value, $"<color=blue>{match.Value}</color>");
                }

                output.text += $"{stacktrace}";
                output.text += $"\n-----------------\n";
            }
            else if (type == LogType.Exception)
            {
                output.text += $"<color=red>{logString}</color>\n\n ";

                string stacktrace = stackTrace;
                var m1 = Regex.Matches(stacktrace, @"((([A-Za-z]+\/)+)?[A-Z-a-z]+?(.[A-Za-z]+:[0-9]+))");

                foreach (Match match in m1)
                {
                    stacktrace = stacktrace.Replace(match.Value, $"<color=blue>{match.Value}</color>");
                }

                output.text += $"{stacktrace}";
                output.text += $"\n-----------------\n";
            }
            else if (type == LogType.Assert)
            {
                output.text += $"<color=red>{logString}</color>\n\n ";

                string stacktrace = stackTrace;
                var m1 = Regex.Matches(stacktrace, @"((([A-Za-z]+\/)+)?[A-Z-a-z]+?(.[A-Za-z]+:[0-9]+))");

                foreach (Match match in m1)
                {
                    stacktrace = stacktrace.Replace(match.Value, $"<color=blue>{match.Value}</color>");
                }

                output.text += $"{stacktrace}";
                output.text += $"\n-----------------\n";
            }
        }
    }
}
