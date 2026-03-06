using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class PSV_MovimientoConfiguration : IEntityTypeConfiguration<PSV_Movimiento>
{
    public void Configure(EntityTypeBuilder<PSV_Movimiento> builder)
    {
        builder.ToTable("PSV_Movimiento");

        builder.HasKey(e => e.IdMovimiento);

        builder.Property(e => e.Descripcion)
            .HasMaxLength(255);

        builder.HasOne(e => e.TipoMovimiento)
            .WithMany(tm => tm.PSV_Movimiento)
            .HasForeignKey(e => e.IdTipoMovimiento)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.PSV_Contrato)
            .WithMany(c => c.PSV_Movimiento)
            .HasForeignKey(e => e.IdContrato)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
