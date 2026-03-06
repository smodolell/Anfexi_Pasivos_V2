using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class View_ContratosAsignadosConfiguration : IEntityTypeConfiguration<View_ContratosAsignados>
{
    public void Configure(EntityTypeBuilder<View_ContratosAsignados> builder)
    {
        builder.HasKey(e => e.IdContrato);
        builder.ToView("View_ContratosAsignados");
    }
}
