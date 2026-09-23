using System.ComponentModel.DataAnnotations;
namespace GYM_Management_API.Models
{
    public class WorkoutPlan
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Goal { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be a Greater than zero.")]
        public int DurationInWeeks { get; set; }
        [Required]
        public string DifficultyLevel { get; set; }
        public ICollection<MemberWorkoutPlan> MemberWorkoutPlans { get; set; }

    }
}
