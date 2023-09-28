namespace Zermos_Web.Models.PulseCore;

public class SchoolklimaatModel
{
    public string uuid { get; set; }
    public string temperature { get; set; }
    public int temperatureScore { get; set; }
    public string humidity { get; set; }
    public int humidityScore { get; set; }
    public string airQuality { get; set; }
    public int airQualityScore { get; set; }
    public bool isOnline { get; set; }
}