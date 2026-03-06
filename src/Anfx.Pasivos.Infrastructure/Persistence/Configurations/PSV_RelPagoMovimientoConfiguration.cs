using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class PSV_RelPagoMovimientoConfiguration : IEntityTypeConfiguration<PSV_RelPagoMovimiento>
{
    public void Configure(EntityTypeBuilder<PSV_RelPagoMovimiento> builder)
    {
        builder.ToTable("PSV_RelPagoMovimiento");

        builder.HasKey(e => e.IdPagoMovimiento);

        builder.HasOne(e => e.PSV_Pago)
            .WithMany(p => p.PSV_RelPagoMovimiento)
            .HasForeignKey(e => e.IdPago)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.PSV_Movimiento)
            .WithMany(m => m.PSV_RelPagoMovimiento)
            .HasForeignKey(e => e.IdMovimiento)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
