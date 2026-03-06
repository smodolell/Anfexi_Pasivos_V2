using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class PSV_TablaAmortizaConfiguration : IEntityTypeConfiguration<PSV_TablaAmortiza>
{
    public void Configure(EntityTypeBuilder<PSV_TablaAmortiza> builder)
    {
        builder.ToTable("PSV_TablaAmortiza");

        builder.HasKey(e => e.IdTablaAmortiza);

        builder.HasOne(e => e.PSV_Contrato)
            .WithMany(c => c.PSV_TablaAmortiza)
            .HasForeignKey(e => e.IdContrato)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
