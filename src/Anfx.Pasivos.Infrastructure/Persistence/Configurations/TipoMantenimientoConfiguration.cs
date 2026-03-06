using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class TipoMantenimientoConfiguration : IEntityTypeConfiguration<TipoMantenimiento>
{
    public void Configure(EntityTypeBuilder<TipoMantenimiento> builder)
    {
        builder.ToTable("TipoMantenimiento");

        builder.HasKey(e => e.IdTipoMantenimiento);

        builder.Property(e => e.Clave)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Descripcion)
            .IsRequired()
            .HasMaxLength(200);
    }
}
