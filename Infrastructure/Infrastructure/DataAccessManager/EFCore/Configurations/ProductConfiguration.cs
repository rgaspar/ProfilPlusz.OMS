using Domain.Entities;
using Infrastructure.DataAccessManager.EFCore.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants;

namespace Infrastructure.DataAccessManager.EFCore.Configurations;

public class ProductConfiguration : BaseEntityConfiguration<Product>
{
    public override void Configure(EntityTypeBuilder<Product> builder)
    {
        base.Configure(builder);

        // alap mezők
        builder.Property(x => x.Name)
            .HasMaxLength(500);

        builder.Property(x => x.Number)
            .HasMaxLength(100);

        builder.HasIndex(x => x.Number)
            .IsUnique();

        builder.Property(x => x.FactoryName)
            .HasMaxLength(500);

        builder.Property(x => x.Description)
            .HasMaxLength(4000);


        // azonosítók
        builder.Property(x => x.Ean)
            .HasMaxLength(50);

        builder.Property(x => x.ManufacturerNumber)
            .HasMaxLength(200);

        builder.Property(x => x.Manufacturer)
            .HasMaxLength(200);


        // mennyiségek
        builder.Property(x => x.SalesUnitQuantity)
            .HasPrecision(18, 4);

        builder.Property(x => x.MinimumSalesQuantity)
            .HasPrecision(18, 4);

        builder.Property(x => x.OrderQuantityStep)
            .HasPrecision(18, 4);

        builder.Property(x => x.Weight)
            .HasPrecision(18, 4);

        builder.Property(x => x.Length)
            .HasPrecision(18, 4);


        // média
        builder.Property(x => x.Image1Url)
            .HasMaxLength(1000);

        builder.Property(x => x.Image2Url)
            .HasMaxLength(1000);

        builder.Property(x => x.Image3Url)
            .HasMaxLength(1000);

        builder.Property(x => x.VideoUrl)
            .HasMaxLength(1000);

        builder.Property(x => x.PdfUrl)
            .HasMaxLength(1000);


        // enumok stringként tárolva (olvashatóbb DB)
        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(x => x.PurchaseCurrency)
            .HasConversion<string>()
            .HasMaxLength(10);

        builder.Property(x => x.SalesCurrency)
            .HasConversion<string>()
            .HasMaxLength(10);


        // kapcsolatok
        builder
            .HasOne(p => p.Brand)
            .WithMany(b => b.Products)
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(p => p.Color)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.ColorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(p => p.ProductGroup)
            .WithMany()
            .HasForeignKey(p => p.ProductGroupId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(p => p.UnitMeasure)
            .WithMany()
            .HasForeignKey(p => p.UnitMeasureId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

