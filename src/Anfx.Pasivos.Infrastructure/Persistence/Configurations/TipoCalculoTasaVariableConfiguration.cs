using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class TipoCalculoTasaVariableConfiguration : IEntityTypeConfiguration<TipoCalculoTasaVariable>
{
    public void Configure(EntityTypeBuilder<TipoCalculoTasaVariable> builder)
    {
        builder.ToTable("TipoCalculoTasaVariable");

        builder.HasKey(e => e.IdTipoCalculoTasaVariable);

        builder.Property(e => e.TipoCalculoTasaVariable1)
            .HasColumnName("TipoCalculoTasaVariable")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Proceso)
            .HasMaxLength(200);
    }
}
