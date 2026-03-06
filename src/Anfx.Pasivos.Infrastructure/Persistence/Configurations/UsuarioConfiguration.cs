using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        // Mapeo de tabla y esquema
        builder.ToTable("Usuario");

        // Llave primaria - Asumiendo que agregarás una propiedad Id
        builder.HasKey(e => e.Id);

        // Propiedades
        builder.Property(e => e.NombreCompleto)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Email)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(e => e.UsuarioNombre)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Contrasena)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(e => e.FechaRegistro)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        builder.Property(e => e.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.RolId)
            .IsRequired();

        // Índices
        builder.HasIndex(e => e.Email)
            .IsUnique()
            .HasDatabaseName("IX_Usuario_Email");

        builder.HasIndex(e => e.UsuarioNombre)
            .IsUnique()
            .HasDatabaseName("IX_Usuario_UsuarioNombre");

        builder.HasIndex(e => e.RolId)
            .HasDatabaseName("IX_Usuario_RolId");

        builder.HasIndex(e => e.Activo)
            .HasDatabaseName("IX_Usuario_Activo");

        builder.HasIndex(e => new { e.Email, e.Activo })
            .HasDatabaseName("IX_Usuario_Email_Activo");
    }
}