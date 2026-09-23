using System.ComponentModel.DataAnnotations;

namespace GYM_Management_API.Models
{
    public class Member
    {
      public int Id { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        [Range(15, 80, ErrorMessage = "Age must be between 18 and 80.")]
        public int Age { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public DateTime JoinDate { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public int? TrainerId { get; set; }
        public Trainer Trainer { get; set; }
        public ICollection<Membership> Memberships { get; set; }
        public ICollection<MemberWorkoutPlan> MemberWorkoutPlans { get; set; }


    }
}
