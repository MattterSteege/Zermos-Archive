using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SavedUserInformationItem : MonoBehaviour
{
    [SerializeField] TMP_Text key;
    [SerializeField] TMP_Text value;

    public void SetData(string dataKey, string dataValue)
    {
        key.text = dataKey;
        value.text = dataValue;
        
        GetComponent<Button>().onClick.AddListener(() =>
        {
            CopyToClipboard(dataValue);
        });
    }

    private void CopyToClipboard(string str) {
        TextEditor textEditor = new TextEditor();
        textEditor.text = str;
        textEditor.SelectAll();
        textEditor.Copy();
    }
}
