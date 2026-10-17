
using Football.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Football.Configurations
{
    public class CoachEntityTypeConfiguration
        : IEntityTypeConfiguration<Coach>
    {
        public void Configure(EntityTypeBuilder<Coach> builder)
        {
            // Club 1 -> Many Coaches
            builder.HasOne(c => c.Club)
                .WithMany(c => c.Coaches)
                .HasForeignKey(c => c.ClubId)
                .OnDelete(DeleteBehavior.Restrict);

            // Nationality 1 -> Many Coaches
            builder.HasOne(c => c.Nationality)
                .WithMany(n => n.Coaches)
                .HasForeignKey(c => c.NationalityId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
