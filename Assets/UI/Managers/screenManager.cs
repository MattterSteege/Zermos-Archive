using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class screenManager : MonoBehaviour
{
    void Start()
    {
#if UNITY_ANDROID
        Screen.fullScreen = false;
#endif

        if (PlayerPrefs.GetString("main_menu_settings", "") == "")
        {
            string defaultSettings;
            defaultSettings = "1"; //show paklijst
            PlayerPrefs.SetString("main_menu_settings", defaultSettings);
        }
    }
}
