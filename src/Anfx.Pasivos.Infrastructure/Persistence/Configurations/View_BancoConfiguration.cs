using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class View_BancoConfiguration : IEntityTypeConfiguration<View_Banco>
{
    public void Configure(EntityTypeBuilder<View_Banco> builder)
    {
        builder.HasNoKey();
        builder.ToView("View_Banco");
    }
}
