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
        builder.Property(e => e.Id)
            .HasColumnName("IdUsuario");

        builder.HasKey(e => e.Id);

        // Propiedades
        builder.Property(e => e.NombreCompleto)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Email)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(e => e.UsuarioNombre)
            .HasColumnName("UserName")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Contrasena)
            .HasColumnName("UserPass")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(e => e.FechaRegistro)
            .HasColumnName("FechaRegistracion")
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        builder.Property(e => e.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.RolId)
            .HasColumnName("IdRol")
            .IsRequired();

        // Relación con Rol (FK explícita porque Rol.IdRol no sigue la convención Id)
        builder.HasOne(u => u.Rol)
            .WithMany(r => r.Usuarios)
            .HasForeignKey(u => u.RolId)
            .HasPrincipalKey(r => r.IdRol)
            .OnDelete(DeleteBehavior.Restrict);

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