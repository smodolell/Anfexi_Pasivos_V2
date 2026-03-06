using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class PSV_CuentaBancariaConfiguration : IEntityTypeConfiguration<PSV_CuentaBancaria>
{
    public void Configure(EntityTypeBuilder<PSV_CuentaBancaria> builder)
    {
        builder.ToTable("PSV_CuentaBancaria");

        builder.HasKey(e => e.IdCuentaBancaria);

        builder.Property(e => e.CuentaBancaria)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.CLABE)
            .HasMaxLength(18);

        builder.HasOne(e => e.PSV_Banco)
            .WithMany(b => b.PSV_CuentaBancaria)
            .HasForeignKey(e => e.IdBanco)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
