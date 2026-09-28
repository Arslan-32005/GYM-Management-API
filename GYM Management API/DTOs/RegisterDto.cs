using System.ComponentModel.DataAnnotations;

namespace GYM_Management_API.DTOs
{
    public class RegisterDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string FullName
        {
            get; set;
        }
    }
}
