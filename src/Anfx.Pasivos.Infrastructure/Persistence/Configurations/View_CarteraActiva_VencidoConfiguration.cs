using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class View_CarteraActiva_VencidoConfiguration : IEntityTypeConfiguration<View_CarteraActiva_Vencido>
{
    public void Configure(EntityTypeBuilder<View_CarteraActiva_Vencido> builder)
    {
        builder.HasNoKey();
        builder.ToView("View_CarteraActiva_Vencido");
    }
}
