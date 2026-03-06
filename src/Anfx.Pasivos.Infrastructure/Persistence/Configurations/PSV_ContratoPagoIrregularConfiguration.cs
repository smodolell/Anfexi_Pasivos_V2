using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class PSV_ContratoPagoIrregularConfiguration : IEntityTypeConfiguration<PSV_ContratoPagoIrregular>
{
    public void Configure(EntityTypeBuilder<PSV_ContratoPagoIrregular> builder)
    {
        builder.ToTable("PSV_ContratoPagoIrregular");

        builder.HasKey(e => new { e.IdContrato, e.NoPago, e.VersionTabla });

        builder.HasOne(e => e.PSV_Contrato)
            .WithMany(c => c.PSV_ContratoPagoIrregular)
            .HasForeignKey(e => e.IdContrato)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
