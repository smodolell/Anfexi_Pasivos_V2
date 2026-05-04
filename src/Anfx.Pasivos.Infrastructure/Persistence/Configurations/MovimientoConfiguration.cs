using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class MovimientoConfiguration : IEntityTypeConfiguration<Movimiento>
{
    public void Configure(EntityTypeBuilder<Movimiento> entity)
    {
        entity.HasKey(e => e.IdMovimiento).HasName("PK__Movimien__881A6AE03BFFE745");

        entity.ToTable("Movimiento");

        entity.HasIndex(e => new { e.IdContrato, e.FecMovimiento }, "IDX_Movimiento_IdContrato_FecMovimiento");

        entity.HasIndex(e => new { e.SaldoTotal, e.FecMovimiento }, "IDX_Movimiento_SaldoTotal_FecMovimiento");

        entity.Property(e => e.IdMovimiento).ValueGeneratedNever();
        entity.Property(e => e.Capital)
            .HasDefaultValue(0m)
            .HasColumnType(ApplicationDbContextConstants.Decimal_13_2);
        entity.Property(e => e.FecMovimiento).HasColumnType("datetime");
        entity.Property(e => e.FecUltimoCambio).HasColumnType("datetime");
        entity.Property(e => e.IVA)
            .HasDefaultValue(0m)
            .HasColumnType(ApplicationDbContextConstants.Decimal_13_2);
        entity.Property(e => e.Interes)
            .HasDefaultValue(0m)
            .HasColumnType(ApplicationDbContextConstants.Decimal_13_2);
        entity.Property(e => e.SaldoCapital)
            .HasDefaultValue(0m)
            .HasColumnType(ApplicationDbContextConstants.Decimal_13_2);
        entity.Property(e => e.SaldoIVA)
            .HasDefaultValue(0m)
            .HasColumnType(ApplicationDbContextConstants.Decimal_13_2);
        entity.Property(e => e.SaldoInteres)
            .HasDefaultValue(0m)
            .HasColumnType(ApplicationDbContextConstants.Decimal_13_2);
        entity.Property(e => e.SaldoTotal)
            .HasDefaultValue(0m)
            .HasColumnType(ApplicationDbContextConstants.Decimal_13_2);
        entity.Property(e => e.Total)
            .HasDefaultValue(0m)
            .HasColumnType(ApplicationDbContextConstants.Decimal_13_2);

        entity.HasOne(d => d.Contrato)
            .WithMany(p => p.Movimientos)
            .HasForeignKey(d => d.IdContrato)
            .HasConstraintName("FK_Movimiento_Contrato");

    }

}
