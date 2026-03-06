using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class View_CuentaBancariaConfiguration : IEntityTypeConfiguration<View_CuentaBancaria>
{
    public void Configure(EntityTypeBuilder<View_CuentaBancaria> builder)
    {
        builder.HasKey(e => e.ID);
        builder.ToView("View_CuentaBancaria");
    }
}
