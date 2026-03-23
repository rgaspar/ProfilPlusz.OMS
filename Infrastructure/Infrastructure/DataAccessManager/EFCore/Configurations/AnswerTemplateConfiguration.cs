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
    public class AnswerTemplateConfiguration : IEntityTypeConfiguration<AnswerTemplate>
    {
        public void Configure(EntityTypeBuilder<AnswerTemplate> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Key).IsRequired();
            builder.Property(x => x.Path).IsRequired();
        }
    }
}
