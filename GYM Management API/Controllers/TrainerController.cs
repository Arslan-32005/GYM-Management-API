using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using GYM_Management_API.Data;
using GYM_Management_API.Models;
using Microsoft.Identity.Client.NativeInterop;

namespace GYM_Management_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainerController : ControllerBase
    {
        private readonly GymContext _context;
        public TrainerController(GymContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<Trainer>> GetAllTrainers(bool IsAvailable, string specialization, int page=1, int pageSize = 5)
        {
            
            
            IQueryable<Trainer> trainers = _context.Trainers; 
            if (IsAvailable)
            {
                trainers = trainers.Where(t => t.IsAvailable);
            }
            if(!string.IsNullOrEmpty(specialization))
            {
                trainers = trainers.Where(t => t.Specialization.Contains(specialization));
            }
            trainers = trainers.Skip((page - 1) * pageSize).Take(pageSize);
            var trainer = await trainers.ToListAsync();

            return Ok(trainer);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetTrainer(int id)
        {
            var trainer = await _context.Trainers.Include(t=>t.Members).Where(t => t.Id == id).Select(t => new {
                t.Id,
                t.FullName,
                t.Email,
                t.PhoneNumber,
                t.Specialization,
                t.IsAvailable,
                Members = t.Members.Select(m => new
                {
                    m.Id,
                    m.FullName,
                    m.Email,
                    m.IsActive
                }).ToList()
            }).FirstOrDefaultAsync();

            if (trainer == null)
            {
                return NotFound();
            }
            return Ok(trainer);
        }
        [HttpPost]
        public async Task<ActionResult> CreateTrainer(Trainer trainer)
        {
            _context.Trainers.Add(trainer);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTrainer), new { id = trainer.Id }, trainer);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTrainer(int id, Trainer updatedTrainer)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null)
            {
                return NotFound();
            }
            if (id != updatedTrainer.Id)
            {
                return BadRequest();
            }
            trainer.FullName = updatedTrainer.FullName;
            trainer.Email = updatedTrainer.Email;
            trainer.PhoneNumber = updatedTrainer.PhoneNumber;
            trainer.Specialization = updatedTrainer.Specialization;
            trainer.IsAvailable = updatedTrainer.IsAvailable;
            await _context.SaveChangesAsync();
            return Ok(trainer);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTrainer(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null)
            {
                return NotFound();
            }
            bool hasMembers = await _context.Members.AnyAsync(m => m.TrainerId == id);
            if (hasMembers)
            {
                return BadRequest("Cannot delete trainer becuase he/she has Members.");
            }
            _context.Trainers.Remove(trainer);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
