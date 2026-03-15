using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class RolConfiguration : IEntityTypeConfiguration<Rol>
{
    public void Configure(EntityTypeBuilder<Rol> builder)
    {
        builder.HasKey(e => e.IdRol);

        builder.ToTable("Rol");

        builder.Property(e => e.Descripcion)
            .HasMaxLength(200)
            .IsUnicode(false);
        builder.Property(e => e.Titulo)
            .HasMaxLength(50)
            .IsUnicode(false);
    }
}
