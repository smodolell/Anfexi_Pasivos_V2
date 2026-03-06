using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class TipoMovimientoConfiguration : IEntityTypeConfiguration<TipoMovimiento>
{
    public void Configure(EntityTypeBuilder<TipoMovimiento> builder)
    {
        builder.ToTable("TipoMovimiento");

        builder.HasKey(e => e.IdTipoMovimiento);

        builder.Property(e => e.TipoMovimiento1)
            .HasColumnName("TipoMovimiento")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.ClaveTipoMovimiento)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasOne(e => e.TipoGeneracionComprobante)
            .WithMany(g => g.TipoMovimiento)
            .HasForeignKey(e => e.IdTipoGeneracionComprobante)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
