using System.Collections.Generic;

namespace Zermos_Web.Models.Somtoday;

public class SomtodayStatistiekenModel
{
    public SortedSomtodayGradesModel.Item item { get; set; }
    public int voldoendes { get; set; }
    public int onvoldoendes { get; set; }
    public List<int> mostCommonGrade { get; set; }
    public double lowest { get; set; }
    public double highest { get; set; }
    public double som { get; set; }
    public int weging { get; set; }
    public int wegingSE { get; set; }
    public double somSE { get; set; }
    public bool containsSE { get; set; }
    public bool containsVoortgang { get; set; }
}