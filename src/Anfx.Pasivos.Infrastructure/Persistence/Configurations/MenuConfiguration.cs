using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class MenuConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        // Mapeo de tabla y esquema
        builder.ToTable("Menu");

        // Configuración de la llave primaria
        // Asumiendo que hay una propiedad Id (aunque no está en tu clase)
        // Si no existe, deberías agregarla. Por ahora asumimos que existe
        builder.HasKey(e => e.Id);

        // Relación auto-referenciada para el menú padre
        builder.HasOne<Menu>()
            .WithMany()
            .HasForeignKey(e => e.MenuId_Padre)
            .OnDelete(DeleteBehavior.Restrict);

        // Propiedades
        builder.Property(e => e.MenuId_Padre)
            .IsRequired(false);

        builder.Property(e => e.sMenu)
            .HasColumnName("Menu")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Area)
            .HasMaxLength(100); // Sugerencia: agregar longitud máxima

        builder.Property(e => e.Controlador)
            .HasMaxLength(100); // Sugerencia: agregar longitud máxima

        builder.Property(e => e.Accion)
            .HasMaxLength(100); // Sugerencia: agregar longitud máxima

        builder.Property(e => e.Icono)
            .HasMaxLength(100); // Sugerencia: agregar longitud máxima

        builder.Property(e => e.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        // Índices recomendados
        builder.HasIndex(e => e.MenuId_Padre)
            .HasDatabaseName("IX_Menu_MenuId_Padre");

        builder.HasIndex(e => e.Activo)
            .HasDatabaseName("IX_Menu_Activo");

        // Índice compuesto para búsquedas por área/controlador/acción
        builder.HasIndex(e => new { e.Area, e.Controlador, e.Accion })
            .HasDatabaseName("IX_Menu_Area_Controlador_Accion")
            .IsUnique(false); // Cambiar a true si debe ser único
    }
}
