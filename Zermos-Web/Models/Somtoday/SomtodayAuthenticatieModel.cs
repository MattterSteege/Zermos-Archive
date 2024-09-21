using System.Collections.Generic;

namespace Zermos_Web.Models
{
    public class SomtodayAuthenticatieModel
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        // public string somtoday_api_url { get; set; }
        // public string somtoday_oop_url { get; set; }
        // public string scope { get; set; }
        // public string somtoday_organisatie_afkorting { get; set; }
        // public string id_token { get; set; }
        // public string token_type { get; set; }
        // public int expires_in { get; set; }
    }
    
    public class SomtodayCFAuthenticatieModel
    {
        public string location { get; set; }
        //is the auth? param from the location url
        //public string auth => location.Substring(35);
        public List<string> cookies { get; set; }
    }
}