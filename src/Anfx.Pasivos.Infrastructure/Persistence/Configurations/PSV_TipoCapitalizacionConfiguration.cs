using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class PSV_TipoCapitalizacionConfiguration : IEntityTypeConfiguration<PSV_TipoCapitalizacion>
{
    public void Configure(EntityTypeBuilder<PSV_TipoCapitalizacion> builder)
    {
        builder.ToTable("PSV_TipoCapitalizacion");

        builder.HasKey(e => e.IdTipoCapitalizacion);

        builder.Property(e => e.TipoCapitalizacion)
            .IsRequired()
            .HasMaxLength(100);
    }
}
