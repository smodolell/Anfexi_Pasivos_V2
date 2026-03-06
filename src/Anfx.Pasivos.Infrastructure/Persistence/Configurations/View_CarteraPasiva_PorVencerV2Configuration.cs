using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class View_CarteraPasiva_PorVencerV2Configuration : IEntityTypeConfiguration<View_CarteraPasiva_PorVencerV2>
{
    public void Configure(EntityTypeBuilder<View_CarteraPasiva_PorVencerV2> builder)
    {
        builder.HasNoKey();
        builder.ToView("View_CarteraPasiva_PorVencerV2");
    }
}
