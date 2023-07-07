using System;
using System.Collections.Generic;

namespace Zermos_Web.Models
{
    public class NotitiesModel
    {
        public List<Notitie> notities { get; set; }
    }
    
    public class Notitie
    {
        public int id { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public DateTime date { get; set; }
        public NotitieType type { get; set; }
    }

    public enum NotitieType
    {
        MindMap,
        Text,
        Image
    }
}