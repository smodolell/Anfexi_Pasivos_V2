using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class TipoDireccionConfiguration : IEntityTypeConfiguration<TipoDireccion>
{
    public void Configure(EntityTypeBuilder<TipoDireccion> builder)
    {
        // Mapeo de tabla y esquema
        builder.ToTable("TipoDireccion");

        // Llave primaria
        builder.HasKey(e => e.Id);

        // Propiedades
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.sTipoDireccion)
            .HasColumnName("TipoDireccion")
            .HasMaxLength(200)
            .IsRequired();

        // Índices
        builder.HasIndex(e => e.sTipoDireccion)
            .IsUnique()
            .HasDatabaseName("IX_TipoDireccion_sTipoDireccion");
    }
}
