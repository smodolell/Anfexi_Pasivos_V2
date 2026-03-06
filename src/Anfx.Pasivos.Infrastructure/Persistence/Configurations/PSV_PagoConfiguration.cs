using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class PSV_PagoConfiguration : IEntityTypeConfiguration<PSV_Pago>
{
    public void Configure(EntityTypeBuilder<PSV_Pago> builder)
    {
        builder.ToTable("PSV_Pago");

        builder.HasKey(e => e.IdPago);

        builder.Property(e => e.Contrato)
            .HasMaxLength(50);

        builder.HasOne(e => e.PSV_Fondeador)
            .WithMany(f => f.PSV_Pago)
            .HasForeignKey(e => e.IdFondeador)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.PSV_CuentaBancaria)
            .WithMany(cb => cb.PSV_Pago)
            .HasForeignKey(e => e.IdCuentaBancaria)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.PSV_TipoPago)
            .WithMany(tp => tp.PSV_Pago)
            .HasForeignKey(e => e.IdTipoPago)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
