using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class View_TipoPagoCapitalConfiguration : IEntityTypeConfiguration<View_TipoPagoCapital>
{
    public void Configure(EntityTypeBuilder<View_TipoPagoCapital> builder)
    {
        builder.HasKey(e => e.ID);
        builder.ToView("View_TipoPagoCapital");
    }
}
