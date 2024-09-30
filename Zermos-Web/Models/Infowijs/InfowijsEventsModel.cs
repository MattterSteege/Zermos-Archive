using System.Collections.Generic;
using System.Globalization;

namespace Zermos_Web.Models
{
    public class InfowijsEventsModel
    {
        public List<Datum> data { get; set; }
    }
    public class Datum
    {
        public string title { get; set; }
        public int startsAt { get; set; }
        public int endsAt { get; set; }
        public bool isAllDay { get; set; }

        
        //turn the unix timestamp into a readable date using the code above
        public string TimeFormat { get {
            var culture = new CultureInfo("nl-NL");
            var startsAt = this.startsAt.ConvertUnixTimestampToDutchTime();
            var endsAt = this.endsAt.ConvertUnixTimestampToDutchTime();
            var itemDate = startsAt.Date;
            if (itemDate == endsAt.Date && !isAllDay) return startsAt.ToString("dddd d MMMM HH:mm", culture) + " - " + endsAt.ToString("HH:mm", culture);
            if (itemDate != endsAt.Date && !isAllDay) return startsAt.ToString("dddd d MMMM", culture) + " t/m " + endsAt.AddDays(-1).ToString("dddd d MMMM", culture);
            if (isAllDay) return startsAt.ToString("dddd d MMMM", culture);
            return "";
        } }
    }
}