using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class View_TipoTablaAmortizaConfiguration : IEntityTypeConfiguration<View_TipoTablaAmortiza>
{
    public void Configure(EntityTypeBuilder<View_TipoTablaAmortiza> builder)
    {
        builder.HasKey(e => e.ID);
        builder.ToView("View_TipoTablaAmortiza");
    }
}
