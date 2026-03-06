using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class View_TipoCreditoConfiguration : IEntityTypeConfiguration<View_TipoCredito>
{
    public void Configure(EntityTypeBuilder<View_TipoCredito> builder)
    {
        builder.HasKey(e => e.ID);
        builder.ToView("View_TipoCredito");
    }
}
