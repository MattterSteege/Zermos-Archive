using System.Collections.Generic;

namespace Zermos_Web.Models
{
    public class ZermeloRoosterModel
    {
        public Response response { get; set; }
    }

    public class Action
    {
        public Appointment appointment { get; set; }
        public List<Status> status { get; set; }
        public bool allowed { get; set; }
        public string post { get; set; }
    }

    public class Appointment
    {
        public List<Status> status { get; set; }
        public List<Action> actions { get; set; }
        public int start { get; set; }
        public int end { get; set; }
        public bool cancelled { get; set; }
        public string appointmentType { get; set; }
        public bool online { get; set; }
        public bool optional { get; set; }
        public int? appointmentInstance { get; set; }
        public string startTimeSlotName { get; set; }
        public string endTimeSlotName { get; set; }
        public List<string> subjects { get; set; }
        public List<string> groups { get; set; }
        public List<string> locations { get; set; }
        public List<string> teachers { get; set; }
        public List<object> onlineTeachers { get; set; }
        public object onlineLocationUrl { get; set; }
        public object capacity { get; set; }
        public object expectedStudentCount { get; set; }
        public object expectedStudentCountOnline { get; set; }
        public string changeDescription { get; set; }
        public string schedulerRemark { get; set; }
        public object content { get; set; }
        public int? id { get; set; }
    }

    public class Items
    {
        public int laatsteWijziging { get; set; }
        public List<Appointment> appointments { get; set; }
    }

    public class Response
    {
        public int status { get; set; }
        public string message { get; set; }
        public string details { get; set; }
        public int eventId { get; set; }
        public int startRow { get; set; }
        public int endRow { get; set; }
        public int totalRows { get; set; }
        public List<Items> data { get; set; }
    }

    public class Status
    {
        public int code { get; set; }
        public string nl { get; set; }
        public string en { get; set; }
    }
}