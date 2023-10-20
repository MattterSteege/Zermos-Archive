using System.Collections.Generic;

namespace Zermos_Web.Models.Zermos
{
    public class NotitiesModel
    {
        public List<NotitieBoek> NotitieBoeken { get; set; }
    }

    public class NotitieBoek
    {
        public string Id { get; set; }
        public int ranking { get; set; }
        public string Naam { get; set; }
        public List<Notitie> Notities { get; set; }
    }

    public class Notitie
    {
        public string Id { get; set; }
        public int ranking { get; set; }
        public string Titel { get; set; }
        public string Omschrijving { get; set; }
        public List<Paragraph> Paragraphs { get; set; }
        public List<string> Tags { get; set; }
    }

    public class Paragraph
    {
        public string Id { get; set; }
        public int ranking { get; set; }
        public string Naam { get; set; }
        public string Inhoud { get; set; }
    }
}