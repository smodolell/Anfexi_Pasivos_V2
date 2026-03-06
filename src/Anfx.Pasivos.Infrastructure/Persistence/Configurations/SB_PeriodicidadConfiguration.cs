using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class SB_PeriodicidadConfiguration : IEntityTypeConfiguration<SB_Periodicidad>
{
    public void Configure(EntityTypeBuilder<SB_Periodicidad> builder)
    {
        builder.ToTable("SB_Periodicidad");

        builder.HasKey(e => e.IdPeriodicidad);

        builder.Property(e => e.CveCortaPeriodicidad)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(e => e.DescPeriodicidad)
            .IsRequired()
            .HasMaxLength(100);


  
    }
}
