using System.Collections.Generic;

namespace Zermos_Web.Models.Somtoday;

public class SomtodayLeermiddelenBijlageModel
{
    public List<Item> items { get; set; }
}

public class Item
{
    public string afkorting { get; set; }
    public string naam { get; set; }
    public string UUID { get; set; }
}