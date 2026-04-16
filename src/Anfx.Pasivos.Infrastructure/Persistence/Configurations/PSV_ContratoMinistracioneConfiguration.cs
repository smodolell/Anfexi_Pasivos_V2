using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public partial class PSV_ContratoMinistracioneConfiguration : IEntityTypeConfiguration<PSV_ContratoMinistracione>
{
    public void Configure(EntityTypeBuilder<PSV_ContratoMinistracione> entity)
    {
        entity.HasKey(e => e.IdContratoMinistraciones).HasName("PK_ContratoMinistraciones");

        entity.Property(e => e.FechaDispersion).HasColumnType("datetime");
        entity.Property(e => e.FechaFondeo).HasColumnType("datetime");
        entity.Property(e => e.MontoDeposito).HasColumnType("decimal(22, 2)");
        entity.Property(e => e.MontoDispersion).HasColumnType("decimal(22, 2)");

        entity.HasOne(d => d.IdContratoNavigation).WithMany(p => p.PSV_ContratoMinistraciones)
            .HasForeignKey(d => d.IdContrato)
            .HasConstraintName("FK_Contrato_ContratoMinistraciones");

        entity.HasOne(d => d.IdCuentaDepositoNavigation).WithMany(p => p.PSV_ContratoMinistraciones)
            .HasForeignKey(d => d.IdCuentaDeposito)
            .HasConstraintName("FK_PSV_FondeadoresCuentas_ContratoMinistraciones");

        entity.HasOne(d => d.IdCuentaDispercionNavigation).WithMany(p => p.PSV_ContratoMinistraciones)
            .HasForeignKey(d => d.IdCuentaDispercion)
            .HasConstraintName("FK_Cat_CuentasDispersion_ContratoMinistraciones");

        entity.HasOne(d => d.IdFondeadorNavigation)
            //.WithMany(p => p.PSV_ContratoMinistraciones)
            .WithMany()
            .HasForeignKey(d => d.IdFondeador)
            .HasConstraintName("FK_Fondeador_ContratoMinistraciones");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<PSV_ContratoMinistracione> entity);
}
