using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class PSV_ContratoConfiguration : IEntityTypeConfiguration<PSV_Contrato>
{
    public void Configure(EntityTypeBuilder<PSV_Contrato> builder)
    {
        builder.ToTable("PSV_Contrato");

        builder.HasKey(e => e.IdContrato);

        builder.Property(e => e.Contrato)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasOne(e => e.PSV_EstatusContrato)
            .WithMany(es => es.PSV_Contrato)
            .HasForeignKey(e => e.IdEstatusContrato)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.PSV_Fondeador)
            .WithMany(f => f.PSV_Contrato)
            .HasForeignKey(e => e.IdFondeador)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.PSV_TipoCapitalizacion)
            .WithMany(c => c.PSV_Contrato)
            .HasForeignKey(e => e.IdTipoCapitalizacion)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.PSV_TipoCredito)
            .WithMany(tc => tc.PSV_Contrato)
            .HasForeignKey(e => e.IdTipoCredito)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.PSV_TipoPagoCapital)
            .WithMany(tpc => tpc.PSV_Contrato)
            .HasForeignKey(e => e.IdTipoPagoCapital)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.SB_Periodicidad)
            .WithMany(p => p.PSV_Contrato)
            .HasForeignKey(e => e.IdPeriodicidad)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.SB_Periodicidad1)
            .WithMany(p => p.PSV_Contrato1)
            .HasForeignKey(e => e.IdPeriodicidad_TTA)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.SB_TipoMoneda)
            .WithMany(m => m.PSV_Contrato)
            .HasForeignKey(e => e.IdMoneda)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Tasa1)
            .WithMany(t => t.PSV_Contrato)
            .HasForeignKey(e => e.IdTasa)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Tasa2)
            .WithMany(t => t.PSV_Contrato1)
            .HasForeignKey(e => e.IdTasaMora)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.PSV_TipoTablaAmortiza)
            .WithMany(tta => tta.PSV_Contrato)
            .HasForeignKey(e => e.IdTipoTablaAmortiza)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
