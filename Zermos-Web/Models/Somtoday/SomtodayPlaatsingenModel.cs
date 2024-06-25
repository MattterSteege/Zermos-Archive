using System.Collections.Generic;

namespace Zermos_Web.Models.SomtodayPlaatsingen;

public class Item
{
    public string UUID { get; set; }
    public string vanafDatum { get; set; }
    public string totDatum { get; set; }
    public bool huidig { get; set; }
    public string stamgroepnaam { get; set; }
    public string opleidingsnaam { get; set; }
    public int leerjaar { get; set; }
}

public class SomtodayPlaatsingenModel
{
    public List<Item> items { get; set; }
}