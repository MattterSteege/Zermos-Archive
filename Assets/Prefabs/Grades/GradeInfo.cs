using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class GradeInfo : MonoBehaviour
{
    public void SetGradeInfo(string vak = null, string datum = null, string details = null, string weging = null, string cijfer = null)
    {
        this.vak.text = vak ?? "";
        this.datum.text = datum ?? "";
        this.details.text = details ?? "";
        this.weging.text = weging ?? "";
        this.cijfer.text = cijfer ?? "";
    }

    [SerializeField] TMP_Text vak;
    [SerializeField] TMP_Text datum;
    [SerializeField] TMP_Text details;
    [SerializeField] TMP_Text weging;
    [SerializeField] TMP_Text cijfer;
}
