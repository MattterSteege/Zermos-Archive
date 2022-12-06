using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UI.Views;

public class SuccesScreen : MonoBehaviour
{
    [SerializeField] private GameObject succesScreen;
    [SerializeField] private string succesText;
    [SerializeField] private TMP_Text screenText;
    
    // Start is called before the first frame update
    void Start()
    {
        //succesScreen.SetActive(false);
        RectTransform rect = succesScreen.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0f, -Screen.height * 2f);
    }

    [ContextMenu("Show Context menu")]
    public void testIt()
    {
        ShowSuccesScreen("Test");
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
