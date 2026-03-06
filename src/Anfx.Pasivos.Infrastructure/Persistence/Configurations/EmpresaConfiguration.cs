using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anfx.Pasivos.Infrastructure.Persistence.Configurations;

public class EmpresaConfiguration : IEntityTypeConfiguration<Empresa>
{
    public void Configure(EntityTypeBuilder<Empresa> builder)
    {
        // Mapeo de tabla y esquema
        builder.ToTable("Empresa");

        // Llave primaria
        builder.HasKey(e => e.Id);

        // Propiedades
        builder.Property(e => e.sEmpresa)
            .HasColumnName("Empresa")
            .HasMaxLength(180)
            .IsRequired();

        builder.Property(e => e.RFC)
            .HasMaxLength(13);

        builder.Property(e => e.RazonSocial)
            .HasMaxLength(180);

        builder.Property(e => e.Telefono)
            .HasMaxLength(12);

        builder.Property(e => e.Representante)
            .HasMaxLength(150);

        builder.Property(e => e.AvisosEstadodeCuenta)
            .HasColumnType("text");

        builder.Property(e => e.AdvertenciasEstadodeCuenta)
            .HasColumnType("text");

        builder.Property(e => e.AclaracionesEstadodeCuenta)
            .HasColumnType("text");

        builder.Property(e => e.UsaDesembolso)
            .IsRequired();

        builder.Property(e => e.Pasivo)
            .IsRequired();

        builder.Property(e => e.TipoDireccionId)
            .IsRequired();

        builder.Property(e => e.Calle)
            .HasMaxLength(200);

        builder.Property(e => e.NumExterior)
            .HasMaxLength(20);

        builder.Property(e => e.NumInterior)
            .HasMaxLength(20);

        builder.Property(e => e.ColoniaId)
            .IsRequired();

        // Índices (opcional, según necesidades)
        builder.HasIndex(e => e.RFC)
            .HasDatabaseName("IX_Empresa_RFC");

        builder.HasIndex(e => e.RazonSocial)
            .HasDatabaseName("IX_Empresa_RazonSocial");
    }
}
