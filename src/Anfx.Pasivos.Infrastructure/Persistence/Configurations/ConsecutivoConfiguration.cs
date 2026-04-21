using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations
{
    public partial class ConsecutivoConfiguration : IEntityTypeConfiguration<Consecutivo>
    {
        public void Configure(EntityTypeBuilder<Consecutivo> entity)
        {
            entity.HasKey(e => e.NombreTabla).HasName("PK__Consecut__5BCFB3894AB81AF0");

            entity.ToTable("Consecutivo");

            entity.Property(e => e.NombreTabla)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FecUltimoCambio).HasColumnType("datetime");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Consecutivo> entity);
    }
}
