using System;

namespace Zermos_Web.Models
{
    public class CustomHuiswerkModel
    {
        public string titel;
        public string omschrijving;
        public DateTime deadline;
        public bool gemaakt;
        public int id;
        
        public CustomHuiswerkModel(string titel, string omschrijving, DateTime deadline, bool gemaakt, int id = -1)
        {
            this.titel = titel;
            this.omschrijving = omschrijving;
            this.deadline = deadline;
            this.gemaakt = gemaakt;
            this.id = id;
        }
    }
}