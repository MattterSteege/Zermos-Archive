using System.Collections.Generic;
using Newtonsoft.Json;

namespace Zermos_Web.Models.SomtodayLeermiddelen;

public class Item
{
    public Product product { get; set; }
    public string URL {get => product.url;}
    public string Title {get => product.title;}
    public string Methode => product.methodeInformatie?.methode;
    public string Uitgever => product.methodeInformatie?.uitgever;
    public string DashboardMethodeNaam => product.methodeInformatie?.dashboardMethodeNaam;
    public bool isCustom { get; set; }
}

public class MethodeInformatie
{
    public string dashboardMethodeNaam { get; set; }
    public string methode { get; set; }
    public string uitgever { get; set; }
}

public class Product
{
    public string title { get; set; }
    public string url { get; set; }
    public MethodeInformatie methodeInformatie { get; set; }
}

public class SomtodayLeermiddelenModel
{
    public List<Item> items { get; set; }
}

