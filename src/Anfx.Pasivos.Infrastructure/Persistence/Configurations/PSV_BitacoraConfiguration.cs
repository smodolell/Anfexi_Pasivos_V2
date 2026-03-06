using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class PSV_BitacoraConfiguration : IEntityTypeConfiguration<PSV_Bitacora>
{
    public void Configure(EntityTypeBuilder<PSV_Bitacora> builder)
    {
        builder.ToTable("PSV_Bitacora");

        builder.HasKey(e => e.ID);

        builder.Property(e => e.Usuario)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Pantalla)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Accion)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Descripcion)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.FechaOperacion)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");
    }
}
