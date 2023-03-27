using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class OpenMenu : MonoBehaviour
{
    [SerializeField] private RectTransform[] MenuParts;
    [SerializeField] private Vector2[] _MenuPartsPos;
    [SerializeField] float waitTime = 0.1f;
    [SerializeField] float MovementSpeed = 0.5f;
    
    private void Awake()
    {
        _MenuPartsPos = new Vector2[MenuParts.Length];
        for (int i = 0; i < MenuParts.Length; i++)
        {
            _MenuPartsPos[i] = MenuParts[i].anchoredPosition;
        }
        
    }
    
    [ContextMenu("Close Menu")]
    public void _CloseMenu()
    {
        StartCoroutine(_CloseMenuCoroutine());
    }
    
    IEnumerator _CloseMenuCoroutine()
    {
        yield return null;
        for (int i = 0; i < MenuParts.Length; i++)
        {
            Vector2 pos = _MenuPartsPos[i];
            pos.x -= 1000;

            MenuParts[i].DOAnchorPos(pos, MovementSpeed);
            yield return new WaitForSeconds(waitTime);
        }
    }
    
    [ContextMenu("Open Menu")]
    public void _OpenMenu()
    {
        StartCoroutine(_OpenMenuCoroutine());
    }
    
    IEnumerator _OpenMenuCoroutine()
    {
        yield return null;
        for (int i = 0; i < MenuParts.Length; i++)
        {
            Vector2 pos = _MenuPartsPos[i];
            //pos.y = _MenuPartsPos[i].y * 2;
            
            MenuParts[i].DOAnchorPos(pos, MovementSpeed);
            yield return new WaitForSeconds(waitTime);
        }
    }
}
