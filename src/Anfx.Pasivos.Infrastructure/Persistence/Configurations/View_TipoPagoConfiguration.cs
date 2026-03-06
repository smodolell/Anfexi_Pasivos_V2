using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class View_TipoPagoConfiguration : IEntityTypeConfiguration<View_TipoPago>
{
    public void Configure(EntityTypeBuilder<View_TipoPago> builder)
    {
        builder.HasKey(e => e.ID);
        builder.ToView("View_TipoPago");
    }
}
