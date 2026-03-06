using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class View_ContratoPasivoConfiguration : IEntityTypeConfiguration<View_ContratoPasivo>
{
    public void Configure(EntityTypeBuilder<View_ContratoPasivo> builder)
    {
        builder.HasKey(e => e.ID);
        builder.ToView("View_ContratoPasivo");
    }
}
