using Anfx.Pasivos.Application.Common.Interfaces;
using Anfx.Pasivos.Application.Common.Models.StoredProcedures;
using Anfx.Pasivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Anfx.Pasivos.Infrastructure.Persistence;

public partial class ApplicationDbContext : DbContext,IApplicationDbContext
{
    // Propiedades existentes (ya implementadas)
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<TipoDireccion> TiposDirecciones => Set<TipoDireccion>();
    public DbSet<Colonia> Colonias => Set<Colonia>();
    public DbSet<PSV_Menu> PSV_Menu => Set<PSV_Menu>();

    // Implementación de todas las propiedades que faltaban
    public DbSet<EstatusContrato> EstatusContrato => Set<EstatusContrato>();
    public DbSet<Genero> Genero => Set<Genero>();
    public DbSet<SB_Periodicidad> SB_Periodicidad => Set<SB_Periodicidad>();
    public DbSet<SB_TipoMoneda> SB_TipoMoneda => Set<SB_TipoMoneda>();
    public DbSet<Tasa> Tasa => Set<Tasa>();
    public DbSet<TipoCalculoTasaVariable> TipoCalculoTasaVariable => Set<TipoCalculoTasaVariable>();
    public DbSet<TipoGeneracionComprobante> TipoGeneracionComprobante => Set<TipoGeneracionComprobante>();
    public DbSet<TipoMantenimiento> TipoMantenimiento => Set<TipoMantenimiento>();
    public DbSet<TipoMovimiento> TipoMovimiento => Set<TipoMovimiento>();
    public DbSet<PSV_Fondeador> PSV_Fondeador => Set<PSV_Fondeador>();
    public DbSet<PSV_Movimiento> PSV_Movimiento => Set<PSV_Movimiento>();
    public DbSet<PSV_Pago> PSV_Pago => Set<PSV_Pago>();
    public DbSet<PSV_RelPagoMovimiento> PSV_RelPagoMovimiento => Set<PSV_RelPagoMovimiento>();
    public DbSet<Contrato> Contrato => Set<Contrato>();
    public DbSet<PSV_Archivo> PSV_Archivo => Set<PSV_Archivo>();
    public DbSet<PSV_Parameter> PSV_Parameter => Set<PSV_Parameter>();
    public DbSet<Catalogo> Catalogo => Set<Catalogo>();
    public DbSet<Empresa> Empresa => Set<Empresa>();
    public DbSet<PSV_Bitacora> PSV_Bitacora => Set<PSV_Bitacora>();
    public DbSet<View_Menu> View_Menu => Set<View_Menu>();
    public DbSet<View_MenuRol> View_MenuRol => Set<View_MenuRol>();
    public DbSet<View_Rol> View_Rol => Set<View_Rol>();
    public DbSet<PSV_RelActivoPasivo> PSV_RelActivoPasivo => Set<PSV_RelActivoPasivo>();
    public DbSet<View_LineaCredito> View_LineaCredito => Set<View_LineaCredito>();
    public DbSet<PSV_EstatusContrato> PSV_EstatusContrato => Set<PSV_EstatusContrato>();
    public DbSet<View_EstatusContrato> View_EstatusContrato => Set<View_EstatusContrato>();
    public DbSet<TipoCredito> TipoCredito => Set<TipoCredito>();
    public DbSet<PSV_TipoCredito> PSV_TipoCredito => Set<PSV_TipoCredito>();
    public DbSet<View_TipoCredito> View_TipoCredito => Set<View_TipoCredito>();
    public DbSet<View_ContratoPasivo> View_ContratoPasivo => Set<View_ContratoPasivo>();
    public DbSet<PSV_LineaCredito> PSV_LineaCredito => Set<PSV_LineaCredito>();
    public DbSet<PSV_RelLineaCreditoTipoCredito> PSV_RelLineaCreditoTipoCredito => Set<PSV_RelLineaCreditoTipoCredito>();
    public DbSet<PSV_CuentaBancaria> PSV_CuentaBancaria => Set<PSV_CuentaBancaria>();
    public DbSet<PSV_TipoPago> PSV_TipoPago => Set<PSV_TipoPago>();
    public DbSet<View_TipoPago> View_TipoPago => Set<View_TipoPago>();
    public DbSet<PSV_Banco> PSV_Banco => Set<PSV_Banco>();
    public DbSet<View_Banco> View_Banco => Set<View_Banco>();
    public DbSet<View_CuentaBancaria> View_CuentaBancaria => Set<View_CuentaBancaria>();
    public DbSet<PSV_TipoTablaAmortizaPeriodicidad> PSV_TipoTablaAmortizaPeriodicidad => Set<PSV_TipoTablaAmortizaPeriodicidad>();
    public DbSet<PSV_ContratoPagoIrregular> PSV_ContratoPagoIrregular => Set<PSV_ContratoPagoIrregular>();
    public DbSet<View_TipoPagoCapital> View_TipoPagoCapital => Set<View_TipoPagoCapital>();
    public DbSet<PSV_TipoPagoCapital> PSV_TipoPagoCapital => Set<PSV_TipoPagoCapital>();
    public DbSet<PSV_Contrato> PSV_Contrato => Set<PSV_Contrato>();
    public DbSet<PSV_TipoCapitalizacion> PSV_TipoCapitalizacion => Set<PSV_TipoCapitalizacion>();
    public DbSet<View_TipoCapitalizacion> View_TipoCapitalizacion => Set<View_TipoCapitalizacion>();
    public DbSet<View_ContratosAsignados> View_ContratosAsignados => Set<View_ContratosAsignados>();
    public DbSet<PSV_TipoTablaAmortiza> PSV_TipoTablaAmortiza => Set<PSV_TipoTablaAmortiza>();
    public DbSet<View_TipoTablaAmortiza> View_TipoTablaAmortiza => Set<View_TipoTablaAmortiza>();
    public DbSet<PSV_TablaAmortiza> PSV_TablaAmortiza => Set<PSV_TablaAmortiza>();
    public DbSet<PSV_TipoTablaAmortizaTipoCapitalizacion> PSV_TipoTablaAmortizaTipoCapitalizacion => Set<PSV_TipoTablaAmortizaTipoCapitalizacion>();
    public DbSet<PSV_TipoTablaAmortizaTipoPagoCapital> PSV_TipoTablaAmortizaTipoPagoCapital => Set<PSV_TipoTablaAmortizaTipoPagoCapital>();
    public DbSet<View_CarteraActiva_PorVencer> View_CarteraActiva_PorVencer => Set<View_CarteraActiva_PorVencer>();
    public DbSet<View_CarteraActiva_Vencido> View_CarteraActiva_Vencido => Set<View_CarteraActiva_Vencido>();
    public DbSet<View_CarteraPasiva_PorVencer> View_CarteraPasiva_PorVencer => Set<View_CarteraPasiva_PorVencer>();
    public DbSet<View_CarteraPasiva_Vencido> View_CarteraPasiva_Vencido => Set<View_CarteraPasiva_Vencido>();
    public DbSet<View_RelActivoPasivo> View_RelActivoPasivo => Set<View_RelActivoPasivo>();
    public DbSet<View_Fondeador> View_Fondeador => Set<View_Fondeador>();
    public DbSet<View_PSV_PasivoAutocomple> View_PSV_PasivoAutocomple => Set<View_PSV_PasivoAutocomple>();
    public DbSet<PSV_TipoTerminacion> PSV_TipoTerminacion => Set<PSV_TipoTerminacion>();
    public DbSet<PSV_Terminacion> PSV_Terminacion => Set<PSV_Terminacion>();
    public DbSet<View_CarteraPasiva_PorVencerV2> View_CarteraPasiva_PorVencerV2 => Set<View_CarteraPasiva_PorVencerV2>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

   
}