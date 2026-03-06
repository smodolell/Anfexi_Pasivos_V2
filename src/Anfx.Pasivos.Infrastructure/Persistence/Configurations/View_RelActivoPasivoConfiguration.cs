using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class View_RelActivoPasivoConfiguration : IEntityTypeConfiguration<View_RelActivoPasivo>
{
    public void Configure(EntityTypeBuilder<View_RelActivoPasivo> builder)
    {
        builder.HasKey(e => e.ID);
        builder.ToView("View_RelActivoPasivo");
    }
}
