
using Football.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Football.Configurations
{
    public class PlayerEntityTypeConfiguration
        : IEntityTypeConfiguration<Player>
    {
        public void Configure(EntityTypeBuilder<Player> builder)
        {
            builder.HasOne(p => p.Club)
                .WithMany(c => c.Players)
                .HasForeignKey(p => p.ClubId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Nationality)
                .WithMany(n => n.Players)
                .HasForeignKey(p => p.NationalityId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
