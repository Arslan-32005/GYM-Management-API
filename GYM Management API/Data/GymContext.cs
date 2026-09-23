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


            base.OnModelCreating(modelBuilder);
        }
    }
    
}
