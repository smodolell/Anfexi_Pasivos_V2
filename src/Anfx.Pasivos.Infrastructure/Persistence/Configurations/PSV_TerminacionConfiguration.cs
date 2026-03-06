using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class PSV_TerminacionConfiguration : IEntityTypeConfiguration<PSV_Terminacion>
{
    public void Configure(EntityTypeBuilder<PSV_Terminacion> builder)
    {
        builder.ToTable("PSV_Terminacion");

        builder.HasKey(e => e.IdTerminacion);

        builder.HasOne(e => e.PSV_Contrato)
            .WithMany(c => c.PSV_Terminacion)
            .HasForeignKey(e => e.IdContrato)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.PSV_TipoTerminacion)
            .WithMany(tt => tt.PSV_Terminacion)
            .HasForeignKey(e => e.IdTipoTerminacion)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
