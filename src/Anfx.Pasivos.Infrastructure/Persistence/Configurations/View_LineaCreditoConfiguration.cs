using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class View_LineaCreditoConfiguration : IEntityTypeConfiguration<View_LineaCredito>
{
    public void Configure(EntityTypeBuilder<View_LineaCredito> builder)
    {
        builder.HasKey(e => e.ID);
        builder.ToView("View_LineaCredito");
    }
}
