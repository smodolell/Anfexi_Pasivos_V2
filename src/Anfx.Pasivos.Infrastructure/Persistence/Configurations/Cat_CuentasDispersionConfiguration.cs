using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

#pragma warning disable S101
public class Cat_CuentasDispersionConfiguration : IEntityTypeConfiguration<Cat_CuentasDispersion>
#pragma warning restore S101
{
    public void Configure(EntityTypeBuilder<Cat_CuentasDispersion> entity)
    {
        entity.HasKey(e => e.IdCuentaDispersion);

        entity.ToTable("Cat_CuentasDispersion");

        entity.Property(e => e.CuentaDispersion).IsUnicode(false);

    }

}
