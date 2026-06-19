using Domain.Entities;
using Infrastructure.DataAccessManager.EFCore.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataAccessManager.EFCore.Configurations;

public class PriceListConfiguration : BaseEntityConfiguration<PriceList>
{
    public override void Configure(EntityTypeBuilder<PriceList> builder)
    {
        base.Configure(builder);

        builder
            .HasOne(pl => pl.Product)
            .WithMany()
            .HasForeignKey(pl => pl.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(pl => pl.Customer)
            .WithMany()
            .HasForeignKey(pl => pl.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(pl => pl.Tax)
            .WithMany()
            .HasForeignKey(pl => pl.TaxId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(pl => pl.QuantityDiscount)
            .HasPrecision(18, 4);
    }
}
