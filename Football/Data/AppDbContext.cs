
using Football.Models;
using Microsoft.EntityFrameworkCore;
using Football.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Football.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(
            DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Load Entity Configurations
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(PlayerEntityTypeConfiguration).Assembly);

            // AgeGroup -> Teams
            modelBuilder.Entity<Team>()
                .HasOne(t => t.AgeGroup)
                .WithMany(a => a.Teams)
                .HasForeignKey(t => t.AgeGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            // Coach -> Teams
            modelBuilder.Entity<Team>()
                .HasOne(t => t.Coach)
                .WithMany(c => c.Teams)
                .HasForeignKey(t => t.CoachId)
                .OnDelete(DeleteBehavior.SetNull);

            // Team -> Players
            modelBuilder.Entity<Player>()
                .HasOne(p => p.Team)
                .WithMany(t => t.Players)
                .HasForeignKey(p => p.TeamId)
                .OnDelete(DeleteBehavior.SetNull);

            // Team -> TrainingSessions
            modelBuilder.Entity<TrainingSession>()
                .HasOne(s => s.Team)
                .WithMany(t => t.TrainingSessions)
                .HasForeignKey(s => s.TeamId)
                .OnDelete(DeleteBehavior.Restrict);

            // Player -> Attendance
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Player)
                .WithMany(p => p.Attendances)
                .HasForeignKey(a => a.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            // TrainingSession -> Attendance
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.TrainingSession)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.TrainingSessionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prevent duplicate attendance
            modelBuilder.Entity<Attendance>()
                .HasIndex(a => new
                {
                    a.TrainingSessionId,
                    a.PlayerId
                })
                .IsUnique();

            //
            var UserRole = Guid.NewGuid().ToString();
            var AdminRole = Guid.NewGuid().ToString();

            modelBuilder.Entity<IdentityRole>().HasData(
       new IdentityRole
       {
           Id = "a97b2105-51d1-4e23-a94a-6b8d472a3c01",
           Name = "Coach",
           NormalizedName = "COACH",
           ConcurrencyStamp = "a97b2105-51d1-4e23-a94a-6b8d472a3c01"
       },
       new IdentityRole
       {
           Id = "a97b2105-51d1-4e23-a94a-6b8d472a3c02",
           Name = "Admin",
           NormalizedName = "ADMIN",
           ConcurrencyStamp = "a97b2105-51d1-4e23-a94a-6b8d472a3c02"
       }
   );
        }

        public DbSet<Club> Clubs { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Coach> Coaches { get; set; }
        public DbSet<Nationality> Nationalities { get; set; }
        public DbSet<AgeGroup> AgeGroups { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TrainingSession> TrainingSessions { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
    }
}
