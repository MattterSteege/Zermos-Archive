using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Zermos_Web.Models.Somtoday;

public class Item
{
    [JsonProperty("$type")]
    public string type { get; set; }
    public string uniqueIdentifier { get; set; }
    public string afspraakItemType { get; set; }
    public string locatie { get; set; }
    public DateTime beginDatumTijd { get; set; }
    public DateTime eindDatumTijd { get; set; }
    public int beginLesuur { get; set; }
    public int eindLesuur { get; set; }
    public string titel { get; set; }
    public bool onlineDeelname { get; set; }
    public string omschrijving { get; set; }
    public Vak vak { get; set; }
    public List<object> bijlagen { get; set; }
    public List<object> lesgroepen { get; set; }
    public List<string> docentNamen { get; set; }
}

public class SomtodayafspraakitemsModel
{
    public List<int> timeStamps;
    public List<Item> items { get; set; }
    public DateTime MondayOfAppointmentsWeek { get; set; }
}

public class Vak
{
    [JsonProperty("$type")]
    public string type { get; set; }
    public int id { get; set; }
    public string naam { get; set; }
    public string afkorting { get; set; }
    public string UUID { get; set; }
}

