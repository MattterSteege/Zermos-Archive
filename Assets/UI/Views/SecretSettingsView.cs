using System;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class SecretSettingsView : SubView
    {
        [SerializeField] TMP_Text output;
        [SerializeField] Button deletePlayerPrefsButton;

        [Header("Send notif")]
        [SerializeField] private Button SendNotifButton;
        [SerializeField] private LessonNotificationManager lessonNotificationManager;

        public override void Initialize()
        {
            backButton.onClick.AddListener(() =>
            {
                gameObject.GetComponentInParent<SubViewManager>().ShowParentView();
            });
            
            int timesClicked = 0;
            deletePlayerPrefsButton.onClick.AddListener(() =>
            {
                if (timesClicked <= 4)
                {
                    timesClicked++;
                }
                else
                {
                    string[] filePaths = Directory.GetFiles(Application.persistentDataPath);
                    foreach (string filePath in filePaths)
                    {
                        File.Delete(filePath);
                    }
                    
                    LocalPrefs.Load();
                    ViewManager.Instance.ShowNewView<ConnectZermeloView>();

                    timesClicked = 0;
                }
            });
            
            
#if UNITY_ANDROID
            SendNotifButton.onClick.AddListener(() =>
            {
                lessonNotificationManager.SendTestNotification();
            });
#endif

            if (output.text == "")
                output.text = "Log:\n\n";

            base.Initialize();
        }

        public override void Show(object args = null)
        {
            Application.logMessageReceived -= HandleLog;
            Application.logMessageReceived += HandleLog;
            base.Show(args);
        }

        public override void Refresh(object args)
        {
            backButton.onClick.RemoveAllListeners();
            deletePlayerPrefsButton.onClick.RemoveAllListeners();
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
