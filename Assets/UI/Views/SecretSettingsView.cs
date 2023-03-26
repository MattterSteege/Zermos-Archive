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

        public override void Refresh(object args)
        {
            backButton.onClick.RemoveAllListeners();
            deletePlayerPrefsButton.onClick.RemoveAllListeners();
            SendNotifButton.onClick.RemoveAllListeners();
            base.Refresh(args);
        }
    }
}
