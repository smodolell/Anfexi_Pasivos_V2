using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class View_FondeadorConfiguration : IEntityTypeConfiguration<View_Fondeador>
{
    public void Configure(EntityTypeBuilder<View_Fondeador> builder)
    {
        builder.HasKey(e => e.ID);
        builder.ToView("View_Fondeador");
    }
}
