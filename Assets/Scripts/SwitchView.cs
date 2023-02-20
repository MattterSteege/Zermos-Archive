using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UI.Views;
using UnityEngine;

public class SwitchView : MonoBehaviour
{
    [SerializeField] private RectTransform[] MenuParts;
    [SerializeField] private Vector2[] _MenuPartsPos;
    [SerializeField] float waitTime = 0.1f;
    [SerializeField] float MovementSpeed = 0.5f;
    [SerializeField] TMP_Text ViewText;
    public static SwitchView Instance;

    void Awake()
    {
        Instance = this;
        GetComponent<CanvasGroup>().alpha = 1;
        _MenuPartsPos = new Vector2[MenuParts.Length];
        for (int i = 0; i < MenuParts.Length; i++)
        {
            _MenuPartsPos[i] = MenuParts[i].anchoredPosition;
            MenuParts[i].anchoredPosition = new Vector2(Screen.width + 40, MenuParts[i].anchoredPosition.y);
        }
    }

    public void Show<TView>(object args = null) where TView : View
    {
        foreach (View view in ViewManager.Instance.views)
        {
            if (view is TView)
            {
                StartCoroutine(Animate(view, args));
            }
        }
    }
    
    IEnumerator Animate(View view, object args)
    {
        if (view == ViewManager.Instance.currentView)
            yield break;
        
        //random number either -1 or 1
        Random.InitState(TimeManager.Instance.CurrentDateTime.ToUnixTime());
        float random = Random.Range(-2f, 2f);
        ViewText.text = view.viewName;
        yield return null;
        for (int i = 0; i < MenuParts.Length; i++)
        {
            MenuParts[i].anchoredPosition = new Vector2((Screen.width + 80) * (random > 0 ? 1 : -1), MenuParts[i].anchoredPosition.y);
            Vector2 pos = _MenuPartsPos[i];
            pos.x = 0;

            MenuParts[i].DOAnchorPos(pos, MovementSpeed);
            yield return new WaitForSeconds(waitTime);
        }

        yield return new WaitForSeconds(waitTime * MenuParts.Length);

        ViewManager.Instance.HideCurrentView();
        view.Show(args);
        ViewManager.Instance.currentView = view;
        
        Random.InitState(TimeManager.Instance.CurrentDateTime.ToUnixTime());
        random = Random.Range(-1f, 1f);
        yield return null;
        
        for (int i = 0; i < MenuParts.Length; i++)
        {
            Vector2 pos = _MenuPartsPos[i];
            pos.x = -(Screen.width + 80) * (random > 0 ? 1 : -1);

            MenuParts[i].DOAnchorPos(pos, MovementSpeed);
            yield return new WaitForSeconds(waitTime);
        }
    }
}
