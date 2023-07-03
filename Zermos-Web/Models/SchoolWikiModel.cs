using System.Collections.Generic;

namespace Zermos_Web.Models
{
    public class SchoolWikiModel
    {
        public List<Result> results { get; set; }
    }

    public class Hit
    {
        public string title { get; set; }
        public string type { get; set; }
        public string path { get; set; }
        //public string body { get; set; }
        //public int version { get; set; }
        public string schoolName { get; set; }
        //public string schoolColor { get; set; }
    }

    public class Result
    {
        public List<Hit> hits { get; set; }
        public string query { get; set; }
    }
}