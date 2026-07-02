using Domain.Entities;
using Infrastructure.DataAccessManager.EFCore.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataAccessManager.EFCore.Configurations;

public class ProductCustomerConfiguration : BaseEntityConfiguration<ProductCustomer>
{
    public override void Configure(EntityTypeBuilder<ProductCustomer> builder)
    {
        base.Configure(builder);

        builder
            .HasOne(pc => pc.Product)
            .WithMany()
            .HasForeignKey(pc => pc.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(pc => pc.Customer)
            .WithMany()
            .HasForeignKey(pc => pc.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
