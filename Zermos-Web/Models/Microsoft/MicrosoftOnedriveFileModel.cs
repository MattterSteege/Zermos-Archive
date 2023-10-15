using System.Collections.Generic;
using Newtonsoft.Json;

namespace Zermos_Web.Models
{

    public class Folder
    {
        public int childCount { get; set; }
    }

    public class MicrosoftOnedriveFileModel
    {
        public List<Value> value { get; set; }
    }

    public class Value
    {
        public string id { get; set; }
        public string name { get; set; }
        public string webUrl { get; set; }
        public int size { get; set; }
        public Folder folder { get; set; }
    }
}