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
        try
        {
            var grades = gradesObject.getGrades();
            if (grades == null) return;
            
            foreach (var grade in grades.items)
            {
                var gradeView = Instantiate(gradePrefab, content.transform);
                gradeView.GetComponent<GradeInfo>().SetGradeInfo(grade.vak.naam ?? "",
                    grade.datumInvoer.ToString("dd MMMM"), grade.omschrijving ?? "", grade.weging + "x",
                    grade.resultaat ?? "-");
            }
        }
        catch (Exception) { }


        base.Initialize();
    }
}