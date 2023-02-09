using System;
using System.Collections.Generic;
using E2C;
using TMPro;
using UI.Views;
using UnityEngine;
using UnityEngine.UI;

public class GradeStatisticsView : View
{
    [SerializeField] private List<Grades.Item> Grades;

    [SerializeField] private E2Chart gradeChart;
    [SerializeField] private TMP_Text TitleText;

    public override void Show(object args = null)
    {
        this.Grades = (List<Grades.Item>)args ?? new List<Grades.Item>();

        if (Grades.Count == 0) return;
        
        Grades.Reverse();
        
        TitleText.text = Grades[0].vak.naam + " - statistieken";
        
        gradeChart.chartData.series[0].dataY.Clear();
        gradeChart.chartData.series[1].dataY.Clear();

        double average = 0;
        int weight = 0;

        foreach (var gradeItem in Grades)
        {
            gradeChart.chartData.series[0].dataY.Add(float.Parse(gradeItem.geldendResultaat));
            
            average += Convert.ToDouble(gradeItem.geldendResultaat) * gradeItem.weging;
            weight += gradeItem.weging;
            gradeChart.chartData.series[1].dataY.Add(MathF.Round((float) (average / weight), 1, MidpointRounding.AwayFromZero));
        }

        MonoBehaviour camMono = ViewManager.Instance.GetComponent<MonoBehaviour>();
        camMono.StartCoroutine(gradeChart.Start());
        base.Show(args);
    }

    public override void Initialize()
    {
        openNavigationButton.onClick.AddListener(() =>
        {
            ViewManager.Instance.ShowNewView<GradeItemView>(Grades);
        });
        
        gradeChart.UpdateChart();

        base.Initialize();
    }
    
    public override void Refresh(object args)
    {
        openNavigationButton.onClick.RemoveAllListeners();
        base.Refresh(args);
    }
}
