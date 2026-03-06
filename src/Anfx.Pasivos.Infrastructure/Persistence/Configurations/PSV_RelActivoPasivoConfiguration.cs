using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class PSV_RelActivoPasivoConfiguration : IEntityTypeConfiguration<PSV_RelActivoPasivo>
{
    public void Configure(EntityTypeBuilder<PSV_RelActivoPasivo> builder)
    {
        builder.ToTable("PSV_RelActivoPasivo");

        builder.HasKey(e => new { e.IdContratoActivo, e.IdContratoPasivo });

        builder.HasOne(e => e.Contrato)
            .WithMany(c => c.PSV_RelActivoPasivo)
            .HasForeignKey(e => e.IdContratoActivo)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.PSV_Contrato)
            .WithMany(c => c.PSV_RelActivoPasivo)
            .HasForeignKey(e => e.IdContratoPasivo)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
