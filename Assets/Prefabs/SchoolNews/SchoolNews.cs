using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SchoolNews : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    
    public void Initialize(Message message)
    {
        text.text = message.content;
    }
}
