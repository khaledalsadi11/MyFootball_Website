using Football.Models;
using Microsoft.EntityFrameworkCore;
using Football.Configurations;

namespace Football.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {


        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PlayerEntityTypeConfiguration).Assembly);

        }
        public DbSet<Club> Clubs { get; set; }
        public DbSet<Player> Players { get; set; }

        public DbSet<Coach> Coaches { get; set; }

        public DbSet<Nationality> Nationalities { get; set; }
        public DbSet<League> Leagues { get; set; }

    }
}
