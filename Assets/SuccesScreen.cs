using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

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
        rect.anchoredPosition = new Vector2(0f, -Screen.height);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    [ContextMenu("Show Context menu")]
    public void ShowSuccesScreen(string koppelingName)
    {
        succesScreen.SetActive(true);
        StartCoroutine(IShowSuccesScreen(koppelingName));
    }
    
    public IEnumerator IShowSuccesScreen(string screenText)
    {
        this.screenText.text = succesText.Replace("{koppeling name}", screenText);
        
        RectTransform rect = succesScreen.GetComponent<RectTransform>();
        
        rect.anchoredPosition = new Vector2(0f, -Screen.height);
        
        yield return new WaitForSeconds(0.3f);
        
        rect.DOAnchorPosY(0f, 2f);
        yield return new WaitForSeconds(2f);
        
        ViewManager.Instance.Show<NavBarView, DagRoosterView>();
        ViewManager.Instance.Refresh<NavBarView>();
        ViewManager.Instance.Refresh<HomeworkView>();
        ViewManager.Instance.Refresh<CijferView>();
        ViewManager.Instance.Refresh<DagRoosterView>();
        
        yield return new WaitForSeconds(0.25f);
        
        rect.DOAnchorPosY(Screen.height * 1.1f, 2f);
        yield return new WaitForSeconds(2f);
        
        succesScreen.SetActive(false);
    }
}
