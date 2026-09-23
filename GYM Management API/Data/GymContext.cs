using Microsoft.EntityFrameworkCore;
using GYM_Management_API.Models;
namespace GYM_Management_API.Data
{
    public class GymContext: DbContext
    {
        public GymContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Member> Members { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<WorkoutPlan> WorkoutPlans { get; set; }
        public DbSet<MemberWorkoutPlan> MemberWorkoutPlans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Trainer>()
                .HasMany(t => t.Members)
                .WithOne(m => m.Trainer)
                .HasForeignKey(m => m.TrainerId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            // one member to many memberships
            modelBuilder.Entity<Member>()
                .HasMany(m => m.Memberships)
                .WithOne(ms => ms.Member)
                .HasForeignKey(ms=>ms.MemberId)
                .IsRequired(true)
                
                .OnDelete(DeleteBehavior.Cascade);
            // one member to many MemberWorkoutPlan
            modelBuilder.Entity<Member>()
                .HasMany(m => m.MemberWorkoutPlans)
                .WithOne(mw => mw.Member)
                .HasForeignKey(mw=>mw.MemberId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);
            // workoutplan to member workout plan(M to M)
            modelBuilder.Entity<WorkoutPlan>()
                .HasMany(wp => wp.MemberWorkoutPlans)
                .WithOne(mw => mw.WorkoutPlan)
                .HasForeignKey(mw => mw.WorkoutPlanId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);
            // membership with fee to scale precison
            modelBuilder.Entity<Membership>()
                .Property(m => m.Fee)
                .HasColumnType("decimal(8,2)");

            modelBuilder.Entity<MemberWorkoutPlan>()
            .HasIndex(m => new { m.MemberId, m.WorkoutPlanId })
            .IsUnique();
            modelBuilder.Entity<Member>()
           .Property(m => m.FullName)
           .HasMaxLength(100);

            modelBuilder.Entity<Trainer>()
            .Property(t => t.FullName)
            .HasMaxLength(100);
            modelBuilder.Entity<Member>()
            .Property(m => m.Email)
            .HasMaxLength(150);

            modelBuilder.Entity<Trainer>()
                .Property(t => t.Email)
                .HasMaxLength(150);
            modelBuilder.Entity<Member>()
             .Property(m => m.PhoneNumber)
               .HasMaxLength(20);

            modelBuilder.Entity<Trainer>()
                .Property(t => t.PhoneNumber)
                .HasMaxLength(20);
            modelBuilder.Entity<Membership>()
                   .Property(m => m.PlanName)
                   .HasMaxLength(100);

            modelBuilder.Entity<Membership>()
                .Property(m => m.PaymentStatus)
                .HasMaxLength(30);
            modelBuilder.Entity<WorkoutPlan>()
    .Property(w => w.Name)
    .HasMaxLength(100);

            modelBuilder.Entity<WorkoutPlan>()
                .Property(w => w.Goal)
                .HasMaxLength(50);

            modelBuilder.Entity<WorkoutPlan>()
                .Property(w => w.DifficultyLevel)
                .HasMaxLength(30);
            modelBuilder.Entity<WorkoutPlan>()
                .Property(w => w.Name)
                .HasMaxLength(100);

            modelBuilder.Entity<WorkoutPlan>()
                .Property(w => w.Goal)
                .HasMaxLength(50);

            modelBuilder.Entity<WorkoutPlan>()
                .Property(w => w.DifficultyLevel)
                .HasMaxLength(30);


            base.OnModelCreating(modelBuilder);
        }
    }
    
}
