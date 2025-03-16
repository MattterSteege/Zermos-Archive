using System;
using System.Collections.Generic;

namespace Zermos_Web.Models.Somtoday;

public class AssemblyResult
{
    public string fileExtension { get; set; }
    public int fileSize { get; set; }
    public string fileUrl { get; set; }
    public string fileName { get; set; }
}

public class JaarBijlagen
{
    public string omschrijving { get; set; }
    public List<AssemblyResult> assemblyResults { get; set; }
    //public int sortering { get; set; }
    //public bool zichtbaarVoorLeerling { get; set; }
}

public class SomtodayLeermiddelenBijlageLinkModel
{
    //public List<object> lesstof { get; set; }
    public List<JaarBijlagen> jaarBijlagen { get; set; }
    public string vak { get; set; }
    public string UUID { get; set; }
}

// public class Lesstof
// {
//     public StudiewijzerItem studiewijzerItem { get; set; }
//     public int sortering { get; set; }
//     public DateTime datumTijd { get; set; }
//     public DateTime aangemaaktOpDatumTijd { get; set; }
// }

// public class StudiewijzerItem
// {
//     public string onderwerp { get; set; }
//     public string huiswerkType { get; set; }
//     public string omschrijving { get; set; }
//     public bool inleverperiodes { get; set; }
//     public bool lesmateriaal { get; set; }
//     public bool projectgroepen { get; set; }
//     public List<object> bijlagen { get; set; }
//     public List<object> externeMaterialen { get; set; }
//     public List<object> inlevermomenten { get; set; }
//     public bool tonen { get; set; }
//     public bool notitieZichtbaarVoorLeerling { get; set; }
//     public string leerdoelen { get; set; }
// }