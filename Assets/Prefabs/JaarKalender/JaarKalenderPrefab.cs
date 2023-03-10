using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JaarKalenderPrefab : MonoBehaviour
{
    [SerializeField] public TMP_Text titleText;
    [SerializeField] public TMP_Text messageText;

    public void Initialize(JaarKalender.Datum datum)
    {
        //titleText.text = datum.startsAt.ToDateTime().ToString("dd MMMM") + " t/m " + datum.endsAt.ToDateTime().ToString("dd MMMM");
        messageText.text = datum.title;
    }
}
