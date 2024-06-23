using System;
using System.Collections.Generic;

namespace Zermos_Web.Models.Zermelo;

public class SimpleZermeloRoosterModel
{
    public Response response { get; set; }
    public List<Appointment> appointments
    {
        get => response.data;
        set => response.data = value;
    }
}

public class Appointment
{
    public bool cancelled { get; set; }
    public string type { get; set; }
    public List<string> subjects { get; set; }
    public List<string> locations { get; set; }
    public List<string> teachers { get; set; }
    public List<string> groups { get; set; }
    public int start { get; set; }
    public int end { get; set; }
    
}

public class Response
{
    public List<Appointment> data { get; set; }
    public string totalRows { get; set; }
}