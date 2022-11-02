using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class CijferView : View
{
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject gradePrefab;
    [SerializeField] private Grades gradesObject;
    
    [SerializeField] private GameObject LastGradeContentGameObject;
    private List<Grades.Item> lastGrades = new();

    [ContextMenu("Refresh")]
    public override void Initialize()
    {
        var grades = gradesObject.getGrades();
        if (grades == null) return;

        lastGrades = grades.items.TakeLast(2).ToList();
        
        foreach (Transform child in content.transform)
            Destroy(child.gameObject);
        
        foreach (Transform child in LastGradeContentGameObject.transform)
            Destroy(child.gameObject);

        foreach (var grade in lastGrades)
        {
            var gradeView = Instantiate(gradePrefab, LastGradeContentGameObject.transform);
            gradeView.GetComponent<GradeInfo>().SetGradeInfo(grade.vak.naam ?? "",
                grade.datumInvoer.ToString("d MMMM"), /*grade.omschrijving*/ "", grade.weging + "x",
                grade.geldendResultaat ?? "-");
        }

        foreach (var grade in grades.items)
        {

            var gradeView = Instantiate(gradePrefab, content.transform);
                gradeView.GetComponent<GradeInfo>().SetGradeInfo(grade.vak.naam ?? "",
                    grade.datumInvoer.ToString("d MMMM"), /*grade.omschrijving*/ "", grade.weging + "x",
                    grade.geldendResultaat ?? "-");

        }
        
        base.Initialize();
    }
}

public static class MiscExtensions
{
    // Ex: collection.TakeLast(5);
    public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int N)
    {
        return source.Skip(Math.Max(0, source.Count() - N));
    }
}