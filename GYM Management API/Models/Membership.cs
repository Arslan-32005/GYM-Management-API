using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace GYM_Management_API.Models
{
    public class Membership
    {
        public int Id { get; set; }
        [Required]
        public string PlanName { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        [Compare("StartDate", ErrorMessage = "End date must be before than start date.")]
        public DateTime EndDate { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Fee must be a positive value.")]
        public int Fee { get; set; }
        [Required]
        public string PaymentStatus { get; set; }
        [Required]
        public int MemberId { get; set; }
        public Member Member { get; set; }

    }
}
