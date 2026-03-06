using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class PSV_TipoCreditoConfiguration : IEntityTypeConfiguration<PSV_TipoCredito>
{
    public void Configure(EntityTypeBuilder<PSV_TipoCredito> builder)
    {
        builder.ToTable("PSV_TipoCredito");

        builder.HasKey(e => e.IdTipoCredito);

        builder.Property(e => e.TipoCredito)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Prefijo)
            .HasMaxLength(10);

        builder.Property(e => e.Sufijo)
            .HasMaxLength(10);

        builder.Property(e => e.Activo)
            .IsRequired();

        builder.HasOne(e => e.TipoMovimiento)
            .WithMany(m => m.PSV_TipoCredito)
            .HasForeignKey(e => e.IdTipoMovimiento)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.TipoMovimiento1)
            .WithMany(m => m.PSV_TipoCredito1)
            .HasForeignKey(e => e.IdTipoMovimiento_Mora)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.PSV_TipoTablaAmortiza)
            .WithMany(t => t.PSV_TipoCredito)
            .HasForeignKey(e => e.IdTipoTablaAmortiza)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(e => e.Empresa)
            .WithMany()
            .HasForeignKey(e => e.IdEmpresa)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
