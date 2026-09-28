using System.ComponentModel.DataAnnotations;

namespace GYM_Management_API.DTOs
{
    public class LoginDto
    {
        [Required] 
        public string Email { get; set; }
        [Required] 
        public string Password { get; set; }
    }
}
