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
        this.weging.text = weging ?? "";
        this.cijfer.text = cijfer ?? "";
        
        if (details?.Length > 0 && datum?.Length > 0)
        {
            this.datum.text = details;
        }
        else if (details?.Length > 0)
        {
            this.datum.text = details;
        }
        else if (datum?.Length > 0)
        {
            this.datum.text = datum;
        }
        else
        {
            this.datum.gameObject.SetActive(false);
        }

        if (float.Parse(cijfer ?? "0.0") >= 8.0f)
        {
            this.cijfer.color = new Color(0.172549f, 0.702f, 0.028f);
        }
        else if (float.Parse(cijfer ?? "0.0") >= 5.5f)
        {
            this.cijfer.color = new Color(0.172549f, 0.702f, 0.028f);
        }
        else
        {
            this.cijfer.color = new Color(0.929411f, 0.203921f, 0.098039f);
        }
    }

    [SerializeField] TMP_Text vak;
    [SerializeField] TMP_Text datum;
    [SerializeField] TMP_Text weging;
    [SerializeField] TMP_Text cijfer;
}
