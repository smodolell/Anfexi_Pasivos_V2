using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

#pragma warning disable S101
public class PSV_ContratoMinistracioneConfiguration : IEntityTypeConfiguration<PSV_ContratoMinistracione>
#pragma warning restore S101
{
    public void Configure(EntityTypeBuilder<PSV_ContratoMinistracione> entity)
    {
        entity.HasKey(e => e.IdContratoMinistraciones).HasName("PK_ContratoMinistraciones");

        entity.Property(e => e.FechaDispersion).HasColumnType("datetime");
        entity.Property(e => e.FechaFondeo).HasColumnType("datetime");
        entity.Property(e => e.MontoDeposito).HasColumnType(ApplicationDbContextConstants.Decimal_22_2);
        entity.Property(e => e.MontoDispersion).HasColumnType(ApplicationDbContextConstants.Decimal_22_2);

        entity.HasOne(d => d.Contrato).WithMany(p => p.PSV_ContratoMinistraciones)
            .HasForeignKey(d => d.IdContrato)
            .HasConstraintName("FK_Contrato_ContratoMinistraciones");

        entity.HasOne(d => d.CuentasDispersion).WithMany(p => p.PSV_ContratoMinistraciones)
            .HasForeignKey(d => d.IdCuentaDeposito)
            .HasConstraintName("FK_PSV_FondeadoresCuentas_ContratoMinistraciones");

        entity.HasOne(d => d.CuentasDispersion).WithMany(p => p.PSV_ContratoMinistraciones)
            .HasForeignKey(d => d.IdCuentaDispercion)
            .HasConstraintName("FK_Cat_CuentasDispersion_ContratoMinistraciones");

        entity.HasOne(d => d.Fondeador)
            //.WithMany(p => p.PSV_ContratoMinistraciones)
            .WithMany()
            .HasForeignKey(d => d.IdFondeador)
            .HasConstraintName("FK_Fondeador_ContratoMinistraciones");

    }

}
