using System.ComponentModel.DataAnnotations;
namespace GYM_Management_API.Models
{
    public class Trainer
    {
      public int Id { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Specialization { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public bool IsAvailable { get; set; }
        public ICollection<Member> Members { get; set; }
    }
}
