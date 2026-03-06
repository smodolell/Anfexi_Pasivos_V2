using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class View_TipoCapitalizacionConfiguration : IEntityTypeConfiguration<View_TipoCapitalizacion>
{
    public void Configure(EntityTypeBuilder<View_TipoCapitalizacion> builder)
    {
        builder.HasKey(e => e.ID);
        builder.ToView("View_TipoCapitalizacion");
    }
}
