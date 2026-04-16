using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public partial class TasaValorConfiguration : IEntityTypeConfiguration<TasaValor>
{
    public void Configure(EntityTypeBuilder<TasaValor> entity)
    {
        entity.HasKey(e => e.IdTasaValor).HasName("PK__TasaValo__3B2E06A12865639A");

        entity.ToTable("TasaValor");

        entity.Property(e => e.FecRegistroTasa).HasColumnType("datetime");
        entity.Property(e => e.FecValorTasa).HasColumnType("datetime");
        entity.Property(e => e.ValorTasa).HasColumnType("decimal(11, 6)");

        entity.HasOne(d => d.Tasa).WithMany(p => p.TasaValors)
            .HasForeignKey(d => d.IdTasa)
            .HasConstraintName("FK__TasaValor__IdTas__310FB98B");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<TasaValor> entity);
}
