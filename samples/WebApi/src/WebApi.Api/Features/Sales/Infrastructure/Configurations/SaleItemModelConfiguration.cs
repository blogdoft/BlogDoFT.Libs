using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApi.Api.Features.Sales.Infrastructure.Models;

namespace WebApi.Api.Features.Sales.Infrastructure.Configurations;

public sealed class SaleItemModelConfiguration : IEntityTypeConfiguration<SaleItemModel>
{
    public void Configure(EntityTypeBuilder<SaleItemModel> b)
    {
        b.ToTable("sale_item");

        b.HasKey(x => x.Id)
         .HasName("pk_sale_item");

        b.Property(x => x.Id)
            .HasColumnName("id");

        b.Property(x => x.SaleId)
            .HasColumnName("sale_id")
            .IsRequired();

        b.Property(x => x.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        b.Property(x => x.ProductDescription)
            .HasColumnName("product_description")
            .HasMaxLength(300);

        b.Property(x => x.Quantity)
            .HasColumnName("quantity")
            .HasPrecision(18, 3)
            .IsRequired();

        b.Property(x => x.UnitValue)
            .HasColumnName("unit_value")
            .HasPrecision(18, 2)
            .IsRequired();

        b.Property(x => x.Discount)
            .HasColumnName("discount")
            .HasPrecision(18, 4)
            .IsRequired();

        b.HasIndex(x => x.SaleId)
            .HasDatabaseName("ix_sale_item__sale_id");

        b.HasIndex(x => x.ProductId)
            .HasDatabaseName("ix_sale_item__product_id");
    }
}
