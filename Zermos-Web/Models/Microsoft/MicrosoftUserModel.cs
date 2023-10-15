using System.Collections.Generic;
using Newtonsoft.Json;

namespace Zermos_Web.Models;

public class MicrosoftUserModel
{
    [JsonProperty("@odata.context")] 
    public string odatacontext { get; set; }
    public List<object> businessPhones { get; set; }
    public string displayName { get; set; }
    public string givenName { get; set; }
    public object jobTitle { get; set; }
    public string mail { get; set; }
    public object mobilePhone { get; set; }
    public string officeLocation { get; set; }
    public object preferredLanguage { get; set; }
    public string surname { get; set; }
    public string userPrincipalName { get; set; }
    public string id { get; set; }
}