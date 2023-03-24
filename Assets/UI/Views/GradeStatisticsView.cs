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
    
    [Header("Cijferoverzicht"), SerializeField] private LineChart Cijferoverzicht;
    
    [Header("Gemiddelde voortgangsgrafiek"), SerializeField] private LineChart GemiddeldeVoortgangsgrafiek;
    
    [Header("Voldoende - onvoldoende ratio"), SerializeField] private PieChart VoldoendeOnvoldoendeRatio;
    [SerializeField] private TMP_Text voldoendeText;
    [SerializeField] private TMP_Text onvoldoendeText;
    
    [Header("Gemiddelde"), SerializeField] private TMP_Text GemiddeldeText;
    
    [Header("Wat moet ik halen"), SerializeField] private TMP_InputField watMoetIkHalenCijferInput;
    [SerializeField] private TMP_InputField watMoetIkHalenWegingInput;
    [SerializeField] private TMP_Text watMoetIkHalenText;
    
    [Header("Wat moet ik halen"), SerializeField] private TMP_InputField WatGaIkStaanCijferInput;
    [SerializeField] private TMP_InputField WatGaIkStaanWegingInput;
    [SerializeField] private TMP_Text WatGaIkStaanText;
    

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

        //wat moet ik halen
        if (TryParseFloat(watMoetIkHalenCijferInput.text, out var result))
            watMoetIkHalenText.text = WatMoetIkHalen(Grades, Convert.ToInt32(watMoetIkHalenWegingInput.text), result);
        
        
        //wat ga ik staan
        if (TryParseFloat(WatGaIkStaanCijferInput.text, out var result2))
            WatGaIkStaanText.text = WatGaIkStaan(Grades, Convert.ToInt32(WatGaIkStaanWegingInput.text), result2);
        
        base.Show(args);
    }

    public override void Initialize()
    {
        backButton.onClick.AddListener(() =>
        {
            gameObject.GetComponentInParent<SubViewManager>().HideView<GradeStatisticsView>();
        });

        watMoetIkHalenCijferInput.onValueChanged.AddListener( (x) =>
        {
            if (watMoetIkHalenCijferInput.text == "" || watMoetIkHalenWegingInput.text == "") return;

                if (TryParseFloat(watMoetIkHalenCijferInput.text, out var result))
            {
                watMoetIkHalenText.text = WatMoetIkHalen(Grades, Convert.ToInt32(watMoetIkHalenWegingInput.text), result);
            }
        });
        watMoetIkHalenWegingInput.onValueChanged.AddListener( (x) =>
        {
            if (watMoetIkHalenWegingInput.text == "" || watMoetIkHalenCijferInput.text == "") return;

            if (TryParseFloat(watMoetIkHalenCijferInput.text, out var result))
            {
                watMoetIkHalenText.text = WatMoetIkHalen(Grades, Convert.ToInt32(watMoetIkHalenWegingInput.text), result);
            }
        });
        
        WatGaIkStaanCijferInput.onValueChanged.AddListener( (x) =>
        {
            if (WatGaIkStaanCijferInput.text == "" || WatGaIkStaanWegingInput.text == "") return;

            if (TryParseFloat(WatGaIkStaanCijferInput.text, out var result))
            {
                WatGaIkStaanText.text = WatGaIkStaan(Grades, Convert.ToInt32(WatGaIkStaanWegingInput.text), result);
            }
        });
        
        WatGaIkStaanWegingInput.onValueChanged.AddListener( (x) =>
        {
            if (WatGaIkStaanWegingInput.text == "" || WatGaIkStaanCijferInput.text == "") return;

            if (TryParseFloat(WatGaIkStaanCijferInput.text, out var result))
            {
                WatGaIkStaanText.text = WatGaIkStaan(Grades, Convert.ToInt32(WatGaIkStaanWegingInput.text), result);
            }
        });
        
        base.Initialize();
    }

    public override void Refresh(object args)
    {
        backButton.onClick.RemoveAllListeners();
        base.Refresh(args);
    }
    
    private string WatMoetIkHalen(List<Grades.Item> cijfers, int weging, float gewenstCijfer = 5.5f)
    {
        int totaleWeging = cijfers.Sum(x => x.weging);

        float alBehaaldePunten = cijfers.Sum(x => x.weging * float.Parse(x.geldendResultaat));
        
        float nogTeBehalen = gewenstCijfer * totaleWeging - alBehaaldePunten;

        return (gewenstCijfer + nogTeBehalen / weging).ToString("0.0");
    }
    
    private string WatGaIkStaan(List<Grades.Item> cijfers, int weging, float bijkomendCijfer = 5.5f)
    {
        int totaleWeging = cijfers.Sum(x => x.weging) + weging;
        float totalePunten = cijfers.Sum(x => x.weging * float.Parse(x.geldendResultaat)) + weging * bijkomendCijfer;
        
        return (totalePunten / totaleWeging).ToString("0.0");
    }
    
    public static bool TryParseFloat(string input, out float result)
    {
        // Check if the input is null or empty
        if (string.IsNullOrEmpty(input))
        {
            result = 0f;
            return false;
        }

        // Replace commas with dots, and remove any additional dots
        input = input.Replace(",", ".");
        int dotIndex = input.IndexOf('.');
        if (dotIndex != -1)
        {
            input = input.Remove(dotIndex, 1);
            input = input.Insert(dotIndex, ".");
        }

        // Try to parse the input as a float
        if (float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
        {
            return true;
        }
        else
        {
            result = 0f;
            return false;
        }
    }

}
