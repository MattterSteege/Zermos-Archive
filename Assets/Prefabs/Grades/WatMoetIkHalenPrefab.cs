using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WatMoetIkHalenPrefab : MonoBehaviour
{
    [SerializeField] TMP_Text cijferText;
    [SerializeField] TMP_Text wegingText;

    public void SetGradeInfo(string cijfer, string weging)
    {
        cijferText.text = cijfer;
        wegingText.text = weging;
    }
}
