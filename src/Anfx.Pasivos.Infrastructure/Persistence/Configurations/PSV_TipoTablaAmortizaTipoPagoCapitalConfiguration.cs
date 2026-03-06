using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class PSV_TipoTablaAmortizaTipoPagoCapitalConfiguration : IEntityTypeConfiguration<PSV_TipoTablaAmortizaTipoPagoCapital>
{
    public void Configure(EntityTypeBuilder<PSV_TipoTablaAmortizaTipoPagoCapital> builder)
    {
        builder.ToTable("PSV_TipoTablaAmortizaTipoPagoCapital");

        builder.HasKey(e => new { e.IdTipoTablaAmortiza, e.IdTipoPagoCapital });

        builder.HasOne(e => e.PSV_TipoTablaAmortiza)
            .WithMany(t => t.PSV_TipoTablaAmortizaTipoPagoCapital)
            .HasForeignKey(e => e.IdTipoTablaAmortiza)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.PSV_TipoPagoCapital)
            .WithMany(c => c.PSV_TipoTablaAmortizaTipoPagoCapital)
            .HasForeignKey(e => e.IdTipoPagoCapital)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
