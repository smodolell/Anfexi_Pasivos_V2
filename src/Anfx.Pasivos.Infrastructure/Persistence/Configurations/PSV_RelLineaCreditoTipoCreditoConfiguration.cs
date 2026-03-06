using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class PSV_RelLineaCreditoTipoCreditoConfiguration : IEntityTypeConfiguration<PSV_RelLineaCreditoTipoCredito>
{
    public void Configure(EntityTypeBuilder<PSV_RelLineaCreditoTipoCredito> builder)
    {
        builder.ToTable("PSV_RelLineaCreditoTipoCredito");

        builder.HasKey(e => new { e.IdLineaCredito, e.IdTipoCredito });

        builder.HasOne(e => e.PSV_LineaCredito)
            .WithMany(l => l.PSV_RelLineaCreditoTipoCredito)
            .HasForeignKey(e => e.IdLineaCredito)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.TipoCredito)
            .WithMany()
            .HasForeignKey(e => e.IdTipoCredito)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
