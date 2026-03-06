using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class View_MenuConfiguration : IEntityTypeConfiguration<View_Menu>
{
    public void Configure(EntityTypeBuilder<View_Menu> builder)
    {
        builder.HasKey(e => e.ID);
        builder.ToView("View_Menu");
    }
}
