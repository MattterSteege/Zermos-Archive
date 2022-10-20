using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] TMP_Text loadingText;
    [SerializeField] float timeBetweenDots = 0.2f;
    [SerializeField] float ColordropDuration = 0.2f;

    [SerializeField] private RectTransform yellow;
    [SerializeField] private RectTransform green;
    [SerializeField] private RectTransform blue;
    [SerializeField] private RectTransform red;
    
    [SerializeField] GameObject loadingScreen;
    
    bool done = false;
    
    IEnumerator Start()
    {
        ViewManager.onInitializeComplete += Complete;

        StartCoroutine(LoadingText());
        StartCoroutine(ColorDrop());
        
        yield return new WaitWhile(() => done == false);

        StopCoroutine(LoadingText());
        StopCoroutine(ColorDrop());
        
        yield return null;
    }

    private void Complete()
    {
        done = true;
        Destroy(loadingScreen);
        
        ViewManager.Instance.Refresh<MainMenuView>();
        ViewManager.Instance.Show<MainMenuView, NavBarView>();
    }

    IEnumerator LoadingText()
    {
        loadingText.text = "Loading . ";
        yield return new WaitForSeconds(timeBetweenDots);
        loadingText.text = "Loading...";
        yield return new WaitForSeconds(timeBetweenDots);
        loadingText.text = "Loading. .";
        yield return new WaitForSeconds(timeBetweenDots);
        loadingText.text = "Loading...";
        yield return new WaitForSeconds(timeBetweenDots);

        if(done == false)
        {
            StartCoroutine(LoadingText());
        }
        
        yield return null;
    }
    
    [ContextMenu("Color Drop Test")]
    public void ColorDropTest()
    {
        yellow.anchoredPosition = new Vector2(-Screen.width, 0);
        green.anchoredPosition = new Vector2(-Screen.width, 0);
        blue.anchoredPosition = new Vector2(-Screen.width, 0);
        red.anchoredPosition = new Vector2(-Screen.width, 0);
        
        yellow.localRotation = Quaternion.Euler(0, 0, 0);
        green.localRotation = Quaternion.Euler(0, 0, 0);
        blue.localRotation = Quaternion.Euler(0, 0, 0);
        red.localRotation = Quaternion.Euler(0, 0, 0);

        StartCoroutine(ColorDrop());
    }

    IEnumerator ColorDrop()
    {
        yield return new WaitForSeconds(ColordropDuration / 10f);
        yellow.DORotate(new Vector3(0f, 0f, -90f), ColordropDuration);
        yield return new WaitForSeconds(ColordropDuration / 10f);
        green.DORotate(new Vector3(0f, 0f, -90f), ColordropDuration);
        yield return new WaitForSeconds(ColordropDuration / 10f);
        red.DORotate(new Vector3(0f, 0f, -90f), ColordropDuration);
        yield return new WaitForSeconds(ColordropDuration / 10f);
        blue.DORotate(new Vector3(0f, 0f, -90f), ColordropDuration);
        yield return new WaitForSeconds(4f);
        
        if(done == false)
        {
            StartCoroutine(ColorDrop());
        }
        
        yield return null;
    }
}
