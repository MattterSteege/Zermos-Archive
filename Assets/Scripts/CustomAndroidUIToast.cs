using DG.Tweening;
using TMPro;
using UnityEngine;

public class CustomAndroidUIToast : MonoBehaviour
{
    [SerializeField] float TimeToReadOneWord = 0.1f;
    
    [Space, SerializeField] GameObject toastPrefab;
    [SerializeField] Transform toastParent;
    

    public static CustomAndroidUIToast Instance { get; private set; }
    
    public void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void ShowToast(string message)
    {
        var toast = Instantiate(toastPrefab, toastParent);
        var canvasGroup = toast.GetComponentInChildren<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.transform.parent.SetAsLastSibling();
        toast.GetComponentInChildren<TMP_Text>().text = message;

        canvasGroup.DOFade(1, 0.5f).SetDelay(0.1f).onComplete += () =>
        {
            canvasGroup.DOFade(0, 0.5f).SetDelay(1f + TimeToReadOneWord * (message.Split(' ').Length + 1)).onComplete += () =>
            {
                Destroy(toast);
            };
        };
    }
}

public static class AndroidUIToast
{
    public static object ShowToast(string message)
    {
        if (CustomAndroidUIToast.Instance == null)
            CustomAndroidUIToast.Instance.Start();
        
        CustomAndroidUIToast.Instance.ShowToast(message);
        return null;
    }
}
