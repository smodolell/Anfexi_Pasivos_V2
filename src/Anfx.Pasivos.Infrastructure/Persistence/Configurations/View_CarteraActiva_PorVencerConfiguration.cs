using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class View_CarteraActiva_PorVencerConfiguration : IEntityTypeConfiguration<View_CarteraActiva_PorVencer>
{
    public void Configure(EntityTypeBuilder<View_CarteraActiva_PorVencer> builder)
    {
        builder.HasNoKey();
        builder.ToView("View_CarteraActiva_PorVencer");
    }
}
