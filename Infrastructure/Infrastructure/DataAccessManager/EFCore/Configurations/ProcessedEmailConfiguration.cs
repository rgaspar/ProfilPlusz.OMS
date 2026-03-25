using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DataAccessManager.EFCore.Configurations
{
    public class ProcessedEmailConfiguration : IEntityTypeConfiguration<ProcessedEmail>
    {
        public void Configure(EntityTypeBuilder<ProcessedEmail> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ExternalId)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.FromEmail)
                .HasMaxLength(200);

            builder.Property(x => x.Subject)
                .HasMaxLength(500);

            builder.Property(x => x.OrderNumber)
                .HasMaxLength(50);

            builder.Property(x => x.CustomerName)
                .HasMaxLength(200);

            builder.Property(x => x.County)
                .HasMaxLength(100);

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.ErrorMessage)
                .HasMaxLength(2000);

            builder.HasIndex(x => x.ExternalId)
                .IsUnique();

            builder.HasIndex(x => x.OrderNumber);
        }
    }
}
