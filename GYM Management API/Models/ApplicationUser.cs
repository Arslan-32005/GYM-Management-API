using Microsoft.AspNetCore.Identity;
namespace GYM_Management_API.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string FullName { get; set; }
        public DateTime JoinDate { get; set; }
    }
}
