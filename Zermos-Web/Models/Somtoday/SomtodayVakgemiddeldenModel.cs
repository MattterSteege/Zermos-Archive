using System;
using System.Collections.Generic;

namespace Zermos_Web.Models.SomtodayVakgemiddeldenModel;

public class Gemiddelden
{
    public string vakAfkorting => vakkeuze.vak.afkorting;
    public string vakNaam => vakkeuze.vak.naam;
    public string vakUUID => vakkeuze.vak.UUID;
    public string relevanteCijferLichtingUUID => vakkeuze.relevanteCijferLichting.UUID;
    public Vakkeuze vakkeuze { get; set; }
    public VoortgangsdossierResultaat voortgangsdossierResultaat { get; set; }
    public ExamendossierResultaat examendossierResultaat { get; set; }
    public bool isVoorVoortgangsdossier => voortgangsdossierResultaat != null;
    public bool isVoorExamendossier => examendossierResultaat != null;
}

public class RelevanteCijferLichting
{
    public string UUID { get; set; }
}

public class SomtodayVakgemiddeldenModel
{
    public List<Gemiddelden> gemiddelden { get; set; }
    public double voortgangsdossierGemiddelde { get; set; }
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
    public RelevanteCijferLichting relevanteCijferLichting { get; set; }
}

public class VoortgangsdossierResultaat
{
    public double cijfer { get; set; }
    public bool isVoldoende { get; set; }
    public int periode { get; set; }
    public string formattedResultaat { get; set; }
    public string type { get; set; }
    public string toetscode { get; set; }
    public bool isLabel { get; set; }
    public bool isCijfer { get; set; }
}

public class ExamendossierResultaat
{
    public double cijfer { get; set; }
    public bool isVoldoende { get; set; }
    public bool isVoldoendeEerstePoging { get; set; }
    public int periode { get; set; }
    public string formattedResultaat { get; set; }
    public string formattedEerstePoging { get; set; }
    public string type { get; set; }
    public string toetscode { get; set; }
    public DateTime datumInvoerEerstePoging { get; set; }
    public bool isLabel { get; set; }
    public bool isCijfer { get; set; }
}