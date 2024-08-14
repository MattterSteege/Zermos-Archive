using System;

namespace Zermos_Web.Models.Somtoday;

public class SomtodayPreAuthenticationModel
{
    public string grant_type { get; set; }
    public string code { get; set; }
    public string redirect_uri { get; set; }
    public string code_verifier { get; set; }
    public string client_id { get; set; }
    public string claims { get; set; }
    
    //user send this to the server
    public string user { get; set; }
    public string vanityUrl { get; set; }
    public DateTime expires { get; set; }
    public string callbackUrl { get; set; }
}