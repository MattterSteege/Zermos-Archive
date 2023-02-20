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
    private LoginType loginType;
    [SerializeField] private Image backgroundOfEverything;

    public static SuccesScreen Instance { get; private set; }
    
    // Start is called before the first frame update
    void Start()
    {
        //succesScreen.SetActive(false);
        RectTransform rect = succesScreen.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0f, -Screen.height * 2f);
        Instance = this;
    }

    public void ShowSuccesScreen(LoginType connection)
    {
        succesScreen.SetActive(true);
        StartCoroutine(IShowSuccesScreen(connection));
    }
    
    public enum LoginType
    {
        somtoday,
        zermelo,
        infowijs
    }

    public IEnumerator IShowSuccesScreen(LoginType connection)
    {
        succesScreen.transform.SetAsLastSibling();
        
        if (connection == LoginType.somtoday)
            screenText.text = succesText.Replace("{koppeling name}","Somtoday");
        else if (connection == LoginType.zermelo)
            screenText.text = succesText.Replace("{koppeling name}","Zermelo");
        else if (connection == LoginType.infowijs)
            screenText.text = succesText.Replace("{koppeling name}","Antonius app");

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
            case LoginType.infowijs:
                ViewManager.Instance.Refresh<SchoolNewsView>();
                break;
        }
    }
}
