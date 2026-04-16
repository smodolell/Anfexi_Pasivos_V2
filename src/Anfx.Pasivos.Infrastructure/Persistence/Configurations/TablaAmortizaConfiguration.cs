using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public partial class TablaAmortizaConfiguration : IEntityTypeConfiguration<TablaAmortiza>
{
    public void Configure(EntityTypeBuilder<TablaAmortiza> entity)
    {
        entity.HasKey(e => e.IdTablaAmortiza).HasName("PK__TablaAmo__5B3029CF43A1090D");

        entity.ToTable("TablaAmortiza");

        entity.HasIndex(e => e.IdContrato, "IDX_TablaAmortiza");

        entity.HasIndex(e => new { e.IdContrato, e.VersionTabla, e.Procesado }, "IDX_TablaAmortiza_Capital");

        entity.HasIndex(e => new { e.IdTipoTabla, e.IdContrato, e.FecVencimiento }, "IDX_TablaAmortiza_IdTipoTabla_IdContrato_FecVencimiento");

        entity.HasIndex(e => e.Procesado, "IDX_TablaAmortiza_Procesado");

        entity.Property(e => e.IdTablaAmortiza).ValueGeneratedNever();
        entity.Property(e => e.Capital).HasColumnType("decimal(13, 2)");
        entity.Property(e => e.FecVencimiento).HasColumnType("datetime");
        entity.Property(e => e.IVA).HasColumnType("decimal(13, 2)");
        entity.Property(e => e.Interes).HasColumnType("decimal(13, 2)");
        entity.Property(e => e.Total).HasColumnType("decimal(13, 2)");

        entity.HasOne(d => d.Contrato).WithMany(p => p.TablaAmortizas)
            .HasForeignKey(d => d.IdContrato)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_TablaAmortiza_Contrato");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<TablaAmortiza> entity);
}
