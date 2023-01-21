using System.Collections.Generic;
using E2C;
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
    [SerializeField] private E2Chart _chart;

    public override void Show(object args = null)
    {
        this.Grades = (List<Grades.Item>)args ?? new List<Grades.Item>();

        if (Grades.Count == 0) return;
        
        Grades.Reverse();
        
        TitleText.text = Grades[0].vak.naam;
        
        _chart.chartData.series[0].dataY.Clear();


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
            
            _chart.chartData.series[0].dataY.Add(float.Parse(Grades[Grades.Count - i - 1].geldendResultaat));
        }

        _chart.UpdateChart();
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
