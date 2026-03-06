using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class ContratoConfiguration : IEntityTypeConfiguration<Contrato>
{
    public void Configure(EntityTypeBuilder<Contrato> builder)
    {
        builder.ToTable("Contrato");

        builder.HasKey(e => e.IdContrato);

        builder.Property(e => e.Contrato1)
            .HasColumnName("Contrato")
            .IsRequired()
            .HasMaxLength(50);

        builder.HasOne(e => e.EstatusContrato)
            .WithMany(es => es.Contrato)
            .HasForeignKey(e => e.IdEstatusContrato)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.SB_Periodicidad)
            .WithMany(p => p.Contrato)
            .HasForeignKey(e => e.IdPeriodicidad)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.SB_Periodicidad1)
            .WithMany(p => p.Contrato1)
            .HasForeignKey(c => c.IdPeriodicidad_TTA)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.SB_Periodicidad2)
            .WithMany(p => p.Contrato2)
            .HasForeignKey(c => c.IdPeriodicidadTC)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.SB_TipoMoneda)
            .WithMany(m => m.Contrato)
            .HasForeignKey(e => e.IdMoneda)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Tasa1)
            .WithMany(t => t.Contrato)
            .HasForeignKey(e => e.IdTasa)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.TipoCredito)
            .WithMany()
            .HasForeignKey(e => e.IdTipoCredito)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
