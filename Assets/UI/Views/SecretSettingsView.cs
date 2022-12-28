using System;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

namespace UI.Views
{
    public class SecretSettingsView : View
    {
        [SerializeField] TMP_Text output;
        [SerializeField] Button deletePlayerPrefsButton;
        [SerializeField] Toggle ToggleLeermiddelen;
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
                if (timesClicked <= 5)
                {
                    timesClicked++;
                }
                else
                {
                    LocalPrefs.DeleteFile();
                    LocalPrefs.Load();

                    ViewManager.Instance.ShowNewView<ConnectZermeloView>();

                    timesClicked = 0;
                }
            });

            ToggleLeermiddelen.isOn = LocalPrefs.GetBool("show_leermiddelen", false);
            ToggleLeermiddelen.onValueChanged.AddListener((enabled) =>
            {
                if (enabled)
                {
                    LocalPrefs.SetBool("show_leermiddelen", true);
                    _vakken.Downloadvakken();
                }
                else
                {
                    LocalPrefs.SetBool("show_leermiddelen", false);
                }
                
                ViewManager.Instance.Refresh<NavBarView>();
            });

            
#if UNITY_ANDROID
            SendNotifButton.onClick.AddListener(() =>
            {
                lessonNotificationManager.SendTestNotification();
            });
#endif

            output.text = "Log:\n\n";
            Application.logMessageReceived += HandleLog;
            
            base.Initialize();
        }

        public override void Refresh(object args)
        {
            openNavigationButton.onClick.RemoveAllListeners();
            closeButtonWholePage.onClick.RemoveAllListeners();
            deletePlayerPrefsButton.onClick.RemoveAllListeners();
            ToggleLeermiddelen.onValueChanged.RemoveAllListeners();
            SendNotifButton.onClick.RemoveAllListeners();
            base.Refresh(args);
        }

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Log)
            {
                output.text += $"<color=black>{logString}</color>\n";
                output.text += $"-----------------\n";
            }
            else if (type == LogType.Warning)
            {
                output.text += $"<color=orange>{logString}</color>\n";

                string stacktrace = stackTrace;
                var m1 = Regex.Matches(stacktrace, @"((([A-Za-z]+\/)+)?[A-Z-a-z]+?(.[A-Za-z]+:[0-9]+))");

                foreach (Match match in m1)
                {
                    stacktrace = stacktrace.Replace(match.Value, $"<color=blue>{match.Value}</color>");
                }

                output.text += $"{stacktrace}";
                output.text += $"-----------------\n";
            }
            else if (type == LogType.Error)
            {
                output.text += $"<color=red>{logString}</color>\n";

                string stacktrace = stackTrace;
                var m1 = Regex.Matches(stacktrace, @"((([A-Za-z]+\/)+)?[A-Z-a-z]+?(.[A-Za-z]+:[0-9]+))");

                foreach (Match match in m1)
                {
                    stacktrace = stacktrace.Replace(match.Value, $"<color=blue>{match.Value}</color>");
                }

                output.text += $"{stacktrace}";
                output.text += $"-----------------\n";
            }
            else if (type == LogType.Exception)
            {
                output.text += $"<color=red>{logString}</color>\n";

                string stacktrace = stackTrace;
                var m1 = Regex.Matches(stacktrace, @"((([A-Za-z]+\/)+)?[A-Z-a-z]+?(.[A-Za-z]+:[0-9]+))");

                foreach (Match match in m1)
                {
                    stacktrace = stacktrace.Replace(match.Value, $"<color=blue>{match.Value}</color>");
                }

                output.text += $"{stacktrace}";
                output.text += $"-----------------\n";
            }
            else if (type == LogType.Assert)
            {
                output.text += $"<color=red>{logString}</color>\n";

                string stacktrace = stackTrace;
                var m1 = Regex.Matches(stacktrace, @"((([A-Za-z]+\/)+)?[A-Z-a-z]+?(.[A-Za-z]+:[0-9]+))");

                foreach (Match match in m1)
                {
                    stacktrace = stacktrace.Replace(match.Value, $"<color=blue>{match.Value}</color>");
                }

                output.text += $"{stacktrace}";
                output.text += $"-----------------\n";
            }
        }
    }
}
