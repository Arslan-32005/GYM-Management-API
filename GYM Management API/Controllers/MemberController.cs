using GYM_Management_API.Data;
using GYM_Management_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace GYM_Management_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly GymContext _context;
        public MemberController(GymContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult> GetAllMembers(string? search, bool? IsActive, int? TrainerId, string? sortByDate, int page = 1, int pageSize = 5)
        {
            if (page < 1 || pageSize < 1)
            {
                return BadRequest("Page and pageSize must be greater than 0.");
            }

            IQueryable<Member> query = _context.Members;
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(m => m.FullName.ToLower().Contains(search.ToLower()));

            }
            if (TrainerId.HasValue)
            {
                query = query.Where(m => m.TrainerId == TrainerId.Value);
            }
            if(IsActive.HasValue)
            {
                query = query.Where(m => m.IsActive == IsActive.Value);
            }
            if (sortByDate == "asc")
            {
                query = query.OrderBy(m => m.JoinDate);
            }
            else if (sortByDate == "desc")
            {
                query = query.OrderByDescending(m => m.JoinDate);
            }
            var total = await query.CountAsync();
           var items= await query.Skip((page - 1) * pageSize).Take(pageSize).Select(m => new{
           m.Id,
           m.FullName,
           m.Age,
           m.Gender,
           m.PhoneNumber,
           m.Email,
           m.JoinDate,
           m.IsActive,
           Trainer= new
           {
               m.Trainer.Id,
               m.Trainer.FullName

           }
           }).ToListAsync();
            var totalPages = (int)Math.Ceiling((double)total / pageSize);

            return Ok(new
            {
                items,
                page,
                pageSize,
                totalItems = total,
                totalPages
            });

        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetMember(int id)
        {
            var member = await _context.Members.Include(m => m.Trainer).Where(m => m.Id == id).Select(m => new
            {
                m.Id,
                m.FullName,
                m.Age,
                m.Gender,
                m.PhoneNumber,
                m.Email,
                m.JoinDate,
                m.IsActive,
                Trainer = new
                {
                    m.Trainer.Id,
                    m.Trainer.FullName
                },
                Memberships = m.Memberships.Select(ms => new
                {
                    ms.Id,
                    ms.PlanName,
                    ms.Fee,
                    ms.PaymentStatus
                }).ToList(),
                WorkoutPlan = m.MemberWorkoutPlans.Select(mw => new
                {
                    mw.Id,
                    mw.AssignedDate,
                    mw.Notes,
                    PlanName = mw.WorkoutPlan.Name,
                }).ToList()
            }).FirstOrDefaultAsync();
            if (member == null)
            {
                return NotFound();
            }
            return Ok(member);
        }
        [HttpPost]
        public async Task<ActionResult> CreateMember(Member member)
        {
            if(member.TrainerId.HasValue)
            {
                var trainer = await _context.Trainers.FindAsync(member.TrainerId.Value);
                if (trainer == null)
                {
                    return BadRequest("Trainer does not exist.");
                }
                if(!trainer.IsAvailable)
                {
                    return BadRequest("Trainer is not available.");
                }
            }
            _context.Members.Add(member);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMember), new { id = member.Id }, member);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateMember(int id, Member updatedMember)
        {
            if (id != updatedMember.Id)
            {
                return BadRequest();
            }
            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            if(updatedMember.TrainerId.HasValue)
            {
                var trainer = await _context.Trainers.FindAsync(updatedMember.TrainerId.Value);
                if (trainer == null)
                {
                    return BadRequest("Trainer does not exist.");
                }
                if (!trainer.IsAvailable)
                {
                    return BadRequest("Trainer is not available.");
                }
            }
            member.FullName = updatedMember.FullName;
            member.Age = updatedMember.Age;
            member.Gender = updatedMember.Gender;
            member.PhoneNumber = updatedMember.PhoneNumber;
            member.Email = updatedMember.Email;
            member.JoinDate = updatedMember.JoinDate;
            member.IsActive = updatedMember.IsActive;
            member.TrainerId = updatedMember.TrainerId;
            await _context.SaveChangesAsync();
            return Ok(member);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMember(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            _context.Members.Remove(member);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
