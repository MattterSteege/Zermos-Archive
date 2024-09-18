using System;
using System.Collections.Generic;

namespace Zermos_Web.Models
{
    public class Link
    {
        public Int64 id { get; set; }
    }

    public class Persoon
    {
        public List<Link> links { get; set; }
        public string UUID { get; set; }
        public int leerlingnummer { get; set; }
        public string roepnaam { get; set; }
        public string voorvoegsel { get; set; }
        public string achternaam { get; set; }
        public string email { get; set; }
        public string geboortedatum { get; set; }
        public string geslacht { get; set; }
    }

    public class SomtodayStudentModel
    {
        public string gebruikersnaam { get; set; }
        public List<object> accountPermissions { get; set; }
        public Persoon persoon { get; set; }
        public Int64 id => persoon.links[0].id;
        public string naam => persoon.roepnaam + " " + persoon.voorvoegsel + " " + persoon.achternaam;
    }

}