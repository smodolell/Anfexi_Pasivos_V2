using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class PSV_RelLineaCreditoContratoConfiguration : IEntityTypeConfiguration<PSV_RelLineaCreditoContrato>
{
    public void Configure(EntityTypeBuilder<PSV_RelLineaCreditoContrato> builder)
    {
        builder.ToTable("PSV_RelLineaCreditoContrato");

        // Clave compuesta
        builder.HasKey(e => new { e.IdLineaCredito, e.IdContrato });

        // Relación con PSV_LineaCredito
        builder.HasOne(e => e.PSV_LineaCredito)
            .WithMany(lc => lc.PSV_RelLineaCreditoContrato)
            .HasForeignKey(e => e.IdLineaCredito)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación con PSV_Contrato
        builder.HasOne(e => e.PSV_Contrato)
            .WithMany(c => c.PSV_RelLineaCreditoContrato)
            .HasForeignKey(e => e.IdContrato)
            .OnDelete(DeleteBehavior.Restrict);
    }
}