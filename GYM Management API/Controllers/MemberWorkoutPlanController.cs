using GYM_Management_API.Data;
using GYM_Management_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace GYM_Management_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberWorkoutPlanController : ControllerBase
    {
        private readonly GymContext _context;
        public MemberWorkoutPlanController(GymContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult> GetMemberWorkoutPlans(int page = 1, int pageSize = 5)
        {
            IQueryable<MemberWorkoutPlan> memberWorkoutPlans = _context.MemberWorkoutPlans;
            var totalCount = await memberWorkoutPlans.CountAsync();
            var items = await memberWorkoutPlans.Skip((page - 1) * pageSize).Take(pageSize).Select(mwp => new
            {
                mwp.Id,
                mwp.AssignedDate,
                mwp.Notes,
                Member= new
                {
                    mwp.Member.Id,
                    mwp.Member.FullName,
                },
                WorkoutPlan = new
                {
                    mwp.WorkoutPlan.Id,
                    mwp.WorkoutPlan.Name,
                    mwp.WorkoutPlan.Goal,
                    mwp.WorkoutPlan.DifficultyLevel,   
                }
            }).ToListAsync();
            return Ok(items);

        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetMemberWorkoutPlan(int id)
        {
            var memberWorkoutPlan = await _context.MemberWorkoutPlans
                .Where(mwp => mwp.Id == id)
                .Select(mwp => new
                {
                    mwp.Id,
                    mwp.AssignedDate,
                    mwp.Notes,
                    Member = new
                    {
                        mwp.Member.Id,
                        mwp.Member.FullName,
                        mwp.Member.Email
                    },
                    WorkoutPlan = new
                    {
                        mwp.WorkoutPlan.Id,
                        mwp.WorkoutPlan.Name,
                        mwp.WorkoutPlan.Goal,
                        mwp.WorkoutPlan.DifficultyLevel
                    }
                }).FirstOrDefaultAsync();
            if (memberWorkoutPlan == null)
            {
                return NotFound();
            }
            return Ok(memberWorkoutPlan);
        }
        [HttpPost]
        public async Task<ActionResult> CreateMemberWorkoutPlan(MemberWorkoutPlan memberWorkoutPlan)
        {
            var member = await _context.Members.FindAsync(memberWorkoutPlan.MemberId);
            if(member == null)
            {
                return BadRequest("Invalid MemberId");
            }
            if(!member.IsActive)
            {
                return BadRequest("Member is not active");
            }
            var Plan = await _context.WorkoutPlans.FindAsync(memberWorkoutPlan.WorkoutPlanId);
            if(Plan == null)
            {
                return BadRequest("Invalid WorkoutPlanId");
            }
            var alreadyassigned= await _context.MemberWorkoutPlans.AnyAsync(mwp => mwp.MemberId == memberWorkoutPlan.MemberId && mwp.WorkoutPlanId == memberWorkoutPlan.WorkoutPlanId);
            if(alreadyassigned)
            {
                return BadRequest("Workout plan already assigned to this member");
            }
            _context.MemberWorkoutPlans.Add(memberWorkoutPlan);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMemberWorkoutPlan), new { id = memberWorkoutPlan.Id }, memberWorkoutPlan);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateMemberWorkoutPlan(int id, MemberWorkoutPlan updatedMemberWorkoutPlan)
        {
            if (id != updatedMemberWorkoutPlan.Id)
            {
                return BadRequest();
            }
            var memberWorkoutPlan = await _context.MemberWorkoutPlans.FindAsync(id);
            if (memberWorkoutPlan == null)
            {
                return NotFound();
            }
            memberWorkoutPlan.AssignedDate = updatedMemberWorkoutPlan.AssignedDate;
            memberWorkoutPlan.Notes = updatedMemberWorkoutPlan.Notes;
            await _context.SaveChangesAsync();
            return Ok(memberWorkoutPlan);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMemberWorkoutPlan(int id)
        {
            var memberWorkoutPlan = await _context.MemberWorkoutPlans.FindAsync(id);
            if (memberWorkoutPlan == null)
            {
                return NotFound();
            }
            _context.MemberWorkoutPlans.Remove(memberWorkoutPlan);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        

    }
}
