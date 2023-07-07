using System;

namespace Zermos_Web.Models
{
    public class customHuiswerkModel
    {
        public string titel;
        public string omschrijving;
        public DateTime deadline;
        public bool gemaakt;
        public int id;
        
        public customHuiswerkModel(string titel, string omschrijving, DateTime deadline, bool gemaakt, int id = -1)
        {
            this.titel = titel;
            this.omschrijving = omschrijving;
            this.deadline = deadline;
            this.gemaakt = gemaakt;
            this.id = id;
        }
    }
}