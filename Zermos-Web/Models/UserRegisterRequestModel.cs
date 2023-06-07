using System.ComponentModel.DataAnnotations;

namespace Zermos_Web.Models
{
    public class UserRegisterRequestModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
    
    public class UserVerifyRequestModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string VerificationToken { get; set; } = string.Empty;
    }
}