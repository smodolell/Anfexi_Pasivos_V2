using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class EstatusContratoConfiguration : IEntityTypeConfiguration<EstatusContrato>
{
    public void Configure(EntityTypeBuilder<EstatusContrato> builder)
    {
        builder.ToTable("EstatusContrato");

        builder.HasKey(e => e.IdEstatusContrato);

        builder.Property(e => e.EstatusContrato1)
            .HasColumnName("EstatusContrato")
            .IsRequired()
            .HasMaxLength(100);
    }
}
