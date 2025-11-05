using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApi.Api.Features.Sales.Infrastructure.Models;

namespace WebApi.Api.Features.Sales.Infrastructure.Configurations;

public sealed class SaleModelConfiguration : IEntityTypeConfiguration<SaleModel>
{
    public void Configure(EntityTypeBuilder<SaleModel> b)
    {
        b.ToTable("sale");

        b.HasKey(x => x.Id)
         .HasName("pk_sale");

        b.HasIndex(x => x.NavigationId)
         .IsUnique()
         .HasDatabaseName("ux_sale_navigation_id");

        b.Property(x => x.Id)
            .HasColumnName("id");

        b.Property(x => x.NavigationId)
            .HasColumnName("navigation_id")
            .IsRequired();

        b.Property(x => x.ClientId)
            .HasColumnName("client_id")
            .IsRequired();

        b.Property(x => x.ClientName)
            .HasColumnName("client_name")
            .HasMaxLength(200)
            .IsRequired();

        b.Property(x => x.Installments)
            .HasColumnName("installments")
            .IsRequired();

        b.Property(x => x.PaymentMethod)
            .HasColumnName("payment_method")
            .HasMaxLength(50)
            .IsRequired();

        b.HasMany(x => x.Items)
            .WithOne(i => i.Sale)
            .HasForeignKey(i => i.SaleId)
            .HasConstraintName("fk_sale_item__sale_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
