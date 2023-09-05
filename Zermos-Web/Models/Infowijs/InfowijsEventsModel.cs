using System.Collections.Generic;

namespace Zermos_Web.Models
{
    public class InfowijsEventsModel
    {
        public List<Datum> data { get; set; }
    }
    public class Datum
    {
        public string id { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public int startsAt { get; set; }
        public int endsAt { get; set; }
        public bool isAllDay { get; set; }
        public bool isSubscribed { get; set; }
        public string group { get; set; }
        public int createdAt { get; set; }
        public int updatedAt { get; set; }
    }
}