using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class PSV_TipoTerminacionConfiguration : IEntityTypeConfiguration<PSV_TipoTerminacion>
{
    public void Configure(EntityTypeBuilder<PSV_TipoTerminacion> builder)
    {
        builder.ToTable("PSV_TipoTerminacion");

        builder.HasKey(e => e.IdTipoTerminacion);

        builder.Property(e => e.TipoTerminacion)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(e => e.TipoMovimiento)
            .WithMany(m => m.PSV_TipoTerminacion)
            .HasForeignKey(e => e.IdTipoMovimientoBaja)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.TipoMovimiento1)
            .WithMany(m => m.PSV_TipoTerminacion1)
            .HasForeignKey(e => e.IdTipoMovimientoPena)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.TipoMovimiento2)
            .WithMany(m => m.PSV_TipoTerminacion2)
            .HasForeignKey(e => e.IdTipoMovimientoInteres)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.PSV_CuentaBancaria)
            .WithMany()
            .HasForeignKey(e => e.IdCuentaBancariaDeposito)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.PSV_TipoPago)
            .WithMany(t => t.PSV_TipoTerminacion)
            .HasForeignKey(e => e.IdTipoPagoDeposito)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.PSV_EstatusContrato)
            .WithMany(es => es.PSV_TipoTerminacion)
            .HasForeignKey(e => e.IdEstatusContratoTerminacion)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
