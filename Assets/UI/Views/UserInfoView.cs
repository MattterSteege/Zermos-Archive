using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UserInfoView : View
{
    [SerializeField] private TMP_Text NaamText;
    [SerializeField] private TMP_Text CodeText;
    [SerializeField] private TMP_Text ZermeloAuthCodeText;
    [SerializeField] private TMP_Text SchoolAbbreviationText;
    [SerializeField] private GameObject ZermeloManager;

    public override void Initialize()
    {
        string fullName = PlayerPrefs.GetString("zermelo-full_name");
        string userCode = PlayerPrefs.GetString("zermelo-user_code");
        string accessToken = PlayerPrefs.GetString("zermelo-access_token");
        string schoolCode = PlayerPrefs.GetString("zermelo-school_code");
        
        if (string.IsNullOrEmpty(fullName))
        {
            NaamText.gameObject.SetActive(false);
        }
        else
        {
            NaamText.text = fullName;
        }
        
        if (string.IsNullOrEmpty(userCode))
        {
            CodeText.gameObject.SetActive(false);
        }
        else
        {
            CodeText.text = userCode;
        }
        
        if (string.IsNullOrEmpty(accessToken))
        {
            ZermeloAuthCodeText.gameObject.SetActive(false);
        }
        else
        {
            ZermeloAuthCodeText.text = accessToken;
        }
        
        if (string.IsNullOrEmpty(schoolCode))
        {
            SchoolAbbreviationText.gameObject.SetActive(false);
        }
        else
        {
            SchoolAbbreviationText.text = schoolCode;
        }

        base.Initialize();
    }
}
