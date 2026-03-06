using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class PSV_ArchivoConfiguration : IEntityTypeConfiguration<PSV_Archivo>
{
    public void Configure(EntityTypeBuilder<PSV_Archivo> builder)
    {
        builder.ToTable("PSV_Archivo");

        builder.HasKey(e => e.ID);

        builder.Property(e => e.NombreArchivo)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.Contenido)
            .IsRequired();

        builder.Property(e => e.Activo)
            .IsRequired();
    }
}
