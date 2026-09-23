using Microsoft.AspNetCore.Mvc;
using GYM_Management_API.Data;
using GYM_Management_API.Models;
using Microsoft.EntityFrameworkCore;

namespace GYM_Management_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkoutPlanController : ControllerBase
    {
        private readonly GymContext _context;
        public WorkoutPlanController(GymContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult>GetAllWorkoutPlans(string ? search,string? Goal, string? DifficultyLevel, int page = 1, int pageSize = 10)
        {
            IQueryable<WorkoutPlan> workoutPlans = _context.WorkoutPlans;
            if (!string.IsNullOrEmpty(search))
            {
                workoutPlans = workoutPlans.Where(wp => wp.Name.Contains(search));
            }
            if (!string.IsNullOrEmpty(Goal))
            {
                workoutPlans = workoutPlans.Where(wp => wp.Goal==Goal);
            }
            if(!string.IsNullOrEmpty(DifficultyLevel))
            {
                workoutPlans = workoutPlans.Where(wp => wp.DifficultyLevel==DifficultyLevel);
            }
            var total=await workoutPlans.CountAsync();
            var items = await workoutPlans.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return Ok(items);

        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetWorkoutPlan(int id)
        {
            
            var workoutPlan = await _context.WorkoutPlans
                .Where(wp => wp.Id == id)
                .Select(wp => new
                {
                    wp.Id,
                    wp.Name,
                    wp.DurationWeeks,
                    wp.Goal,
                    wp.DifficultyLevel,
                    assignments = wp.MemberWorkoutPlans.Select(mw => new
                    {
                        mw.Id,
                        mw.AssignedDate,
                        mw.Notes,
                        Member = new
                        {
                            mw.Member.Id,
                            mw.Member.FullName,
                            mw.Member.Email
                        }
                    }).ToList()
                }).FirstOrDefaultAsync();
            if (workoutPlan == null)
            {
                return NotFound();
            }
            return Ok();
        }
        [HttpPost]
        public async Task<ActionResult> CreateWorkoutPlan(WorkoutPlan workoutPlan)
        {

            _context.WorkoutPlans.Add(workoutPlan);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetWorkoutPlan), new { id = workoutPlan.Id }, workoutPlan);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateWorkoutPlan(int id, WorkoutPlan updatedWorkoutPlan)
        {
            if (id != updatedWorkoutPlan.Id)
            {
                return BadRequest();
            }
            var workoutPlan = await _context.WorkoutPlans.FindAsync(id);
            if (workoutPlan == null)
            {
                return NotFound();
            }
            workoutPlan.Name = updatedWorkoutPlan.Name;
            workoutPlan.DurationWeeks = updatedWorkoutPlan.DurationWeeks;
            workoutPlan.Goal = updatedWorkoutPlan.Goal;
            workoutPlan.DifficultyLevel = updatedWorkoutPlan.DifficultyLevel;
            await _context.SaveChangesAsync();
            return Ok(updatedWorkoutPlan);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteWorkoutPlan(int id)
        {
            var workoutPlan = await _context.WorkoutPlans.FindAsync(id);
            if (workoutPlan == null)
            {
                return NotFound();
            }
            bool hasAssignments = await _context.MemberWorkoutPlans.AnyAsync(mw => mw.WorkoutPlanId == id);
            if(hasAssignments)
            {
                return BadRequest("Cannot delete workout plan with assigned members.");
            }
            _context.WorkoutPlans.Remove(workoutPlan);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
