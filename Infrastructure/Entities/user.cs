// ReSharper disable InconsistentNaming
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Infrastructure.Entities
{
    public partial class user
    {
        [Key]
        [StringLength(45)]
        public string uuid { get; set; }
        [StringLength(45)]
        public string name { get; set; }
        [StringLength(45)]
        public string school_id { get; set; }
        [StringLength(45)]
        public string school_naam_code { get; set; }
        [StringLength(45)]
        public string zermelo_access_token { get; set; }
        [StringLength(45)]
        public string somtoday_access_token { get; set; }
        [StringLength(45)]
        public string somtoday_student_id { get; set; }
        [StringLength(45)]
        public string infowijs_access_token { get; set; }
        [StringLength(45)]
        public string infowijs_session_token { get; set; }
    }
}
