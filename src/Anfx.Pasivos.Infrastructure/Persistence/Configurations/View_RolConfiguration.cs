using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class View_RolConfiguration : IEntityTypeConfiguration<View_Rol>
{
    public void Configure(EntityTypeBuilder<View_Rol> builder)
    {
        builder.HasKey(e => e.ID);
        builder.ToView("View_Rol");
    }
}
