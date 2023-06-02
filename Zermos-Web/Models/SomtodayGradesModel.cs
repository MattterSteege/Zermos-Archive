using System;
using System.Collections.Generic;

namespace Zermos_Web.Models
{
    public class SomtodayGradesModel
    {
        public int laatsteWijziging { get; set; }
        public List<Item> items { get; set; }
    }

    public class Item
    {
        public List<Link> links { get; set; }
        public List<Permission> permissions { get; set; }
        public string herkansingstype { get; set; }
        public DateTime datumInvoer { get; set; }
        public bool teltNietmee { get; set; }
        public bool toetsNietGemaakt { get; set; }
        public int leerjaar { get; set; }
        public int periode { get; set; }
        public int weging { get; set; }
        public int examenWeging { get; set; }
        public bool isExamendossierResultaat { get; set; }
        public bool isVoortgangsdossierResultaat { get; set; }
        public string type { get; set; }
        public string omschrijving { get; set; }
        public Vak vak { get; set; }
        public int volgnummer { get; set; }
        public bool vrijstelling { get; set; }
        public string resultaat { get; set; }
        public string geldendResultaat { get; set; }
    }

    public class Link
    {
        public object id { get; set; }
        public string rel { get; set; }
        public string type { get; set; }
        public string href { get; set; }
    }

    public class Permission
    {
        public string full { get; set; }
        public string type { get; set; }
        public List<string> operations { get; set; }
        public List<string> instances { get; set; }
    }


    public class Vak
    {
        public List<Link> links { get; set; }
        public List<Permission> permissions { get; set; }
        public string afkorting { get; set; }
        public string naam { get; set; }
    }
}