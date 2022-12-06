using DG.Tweening;
using TMPro;
using UI.Views;
using UnityEngine;
using UnityEngine.UI;

public class SavedInformationView : View
{
    [Space]
    [SerializeField] private TMP_Text NaamText;
    [SerializeField] private Button NaamTextButton;
    
    [SerializeField, Space] private TMP_Text CodeText;
    [SerializeField] private Button CodeTextButton;
    
    [SerializeField, Space] private TMP_Text ZermeloAuthCodeText;
    [SerializeField] private Button ZermeloAuthCodeTextButton;
    
    [SerializeField, Space] private TMP_Text SchoolAbbreviationText;
    [SerializeField] private Button SchoolAbbreviationTextButton;
    
    [SerializeField, Space] private TMP_Text SomtodayAuthCodeText;
    [SerializeField] private Button SomtodayAuthCodeTextButton;
    
    [SerializeField] private TMP_Text CopiedText;
    
    [SerializeField, Space] private TMP_Text filePathText;
    [SerializeField] private Button filePathTextButton;
    

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
        
        string fullName = LocalPrefs.GetString("zermelo-full_name");
        string userCode = LocalPrefs.GetString("zermelo-user_code");
        string zermeloAccessToken = LocalPrefs.GetString("zermelo-access_token");
        string schoolCode = LocalPrefs.GetString("zermelo-school_code");
        string somtodayAccessToken = LocalPrefs.GetString("somtoday-access_token");
        string filePath = LocalPrefs.GetString("file_path");
     
        CopiedText.gameObject.transform.parent.gameObject.GetComponent<CanvasGroup>().alpha = 0;
        
        if (string.IsNullOrEmpty(fullName))
        {
            NaamText.gameObject.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            NaamText.text = fullName;
            
            NaamTextButton.onClick.AddListener(() =>
            {
                CopyToClipboard(fullName);
                copyComplete("naam");
            });
        }
        
        if (string.IsNullOrEmpty(userCode))
        {
            CodeText.gameObject.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            CodeText.text = userCode;
            
            CodeTextButton.onClick.AddListener(() =>
            {
                CopyToClipboard(userCode);
                copyComplete("leerling code");
            });
        }
        
        if (string.IsNullOrEmpty(zermeloAccessToken))
        {
            ZermeloAuthCodeText.gameObject.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            ZermeloAuthCodeText.text = zermeloAccessToken.Substring(0, 15) + "...";
            
            ZermeloAuthCodeTextButton.onClick.AddListener(() =>
            {
                CopyToClipboard(zermeloAccessToken);
                copyComplete("Zermelo auth code");
            });
        }
        
        if (string.IsNullOrEmpty(schoolCode))
        {
            SchoolAbbreviationText.gameObject.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            SchoolAbbreviationText.text = schoolCode;
            
            SchoolAbbreviationTextButton.onClick.AddListener(() =>
            {
                CopyToClipboard(schoolCode);
                copyComplete("school code");
            });
        }
        
        if (string.IsNullOrEmpty(somtodayAccessToken))
        {
            SomtodayAuthCodeText.gameObject.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            SomtodayAuthCodeText.text = somtodayAccessToken.Substring(0, 15) + "...";
            
            SomtodayAuthCodeTextButton.onClick.AddListener(() =>
            {
                CopyToClipboard(somtodayAccessToken);
                copyComplete("Somtoday auth code");
            });
        }
        
        if (string.IsNullOrEmpty(filePath))
        {
            filePathText.gameObject.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            filePathText.text = filePath.Substring(0, 15) + "...";
            
            filePathTextButton.onClick.AddListener(() =>
            {
                CopiedText.text = "Opening file explorer";
        
                CopiedText.gameObject.transform.parent.gameObject.GetComponent<CanvasGroup>().DOFade(1, 0.5f).onComplete += () =>
                {
                    CopiedText.gameObject.transform.parent.gameObject.GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetDelay(1f);
                };

                Application.OpenURL(filePath.Replace(@"/CustomHomework.json", ""));
            });
        }

        base.Initialize();
    }
    
    private void CopyToClipboard(string str) {
        TextEditor textEditor = new TextEditor();
        textEditor.text = str;
        textEditor.SelectAll();
        textEditor.Copy();
    }

    private void copyComplete(string copiedText)
    {
        CopiedText.text = copiedText + " Gekopieerd!";
        
        CopiedText.gameObject.transform.parent.gameObject.GetComponent<CanvasGroup>().DOFade(1, 0.5f).onComplete += () =>
        {
            CopiedText.gameObject.transform.parent.gameObject.GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetDelay(1f);
        };
    }
}
