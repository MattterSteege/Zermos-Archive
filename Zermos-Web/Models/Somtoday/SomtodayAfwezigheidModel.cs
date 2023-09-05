using System;
using System.Collections.Generic;

namespace Zermos_Web.Models.SomtodayAfwezigheidModel
{

    public class SomtodayAfwezigheidModel
    {
        public List<Item> items { get; set; }
    }

    public class AbsentieReden
    {
        public string absentieSoort { get; set; }
        public string afkorting { get; set; }
        public string omschrijving { get; set; }
        public bool geoorloofd { get; set; }
    }

    public class Item
    {
        public AbsentieReden absentieReden { get; set; }
        public DateTime beginDatumTijd { get; set; }
        public DateTime eindDatumTijd { get; set; }
        public bool afgehandeld { get; set; }
        public string opmerkingen { get; set; }
    }
}

