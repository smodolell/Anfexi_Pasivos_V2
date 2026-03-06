using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class PSV_TipoTablaAmortizaPeriodicidadConfiguration : IEntityTypeConfiguration<PSV_TipoTablaAmortizaPeriodicidad>
{
    public void Configure(EntityTypeBuilder<PSV_TipoTablaAmortizaPeriodicidad> builder)
    {
        builder.ToTable("PSV_TipoTablaAmortizaPeriodicidad");

        builder.HasKey(e => new { e.IdTipoTablaAmortiza, e.IdPeriodicidad });

        builder.HasOne(e => e.PSV_TipoTablaAmortiza)
            .WithMany(t => t.PSV_TipoTablaAmortizaPeriodicidad)
            .HasForeignKey(e => e.IdTipoTablaAmortiza)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.SB_Periodicidad)
            .WithMany(p => p.PSV_TipoTablaAmortizaPeriodicidad)
            .HasForeignKey(e => e.IdPeriodicidad)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
