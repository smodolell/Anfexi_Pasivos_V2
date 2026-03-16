using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class GeneroConfiguration : IEntityTypeConfiguration<Genero>
{
    public void Configure(EntityTypeBuilder<Genero> builder)
    {
        builder.ToTable("Genero");

        builder.HasKey(e => e.IdGenero);

        builder.Property(e => e.Titulo)
            .IsRequired()
            .HasMaxLength(50);

        // La entidad Usuario ya no tiene FK a Genero en el nuevo esquema.
        // Ignorar la colección para evitar shadow FK "GeneroIdGenero".
        builder.Ignore(e => e.Usuario);
    }
}
