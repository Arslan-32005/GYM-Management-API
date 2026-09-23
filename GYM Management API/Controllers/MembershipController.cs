using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using GYM_Management_API.Data;
using GYM_Management_API.Models;

namespace GYM_Management_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembershipController : ControllerBase
    {
        private readonly GymContext _context;
        public MembershipController(GymContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult>GetAllMemberships(int ?memberId, string paymentStatus, int page = 1)
        {
            int pageSize = 5;
            IQueryable<Membership> memberships = _context.Memberships;
            if(memberId.HasValue)
            {
                memberships = memberships.Where(m => m.MemberId == memberId);
            }
            if (!string.IsNullOrEmpty(paymentStatus))
            {
                memberships = memberships.Where(m => m.PaymentStatus == paymentStatus);
            }
            var totalMemberships = await memberships.CountAsync();
            var membershipsList = await memberships
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new
                {
                    m.Id,
                    m.PlanName,
                    m.StartDate,
                    m.EndDate,
                    m.Fee,
                    m.PaymentStatus,
                    Member = new
                    {
                        m.Member.Id,
                        m.Member.FullName,
                        m.Member.Email
                    }
                }).ToListAsync();
            return Ok(membershipsList);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult>GetMembership(int id)
        {
                var membership=await _context.Memberships.Where(m => m.Id == id).Select(m => new
                {
                    m.Id,
                    m.PlanName,
                    m.StartDate,
                    m.EndDate,
                    m.Fee,
                    m.PaymentStatus,
                    Member = new
                    {
                        m.Member.Id,
                        m.Member.FullName,
                        m.Member.Email
                    }
                }).FirstOrDefaultAsync();
                if (membership == null)
                {
                    return NotFound();
                }
                return Ok(membership);
        }
        [HttpPost]
        public async Task<ActionResult> CreateMembership(Membership membership)
        {
            var member = await _context.Members.FindAsync(membership.MemberId);
            if(member == null)
            {
                return BadRequest("Member does not exist.");
            }
            if (!member.IsActive)
            {
                return BadRequest("Cannot create membership for inactive member.");
            }
            if(membership.EndDate < membership.StartDate)
            {
                return BadRequest("EndDate must be after StartDate.");
            }
            if (membership.Fee < 0)
            {
                return BadRequest("Fee must be a positive value.");
            }
            bool hasOverlappingMembership = await _context.Memberships.AnyAsync(m =>
                m.MemberId == membership.MemberId &&
                m.StartDate < membership.EndDate &&
                m.EndDate > membership.StartDate
            );
            if (hasOverlappingMembership)
            {
                return BadRequest("Member already has an overlapping membership.");
            }
            _context.Memberships.Add(membership);
            await _context.SaveChangesAsync();
             return CreatedAtAction(nameof(GetMembership), new { id = membership.Id }, membership);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult>UpdateMembership(int id, Membership updatedMembership)
        {
            var membership = await _context.Memberships.FindAsync(id);
            if(membership == null)
            {
                return NotFound();
            }
            if (id!=updatedMembership.Id)
            {
                return BadRequest("Membership ID mismatch.");
            }
            var member = await _context.Members.FindAsync(updatedMembership.MemberId);
            if(member == null)
            {
                return BadRequest("Member does not exist.");
            }
            if (!member.IsActive)
            {
                return BadRequest("Cannot update membership for inactive member.");
            }
            if(updatedMembership.EndDate < updatedMembership.StartDate)
            {
                return BadRequest("EndDate must be after StartDate.");
            }
            if(updatedMembership.Fee < 0)
            {
                return BadRequest("Fee must be a positive value.");
            }
            bool hasConflict = await _context.Memberships.AnyAsync(m =>
                m.MemberId == updatedMembership.MemberId &&
                m.Id != id &&
                m.StartDate < updatedMembership.EndDate &&
                m.EndDate > updatedMembership.StartDate
            );
            if (hasConflict)
            {
                return BadRequest("Member already has an overlapping membership.");
            }
            membership.PlanName = updatedMembership.PlanName;
            membership.StartDate = updatedMembership.StartDate;
            membership.EndDate = updatedMembership.EndDate;
            membership.Fee = updatedMembership.Fee;
            membership.PaymentStatus = updatedMembership.PaymentStatus;
            membership.MemberId = updatedMembership.MemberId;
            await _context.SaveChangesAsync();
            return Ok(membership);

        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMembership(int id)
        {
            var membership = await _context.Memberships.FindAsync(id);
            if (membership == null)
            {
                return NotFound();
            }
            _context.Memberships.Remove(membership);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
