using System;
using System.Collections.Generic;

namespace Zermos_Web.Models.zermelo
{
    public class ZermeloRoosterModel
    {
        public Response response { get; set; }
        public List<Appointment> appointments
        {
            get => response.data[0].appointments;
            set => response.data[0].appointments = value;
        }
        public DateTime MondayOfAppointmentsWeek
        {
            get => response.data[0].MondayOfAppointmentsWeek;
            set => response.data[0].MondayOfAppointmentsWeek = value;
        }
        public List<int> timeStamps { get; set; }
        public string roosterOrigin { get; set; }
    }

    public class Appointment
    {
        public int start { get; set; }
        public int end { get; set; }
        public bool cancelled { get; set; }
        public string appointmentType { get; set; }
        public List<string> subjects { get; set; }
        public List<string> locations { get; set; }
        public List<string> teachers { get; set; }
        public List<Action> actions { get; set; }
        
    }
    
    public class Action
    {
        public Appointment appointment { get; set; }
        public bool allowed { get; set; }
        public string post { get; set; }
    }

    public class Items
    {
        public DateTime MondayOfAppointmentsWeek { get; set; }
        public List<Appointment> appointments { get; set; }
    }

    public class Response
    {
        public List<Items> data { get; set; }
    }
}