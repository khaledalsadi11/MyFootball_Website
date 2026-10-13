using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Football.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Football.Configurations
{
    public class CoachEntityTypeConfiguration : IEntityTypeConfiguration<Coach>
    {
        public void Configure(EntityTypeBuilder<Coach> builder)
        {
            builder.HasOne(x => x.Club)
       .WithOne(x => x.Coach)
       .HasForeignKey<Coach>(x => x.ClubId);


            builder.HasOne(x => x.Nationality)
     .WithMany(x => x.Coaches)
     .HasForeignKey(x => x.NationalityId);



            
        }
    }
}
