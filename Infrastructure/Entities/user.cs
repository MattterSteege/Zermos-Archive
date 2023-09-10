// ReSharper disable InconsistentNaming

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Infrastructure.Entities
{
    public class user
    {
        [Key] public string email { get; set; }
        public string name { get; set; }
        public string school_id { get; set; }
        public string theme { get; set; }
        public string default_page { get; set; }
        public string custom_huiswerk { get; set; }
        
        //Token Related
        public string zermelo_access_token { get; set; }
        public DateTime? zermelo_access_token_expires_at { get; set; }
        public string somtoday_access_token { get; set; }
        public string somtoday_refresh_token { get; set; }
        public string somtoday_student_id { get; set; }
        public string infowijs_access_token { get; set; }
        public string teams_access_token { get; set; }
        public string teams_refresh_token { get; set; }

        //cache related
        public string cached_somtoday_grades { get; set; }
        public string cached_somtoday_homework { get; set; }
        public string cached_somtoday_absence { get; set; }
        public string cached_infowijs_calendar { get; set; }
        public string cached_infowijs_news { get; set; }
        public string cached_school_informationscreen { get; set; }
        public string cached_zermelo_schedule { get; set; }
    }
}