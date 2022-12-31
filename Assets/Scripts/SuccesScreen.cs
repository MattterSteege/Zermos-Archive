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
    [SerializeField] private LoginType loginType;
    [SerializeField] private Image backgroundOfEverything;
    [SerializeField] private Image koppelingAdded;

    // Start is called before the first frame update
    void Start()
    {
        //succesScreen.SetActive(false);
        RectTransform rect = succesScreen.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0f, -Screen.height * 2f);
    }

    public void ShowSuccesScreen(string koppelingName = "")
    {
        succesScreen.SetActive(true);
        StartCoroutine(IShowSuccesScreen(koppelingName));
    }
    
    public enum LoginType
    {
        somtoday,
        zermelo,
        infowijs
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
        Refresh();
        yield return new WaitForSeconds(1f);
        rect.DOAnchorPosY(-Screen.height * 2f, 2f);
    }

    private void Refresh()
    {
        backgroundOfEverything.color = new Color(0.06666667f, 0.1529412f, 0.4352941f);
        ViewManager.Instance.Show<NavBarView>();
        ViewManager.Instance.Refresh<NavBarView>();
        koppelingAdded.color = new Color(0.172549f, 0.9333333f, 0.5568628f);
        switch (loginType)
        {
            case LoginType.somtoday:
                ViewManager.Instance.Refresh<HomeworkView>();
                ViewManager.Instance.Refresh<GradeView>();
                break;
            case LoginType.zermelo:
                ViewManager.Instance.Refresh<DagRoosterView>(false);
                ViewManager.Instance.Refresh<WeekRoosterView>(false);
                break;
        }
    }
}
