using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AwesomeCharts;
using E2C;
using TMPro;
using UI.Views;
using UnityEngine;
using UnityEngine.UI;

public class GradeStatisticsView : SubView
{
    [SerializeField] private List<Grades.Item> Grades;

    [Header("Charts")]
    [SerializeField] private LineChart Cijferoverzicht;
    [SerializeField] private LineChart GemiddeldeVoortgangsgrafiek;
    [SerializeField] private PieChart VoldoendeOnvoldoendeRatio;
    [SerializeField] private TMP_Text voldoendeText;
    [SerializeField] private TMP_Text onvoldoendeText;
    [SerializeField] private TMP_Text GemiddeldeText;

    public override void Show(object args = null)
    {
        this.Grades = (List<Grades.Item>)args ?? new List<Grades.Item>();

        if (Grades.Count == 0) return;
        
        Grades = Grades.OrderBy(x => x.datumInvoer).ToList();

        Cijferoverzicht.GetChartData().DataSets.Clear();
        GemiddeldeVoortgangsgrafiek.GetChartData().DataSets.Clear();
        VoldoendeOnvoldoendeRatio.GetChartData().DataSet.Clear();

        double average = 0;
        int weight = 0;

        LineDataSet setCijferoverzicht = new LineDataSet(); //cijferoverzicht
        LineDataSet setGemiddeldeVoortgangsgrafiek = new LineDataSet(); //gemiddelde voortgangsgrafiek
        PieDataSet setVoldoendeOnvoldoendeRatio = new PieDataSet(); //voldoende onvoldoende ratio
        setVoldoendeOnvoldoendeRatio.AddEntry(new PieEntry(0, "Voldoende", new Color(0.1803922f, 0.9333333f, 0.5568628f, 1f)));
        setVoldoendeOnvoldoendeRatio.AddEntry(new PieEntry(0, "Onvoldoende", new Color(0.9921569f, 0.4509804f, 0.4431373f, 1f)));

        for (var i = 0; i < Grades.Count; i++)
        {
            var gradeItem = Grades[i];
            double geldendResultaat = Convert.ToDouble(gradeItem.geldendResultaat);
            average += geldendResultaat * (gradeItem.weging == 0 ? gradeItem.examenWeging : gradeItem.weging);
            weight += gradeItem.weging == 0 ? gradeItem.examenWeging : gradeItem.weging;
            
            //charting
            setCijferoverzicht.AddEntry(new LineEntry(i, (float) geldendResultaat));
            setGemiddeldeVoortgangsgrafiek.AddEntry(new LineEntry(i, MathF.Round((float) (average / weight), 1, MidpointRounding.AwayFromZero)));
            setVoldoendeOnvoldoendeRatio.Entries[Convert.ToDouble(gradeItem.geldendResultaat) >= 5.5 ? 0 : 1].Value += 1f;
            GemiddeldeText.text = MathF.Round((float) (average / weight), 2, MidpointRounding.AwayFromZero).ToString("0.00", CultureInfo.InvariantCulture);
        }
        
        
        
        //cijferoverzicht
        setCijferoverzicht.Title = "Cijferoverzicht";
        setCijferoverzicht.LineColor = new Color(0.3921569f, 0.572549f, 0.9764706f, 1f);
        setCijferoverzicht.FillColor = new Color(0.3921569f, 0.572549f, 0.9764706f, 0.1960784f);
        setCijferoverzicht.LineThickness = 3f;
        setCijferoverzicht.UseBezier = true;
        Cijferoverzicht.GetChartData().DataSets.Add(setCijferoverzicht);
        Cijferoverzicht.SetDirty();

        //gemiddelde voortgangsgrafiek
        setGemiddeldeVoortgangsgrafiek.Title = "Gemiddelde Voortgangsgrafiek";
        setGemiddeldeVoortgangsgrafiek.LineColor = new Color(0.9843137f, 0.7882353f, 0.254902f, 1f);
        setGemiddeldeVoortgangsgrafiek.FillColor = new Color(0.9843137f, 0.7882353f, 0.254902f, 0.1960784f);
        setGemiddeldeVoortgangsgrafiek.LineThickness = 3f;
        setGemiddeldeVoortgangsgrafiek.UseBezier = true;
        GemiddeldeVoortgangsgrafiek.GetChartData().DataSets.Add(setGemiddeldeVoortgangsgrafiek);
        GemiddeldeVoortgangsgrafiek.SetDirty();
        
        //voldoende onvoldoende ratio
        voldoendeText.text = "Voldoendes: " + setVoldoendeOnvoldoendeRatio.Entries[0].Value.ToString();
        onvoldoendeText.text = "Onvoldoendes: " + setVoldoendeOnvoldoendeRatio.Entries[1].Value.ToString();
        VoldoendeOnvoldoendeRatio.GetChartData().DataSet = setVoldoendeOnvoldoendeRatio;
        VoldoendeOnvoldoendeRatio.SetDirty();

        base.Show(args);
    }

    public override void Initialize()
    {
        backButton.onClick.AddListener(() =>
        {
            gameObject.GetComponentInParent<SubViewManager>().HideView<GradeStatisticsView>();
        });

        base.Initialize();
    }
    
    public override void Refresh(object args)
    {
        backButton.onClick.RemoveAllListeners();
        base.Refresh(args);
    }
}
