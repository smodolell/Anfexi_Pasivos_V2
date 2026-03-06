using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class SB_TipoMonedaConfiguration : IEntityTypeConfiguration<SB_TipoMoneda>
{
    public void Configure(EntityTypeBuilder<SB_TipoMoneda> builder)
    {
        builder.ToTable("SB_TipoMoneda");

        builder.HasKey(e => e.IdTipoMoneda);

        builder.Property(e => e.DescTipoMoneda)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.CveCortaTipoMoneda)
            .IsRequired()
            .HasMaxLength(10);
    }
}
