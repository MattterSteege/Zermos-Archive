using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Zermos_Web.Models.SomtodayVakgemiddeldenModel;

public class ExamendossierResultaat
{
    public double cijfer { get; set; }
    public bool isVoldoende { get; set; }
    public bool isVoldoendeEerstePoging { get; set; }
    public int periode { get; set; }
    public string formattedResultaat { get; set; }
    public string formattedEerstePoging { get; set; }
    public int volgnummer { get; set; }
    public string type { get; set; }
    public string toetscode { get; set; }
    public DateTime datumInvoerEerstePoging { get; set; }
    public bool isLabel { get; set; }
    public bool isCijfer { get; set; }
    public string label { get; set; }
    public string labelAfkorting { get; set; }
}

public class Gemiddelden
{
    public Vakkeuze vakkeuze { get; set; }
    public VoortgangsdossierResultaat voortgangsdossierResultaat { get; set; }
    public ExamendossierResultaat examendossierResultaat { get; set; }
}

public class RelevanteCijferLichting
{
    public AdditionalObjects additionalObjects { get; set; }
    public string naam { get; set; }
    public string UUID { get; set; }
}

public class SomtodayVakgemiddeldenModel
{
    //public List<Gemiddelden> gemiddelden { get; set; }
    public double voortgangsdossierGemiddelde { get; set; }
}

public class Schooljaar
{
    public AdditionalObjects additionalObjects { get; set; }
    public string naam { get; set; }
    public string vanafDatum { get; set; }
    public string totDatum { get; set; }
    public bool isHuidig { get; set; }
}

public class Vak
{
    public string afkorting { get; set; }
    public string naam { get; set; }
    public string UUID { get; set; }
}

public class Vakkeuze
{
    public Vak vak { get; set; }
    public bool vrijstelling { get; set; }
    public RelevanteCijferLichting relevanteCijferLichting { get; set; }
}

public class VoortgangsdossierResultaat
{
    public double cijfer { get; set; }
    public bool isVoldoende { get; set; }
    public int periode { get; set; }
    public string formattedResultaat { get; set; }
    public int volgnummer { get; set; }
    public string type { get; set; }
    public string toetscode { get; set; }
    public bool isLabel { get; set; }
    public bool isCijfer { get; set; }
}