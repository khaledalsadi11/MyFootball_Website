using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Football.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Football.Configurations
{
    public class PlayerEntityTypeConfiguration : IEntityTypeConfiguration<Player>
    {
        public void Configure(EntityTypeBuilder<Player> builder)
        {

            builder.HasOne(x=>x.Club)
                   .WithMany(x=>x.Players)
                   .HasForeignKey(x=>x.ClubId);


            builder.HasOne(x => x.Nationality)
                   .WithMany(x => x.Players)
                   .HasForeignKey(x => x.NationalityId);

            

        }
    }
}
