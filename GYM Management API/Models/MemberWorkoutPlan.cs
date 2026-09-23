using System.ComponentModel.DataAnnotations;
namespace GYM_Management_API.Models
{
    public class MemberWorkoutPlan
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public Member Member { get; set; }
        public int WorkoutPlanId { get; set; }
        public WorkoutPlan WorkoutPlan { get; set; }
        public DateTime AssignedDate { get; set; }
        public string Notes { get; set; }
    }
}
