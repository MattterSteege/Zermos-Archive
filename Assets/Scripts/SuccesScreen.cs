using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UI.Views;
using UnityEngine.UI;

public class SuccesScreen : MonoBehaviour
{
    [SerializeField] private GameObject succesScreen;
    [SerializeField] private string succesText;
    [SerializeField] private TMP_Text screenText;
    [SerializeField] private Button restartButton;
    
    // Start is called before the first frame update
    void Start()
    {
        //succesScreen.SetActive(false);
        RectTransform rect = succesScreen.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0f, -Screen.height * 2f);
        restartButton.onClick.AddListener(RestartApplication);
    }

    private void RestartApplication()
    {
        if (Application.isEditor) return;

        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
            const int kIntent_FLAG_ACTIVITY_CLEAR_TASK = 0x00008000;
            const int kIntent_FLAG_ACTIVITY_NEW_TASK = 0x10000000;

            var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var pm = currentActivity.Call<AndroidJavaObject>("getPackageManager");
            var intent = pm.Call<AndroidJavaObject>("getLaunchIntentForPackage", Application.identifier);

            intent.Call<AndroidJavaObject>("setFlags", kIntent_FLAG_ACTIVITY_NEW_TASK | kIntent_FLAG_ACTIVITY_CLEAR_TASK);
            currentActivity.Call("startActivity", intent);
            currentActivity.Call("finish");
            var process = new AndroidJavaClass("android.os.Process");
            int pid = process.CallStatic<int>("myPid");
            process.CallStatic("killProcess", pid);
        }
    }

    public void ShowSuccesScreen(string koppelingName = "")
    {
        succesScreen.SetActive(true);
        StartCoroutine(IShowSuccesScreen(koppelingName));
    }
    
    public IEnumerator IShowSuccesScreen(string screenText)
    {
        succesScreen.transform.SetAsLastSibling();
        
        this.screenText.text = succesText.Replace("{koppeling name}", screenText);
        
        RectTransform rect = succesScreen.GetComponent<RectTransform>();
        
        rect.anchoredPosition = new Vector2(0f, -Screen.height * 2f);
        
        yield return new WaitForSeconds(0.3f);
        
        rect.DOAnchorPosY(0f, 2f);
        yield return new WaitForSeconds(2f);

        rect.GetComponent<CanvasGroup>().DOFade(1f, 10f).onComplete += () =>
        {
            Application.Quit(200);
        };
    }
}
