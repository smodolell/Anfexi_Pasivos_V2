using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class PSV_EstatusContratoConfiguration : IEntityTypeConfiguration<PSV_EstatusContrato>
{
    public void Configure(EntityTypeBuilder<PSV_EstatusContrato> builder)
    {
        builder.ToTable("PSV_EstatusContrato");

        builder.HasKey(e => e.IdEstatusContrato);

        builder.Property(e => e.EstatusContrato)
            .IsRequired()
            .HasMaxLength(100);
    }
}
