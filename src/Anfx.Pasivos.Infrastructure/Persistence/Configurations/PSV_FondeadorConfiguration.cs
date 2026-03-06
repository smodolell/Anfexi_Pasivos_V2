using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class PSV_FondeadorConfiguration : IEntityTypeConfiguration<PSV_Fondeador>
{
    public void Configure(EntityTypeBuilder<PSV_Fondeador> builder)
    {
        builder.ToTable("PSV_Fondeador");

        builder.HasKey(e => e.IdFondeador);

        builder.Property(e => e.Fondeador)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.ClaveCuentaContable)
            .HasMaxLength(50);
    }
}
