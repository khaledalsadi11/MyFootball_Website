
using Football.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Football.Configurations
{
    public class ClubEntityTypeConfiguration
        : IEntityTypeConfiguration<Club>
    {
        public void Configure(EntityTypeBuilder<Club> builder)
        {
            builder.HasIndex(c => c.Name)
                .IsUnique();

            // Club 1 -> Many Teams
            builder.HasMany(c => c.Teams)
                .WithOne(t => t.Club)
                .HasForeignKey(t => t.ClubId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
