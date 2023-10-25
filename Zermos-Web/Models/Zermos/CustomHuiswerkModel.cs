using System;

namespace Zermos_Web.Models
{
    public class CustomHuiswerkModel
    {
        public string titel { get; set; }
        public string omschrijving { get; set; }
        public DateTime deadline { get; set; }
        public bool gemaakt { get; set; }
        public int id { get; set; }
    }
}