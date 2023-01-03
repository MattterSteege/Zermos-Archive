using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TMPro;
using UI.Views;
using UnityEngine;
using UnityEngine.UI;

public class GradeItemView : View
{
    [SerializeField] private List<Grades.Item> Grades;
    [SerializeField] private GameObject gradePrefab;
    [SerializeField] private TMP_Text TitleText;
    [SerializeField] private GameObject GradeContent;
    [SerializeField] private Button welkCijferMoetIkHalenButton;
    [SerializeField] private UnityEngine.UI.Extensions.UILineRenderer UILineRenderer;
    [SerializeField] private UnityEngine.UI.Extensions.UIGridRenderer UIGridRenderer;

    public override void Show(object args = null)
    {
        this.Grades = (List<Grades.Item>)args ?? new List<Grades.Item>();

        if (Grades.Count == 0) return;
        
        TitleText.text = Grades[0].vak.naam;

        UIGridRenderer.GridColumns = Grades.Count;
        UIGridRenderer.GridRows = 10;
        UILineRenderer.Points = new Vector2[Grades.Count + 1];
        UILineRenderer.Points[0] = new Vector2(0, float.Parse(Grades[0].geldendResultaat) / 10f);

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
            
            UILineRenderer.Points[i + 1] = new Vector2((1f / Grades.Count) * (i + 1), float.Parse(Grades[i].geldendResultaat) / 10f);
        }

        base.Show(args);
    }

    public override void Initialize()
    {
        openNavigationButton.onClick.AddListener(() =>
        {
          ViewManager.Instance.ShowNewView<GradeView>();  
        });
        
        welkCijferMoetIkHalenButton.onClick.AddListener(() =>
        {
            ViewManager.Instance.ShowNewView<WatMoetIkHalenView>(Grades);
        });
        
        base.Initialize();
    }
    
    public override void Refresh(object args)
    {
        openNavigationButton.onClick.RemoveAllListeners();
        welkCijferMoetIkHalenButton.onClick.RemoveAllListeners();
        base.Refresh(args);
    }
}
