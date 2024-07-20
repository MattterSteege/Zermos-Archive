#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Zermos_Web.Models.SomtodayGradesModel
{
    public class AdditionalObjects
    {
        public string resultaatkolom { get; set; }
        public string vaknaam { get; set; }
        public string lichtinguuid { get; set; }
        public string vakuuid { get; set; }
        //public string? naamalternatiefniveau { get; set; } //voortgang only
    }

    public class Item
    {
        public string resultaatType => links[0].type?.Replace("resultaten.RGeldend", "").Replace("Resultaat", "") ?? "onbekend";
        public bool isVoortgang => resultaatType?.Contains("Voortgang") ?? false;
        public bool isSE => resultaatType?.Contains("Examen") ?? false;
        public List<Link> links { get; set; }
        //public List<Permission> permissions { get; set; }
        public AdditionalObjects additionalObjects { get; set; }
        public double cijfer { get; set; }
        public bool isVoldoende { get; set; }
        //public bool isVoldoendeEerstePoging { get; set; }
        public int periode { get; set; }
        public string formattedResultaat { get; set; }
        //public string formattedEerstePoging { get; set; }
        //public int volgnummer { get; set; }
        public string type { get; set; }
        public string toetscode { get; set; }
        public string omschrijving { get; set; }
        public int weging { get; set; }
        public DateTime datumInvoerEerstePoging { get; set; }
        public bool isLabel { get; set; }
        public bool isCijfer { get; set; }
        public string herkansing { get; set; }
        public string toetssoort { get; set; }
        //public string? label { get; set; } //voortgang only
        //public string? labelAfkorting { get; set; } //voortgang only
        public string? opmerkingen { get; set; } //voortgang only
        //public string? opmerkingenEerstePoging { get; set; } //voortgang only
    }

    public class SomtodayGradesModel
    {
        public List<Item> items { get; set; }
    }
    
    public class Link
    {
        public string type { get; set; }
    }
}

namespace Zermos_Web.Models.SortedSomtodayGradesModel
{
    public class Item
    {
        public string vakAfkorting { get; set; }
        public int weging { get; set; }
        public string cijfer { get; set; }
        public List<SomtodayGradesModel.Item> cijfers { get; set; }
        public int wegingSE { get; set; }
        public string cijferSE { get; set; }
        public List<SomtodayGradesModel.Item> cijfersSE { get; set; }
        public string vakNaam { get; set; }
        public string vakuuid { get; set; }
        public bool isLabel { get; set; }
    }

    public class SortedSomtodayGradesModel
    {
        public List<Item> items { get; set; }
        public List<SomtodayGradesModel.Item> lastGrades { get; set; }
        public double? voortGangsdossierGemiddelde { get; set; }
        public string leerjaarUUID { get; set; }
        public string relevanteCijferLichtingUUID { get; set; }
        public bool onlyVoorVoortgang { get; set; }
    }
}