using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Football.Models;

using System.Collections.Generic;
using System.Text;

namespace Football.Configurations
{
    public class ClubEntityTypeConfiguration : IEntityTypeConfiguration<Club>
    {
        public void Configure(EntityTypeBuilder<Club> builder)
        {
            builder.HasIndex(c => c.Name).IsUnique();
        }
    }
}
