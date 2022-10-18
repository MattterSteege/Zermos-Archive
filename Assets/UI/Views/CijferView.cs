using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CijferView : View
{
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject gradePrefab;
    [SerializeField] private Grades gradesObject;

    [ContextMenu("Refresh")]
    public override void Initialize()
    {
        var grades = gradesObject.getGrades();
        if (grades == null) return;

        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }
        
        foreach (var grade in grades.items)
        {
            try
            {
                if (grade.geldendResultaat == null ||
                    (string.IsNullOrEmpty(grade.omschrijving) && grade.weging == 0)) continue;

                var gradeView = Instantiate(gradePrefab, content.transform);
                gradeView.GetComponent<GradeInfo>().SetGradeInfo(grade.vak.naam ?? "",
                    grade.datumInvoer.ToString("d MMMM"), /*grade.omschrijving*/ "", grade.weging + "x",
                    grade.geldendResultaat ?? "-");
            }
            catch (Exception) { }


            base.Initialize();
        }
    }
}