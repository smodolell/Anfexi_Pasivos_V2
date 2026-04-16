using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public partial class Cat_CuentasDispersionConfiguration : IEntityTypeConfiguration<Cat_CuentasDispersion>
{
    public void Configure(EntityTypeBuilder<Cat_CuentasDispersion> entity)
    {
        entity.HasKey(e => e.IdCuentaDispersion);

        entity.ToTable("Cat_CuentasDispersion");

        entity.Property(e => e.CuentaDispersion).IsUnicode(false);

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<Cat_CuentasDispersion> entity);
}
