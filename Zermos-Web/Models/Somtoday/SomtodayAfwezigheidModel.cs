using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Zermos_Web.Models.SomtodayAfwezigheidModel
{
    public class SomtodayAfwezigheidModel
    {
        public List<Item> items { get; set; }
    }

    public class AbsentieReden
    {
        public string omschrijving { get; set; }
    }

    public class Item
    {
        public AbsentieReden absentieReden { get; set; }
        public string omschrijving => absentieReden?.omschrijving;
        public DateTime beginDatumTijd { get; set; }
        public DateTime eindDatumTijd { get; set; }
        public string opmerkingen { get; set; }
    }
}

