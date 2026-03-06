using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class PSV_TipoTablaAmortizaTipoCapitalizacionConfiguration : IEntityTypeConfiguration<PSV_TipoTablaAmortizaTipoCapitalizacion>
{
    public void Configure(EntityTypeBuilder<PSV_TipoTablaAmortizaTipoCapitalizacion> builder)
    {
        builder.ToTable("PSV_TipoTablaAmortizaTipoCapitalizacion");

        builder.HasKey(e => new { e.IdTipoTablaAmortiza, e.IdTipoCapitalizacion });

        builder.HasOne(e => e.PSV_TipoTablaAmortiza)
            .WithMany(t => t.PSV_TipoTablaAmortizaTipoCapitalizacion)
            .HasForeignKey(e => e.IdTipoTablaAmortiza)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.PSV_TipoCapitalizacion)
            .WithMany(c => c.PSV_TipoTablaAmortizaTipoCapitalizacion)
            .HasForeignKey(e => e.IdTipoCapitalizacion)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
