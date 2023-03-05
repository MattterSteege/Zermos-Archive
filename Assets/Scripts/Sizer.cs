using System.Collections;
using System.Collections.Generic;
using MagneticScrollView;
using UnityEngine;

public class Sizer : MonoBehaviour
{
    [SerializeField] private MagneticScrollRect _scrollRect;
    
    [ContextMenu("Set Size")]
    public void Start()
    {
        _scrollRect.ElementsSize = new Vector2(_scrollRect.GetComponent<RectTransform>().rect.width, _scrollRect.GetComponent<RectTransform>().rect.height);
    }
}
