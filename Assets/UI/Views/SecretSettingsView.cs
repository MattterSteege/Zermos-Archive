using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Views
{
    public class SecretSettingsView : View
    {
        [SerializeField] TMP_Text output;
        [SerializeField] Button deletePlayerPrefsButton;
        [SerializeField] Button ToggleLeermiddelen;
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
        
        
            // int timesClicked = 0;
            // deleteLocalPrefsButton.onClick.AddListener(() =>
            // {
            //     if (timesClicked < 5)
            //     {
            //         timesClicked++;
            //     }
            //     else
            //     {
            //         LocalPrefs.DeleteFile();
            //         LocalPrefs.Save();
            //
            //         ViewManager.Instance.ShowNewView<ConnectZermeloView>();
            //
            //         timesClicked = 0;
            //     }
            // });
        
            ToggleLeermiddelen.onClick.AddListener(() =>
            {
                bool enableLeermiddelen = LocalPrefs.GetBool("enable_leermiddelen");
                
                if (enableLeermiddelen)
                {
                    LocalPrefs.SetBool("enable_leermiddelen", false);
                }
                else
                {
                    LocalPrefs.SetBool("enable_leermiddelen", true);
                }
                _vakken.Downloadvakken();
                ViewManager.Instance.Refresh<NavBarView>();
            });

            SendNotifButton.onClick.AddListener(() =>
            {
#if UNITY_ANDROID
                lessonNotificationManager.SendTestNotification();
#endif
            });

            base.Initialize();
            
            output.text = "Log:\n\n";
            Application.logMessageReceived += HandleLog;
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
                output.text += $"<color=yellow>{logString}</color>\n";

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
