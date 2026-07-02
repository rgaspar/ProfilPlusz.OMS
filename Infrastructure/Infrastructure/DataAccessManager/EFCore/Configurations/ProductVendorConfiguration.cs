using Domain.Entities;
using Infrastructure.DataAccessManager.EFCore.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataAccessManager.EFCore.Configurations;

public class ProductVendorConfiguration : BaseEntityConfiguration<ProductVendor>
{
    public override void Configure(EntityTypeBuilder<ProductVendor> builder)
    {
        base.Configure(builder);

        builder
            .HasOne(pv => pv.Product)
            .WithMany()
            .HasForeignKey(pv => pv.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(pv => pv.Vendor)
            .WithMany()
            .HasForeignKey(pv => pv.VendorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
