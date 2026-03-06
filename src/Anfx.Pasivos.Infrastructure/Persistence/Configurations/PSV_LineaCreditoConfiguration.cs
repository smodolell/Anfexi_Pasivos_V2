using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class PSV_LineaCreditoConfiguration : IEntityTypeConfiguration<PSV_LineaCredito>
{
    public void Configure(EntityTypeBuilder<PSV_LineaCredito> builder)
    {
        builder.ToTable("PSV_LineaCredito");

        builder.HasKey(e => e.IdLineaCredito);

        builder.HasOne(e => e.PSV_Fondeador)
            .WithMany(f => f.PSV_LineaCredito)
            .HasForeignKey(e => e.IdFondeador)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.SB_TipoMoneda)
            .WithMany(m => m.PSV_LineaCredito)
            .HasForeignKey(e => e.IdMoneda)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Tasa1)
            .WithMany(t => t.PSV_LineaCredito)
            .HasForeignKey(e => e.IdTasa)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
