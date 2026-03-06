using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations
{
    public class ColoniaConfiguration : IEntityTypeConfiguration<Colonia>
    {
        public void Configure(EntityTypeBuilder<Colonia> builder)
        {
            builder.HasKey(c => c.Id);
            builder.ToTable("Colonia");
            
            builder.Property(c => c.sColonia)
                .HasColumnName("Colonia")
                .IsRequired()
                .HasMaxLength(200);
                
            builder.Property(c => c.Estado)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(c => c.Municipio)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(c => c.CodigoPostal)
                .IsRequired()
                .HasMaxLength(10);
        }
    }
}
