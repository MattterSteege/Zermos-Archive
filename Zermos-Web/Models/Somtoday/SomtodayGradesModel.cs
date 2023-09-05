using System;
using System.Collections.Generic;

namespace Zermos_Web.Models.SomtodayGradesModel
{
    public class SomtodayGradesModel
    {
        //public int laatsteWijziging { get; set; }
        public List<Item> items { get; set; }
    }

    public class Item
    {
        public List<Link> links { get; set; }
        //public List<Permission> permissions { get; set; }
        //public AdditionalObjects additionalObjects { get; set; }
        //public string herkansingstype { get; set; }
        //public string resultaat { get; set; }
        public string geldendResultaat { get; set; }

        public DateTime datumInvoer { get; set; }

        //public bool teltNietmee { get; set; }
        //public bool toetsNietGemaakt { get; set; }
        //public int leerjaar { get; set; }

        //public long periode { get; set; }
        public int weging { get; set; }

        public int examenWeging { get; set; }

        //public bool isExamendossierResultaat { get; set; }
        //public bool isVoortgangsdossierResultaat { get; set; }
        public string type { get; set; }
        public string omschrijving { get; set; }

        public Vak vak { get; set; }

        //public Leerling leerling { get; set; }
        //public int volgnummer { get; set; }
        //public bool vrijstelling { get; set; }
        //public string resultaatLabel { get; set; }
        public string resultaatLabelAfkorting { get; set; }
    }

    // public class Leerling
    // {
    //     public List<Link> links { get; set; }
    //     public List<Permission> permissions { get; set; }
    //     public AdditionalObjects additionalObjects { get; set; }
    //     public string UUID { get; set; }
    //     public int leerlingnummer { get; set; }
    //     public string roepnaam { get; set; }
    //     public string voorvoegsel { get; set; }
    //     public string achternaam { get; set; }
    // }

    public class Link
    {
        public string id { get; set; }
        //     public string rel { get; set; }
        //     public string type { get; set; }
        //     public string href { get; set; }
    }

    public class Vak
    {
        //public List<Link> links { get; set; }
        //public List<Permission> permissions { get; set; }
        //public AdditionalObjects additionalObjects { get; set; }
        public string afkorting { get; set; }
        public string naam { get; set; }
    }

    public class sortedGrades
    {
        public List<Item> grades;
        public Vak vak;
    }
}