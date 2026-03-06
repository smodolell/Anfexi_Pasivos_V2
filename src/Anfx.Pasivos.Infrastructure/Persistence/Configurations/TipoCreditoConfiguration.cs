using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class TipoCreditoConfiguration : IEntityTypeConfiguration<TipoCredito>
{
    public void Configure(EntityTypeBuilder<TipoCredito> builder)
    {
        builder.ToTable("TipoCredito");

        builder.HasKey(e => e.IdTipoCredito);

        builder.Property(e => e.TipoCredito1)
            .HasColumnName("TipoCredito")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.ClaveTipoCredito)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasOne(e => e.Empresa)
            .WithMany()
            .HasForeignKey(e => e.IdEmpresa)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.TipoMovimiento)
            .WithMany(tm => tm.TipoCredito)
            .HasForeignKey(e => e.IdTipoMovimiento)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.TipoMovimiento1)
            .WithMany(tm => tm.TipoCredito1)
            .HasForeignKey(e => e.IdTipoMovimientoEnganche)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.TipoMovimiento2)
            .WithMany(tm => tm.TipoCredito2)
            .HasForeignKey(e => e.IdTipoMovimientoBallon)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.TipoMovimiento3)
            .WithMany(tm => tm.TipoCredito3)
            .HasForeignKey(e => e.IdTipoMovimientoValorResidual)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.TipoMovimiento4)
            .WithMany(tm => tm.TipoCredito4)
            .HasForeignKey(e => e.IdTipoMovimientoDeposito)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.TipoMovimiento5)
            .WithMany(tm => tm.TipoCredito5)
            .HasForeignKey(e => e.IdTipoMovimientoOpcion)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.TipoMovimiento6)
            .WithMany(tm => tm.TipoCredito6)
            .HasForeignKey(e => e.IdTipoMovimientoMora)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.TipoMovimiento7)
            .WithMany(tm => tm.TipoCredito7)
            .HasForeignKey(e => e.IdTipoMovimientoGastos)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.TipoMovimiento8)
            .WithMany(tm => tm.TipoCredito8)
            .HasForeignKey(e => e.IdTipoMovimientoCancelCheque)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.TipoMovimiento9)
            .WithMany(tm => tm.TipoCredito9)
            .HasForeignKey(e => e.IdTipoMovimientoNoDevuelto)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.TipoMovimiento10)
            .WithMany(tm => tm.TipoCredito10)
            .HasForeignKey(e => e.IdTipoMovimientoSeguro)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
