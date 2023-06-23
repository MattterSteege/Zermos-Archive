// ReSharper disable InconsistentNaming

using System;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Infrastructure.Entities
{
    public partial class user
    {
        [Key]
        public string email { get; set; }
        public string name { get; set; }
        public string school_id { get; set; }
        public string school_naam_code { get; set; }
        public string zermelo_access_token { get; set; }
        public string somtoday_access_token { get; set; }
        public string somtoday_refresh_token { get; set; }
        public string somtoday_student_id { get; set; }
        public string somtoday_student_profile_picture { get; set; }
        public string infowijs_access_token { get; set; }
        public string infowijs_session_token { get; set; }
        
        //auth related
        public string? VerificationToken { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public bool IsVerified => VerifiedAt.HasValue;
        
        //API related
        // public string? ApiKey { get; set; }
        // public DateTime? ApiKeyExpiresAt { get; set; }
        // public bool ApiKeyIsExpired => ApiKeyExpiresAt.HasValue && ApiKeyExpiresAt <= DateTime.UtcNow;
        
    }
}
