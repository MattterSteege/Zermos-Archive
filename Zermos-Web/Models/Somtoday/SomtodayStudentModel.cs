using System.Collections.Generic;

namespace Zermos_Web.Models
{
    public class SomtodayStudentModel
    {
        public List<Item> items { get; set; }
    }


    public class AdditionalObjects
    {
        public Pasfoto pasfoto { get; set; }
    }

    public class Item
    {
        public List<Link> links { get; set; }
        public List<Permission> permissions { get; set; }
        public AdditionalObjects additionalObjects { get; set; }
        public int leerlingnummer { get; set; }
        public string roepnaam { get; set; }
        public string achternaam { get; set; }
        public string email { get; set; }
        public string mobielNummer { get; set; }
        public string geboortedatum { get; set; }
        public string geslacht { get; set; }
    }

    public class Link
    {
        public int id { get; set; }
        public string rel { get; set; }
        public string type { get; set; }
        public string href { get; set; }
    }

    public class Pasfoto
    {
        public List<Link> links { get; set; }
        public List<object> permissions { get; set; }
        public AdditionalObjects additionalObjects { get; set; }
        public string datauri { get; set; }
    }

    public class Permission
    {
        public string full { get; set; }
        public string type { get; set; }
        public List<string> operations { get; set; }
        public List<string> instances { get; set; }
    }
}