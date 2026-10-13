using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Football.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Football.Configurations
{
    public class NationalityEntityTypeConfiguration : IEntityTypeConfiguration<Nationality>
    {
        public void Configure(EntityTypeBuilder<Nationality> builder)
        {

        }
    }
}
