using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class View_EstatusContratoConfiguration : IEntityTypeConfiguration<View_EstatusContrato>
{
    public void Configure(EntityTypeBuilder<View_EstatusContrato> builder)
    {
        builder.HasKey(e => e.ID);
        builder.ToView("View_EstatusContrato");
    }
}
