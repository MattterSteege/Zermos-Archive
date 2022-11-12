using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UI.Views;
using UnityEngine;

public class GradeItemView : View
{
    [SerializeField] private List<Grades.Item> Grades;
    [SerializeField] private GameObject gradePrefab;
    [SerializeField] private TMP_Text TitleText;
    [SerializeField] private GameObject GradeContent;
    [SerializeField] private UILineRenderer UILineRenderer;

    public override void Show(object args = null)
    {
        this.Grades = (List<Grades.Item>)args;

        TitleText.text = Grades[0].vak.naam;

        UILineRenderer.gridSize = new Vector2Int(1, UILineRenderer.gridSize.y);
        UILineRenderer.points.Clear();
        UILineRenderer.AddPoint(float.Parse(Grades[0].geldendResultaat));

        foreach (Transform gameObject in GradeContent.transform)
        {
            Destroy(gameObject.gameObject);
        }
        
        for (var i = 0; i < Grades.Count; i++)
        {
            var grade = Grades[i];
            var gradeView = Instantiate(gradePrefab, GradeContent.transform);
            gradeView.GetComponent<GradeInfo>().SetGradeInfo(grade.vak.naam ?? "",
                grade.datumInvoer.ToString("d MMMM"), /*grade.omschrijving*/ "", grade.weging + "x",
                grade.geldendResultaat ?? "-");
            
            UILineRenderer.AddPoint(float.Parse(Grades[i].geldendResultaat));
        }

        base.Show(args);
    }

    public override void Initialize()
    {
        openNavigationButton.onClick.AddListener(() =>
        {
          ViewManager.Instance.ShowNewView<GradeView>();  
        });
        
        base.Initialize();
    }
}
