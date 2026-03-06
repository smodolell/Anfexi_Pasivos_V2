using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class RolConfiguration : IEntityTypeConfiguration<Rol>
{
    public void Configure(EntityTypeBuilder<Rol> builder)
    {
        // Mapeo de tabla y esquema
        builder.ToTable("Rol");

        // Llave primaria - Asumiendo que agregarás una propiedad Id
        builder.HasKey(e => e.Id);

        // Propiedades
        builder.Property(e => e.sRol)
            .HasColumnName("Rol")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Descripcion)
            .HasMaxLength(500); // Sugerencia: agregar longitud máxima

        builder.Property(e => e.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        // Relación con Usuarios
        builder.HasMany(e => e.Usuarios)
            .WithOne(e => e.Rol)
            .HasForeignKey(e => e.RolId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(e => e.sRol)
            .IsUnique()
            .HasDatabaseName("IX_Rol_sRol");

        builder.HasIndex(e => e.Activo)
            .HasDatabaseName("IX_Rol_Activo");
    }
}
