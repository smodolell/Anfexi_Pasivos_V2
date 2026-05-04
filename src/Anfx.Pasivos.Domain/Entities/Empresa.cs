namespace Anfx.Pasivos.Domain.Entities;

public class Empresa
{
    public int IdEmpresa { get; set; }

    public string? Empresa1 { get; set; }

    public string? RFC { get; set; }

    public string? RazonSocial { get; set; }

    public string? DireccionEmpresa { get; set; }

    public string? Telefono { get; set; }

    public string? Representante { get; set; }

    public string? AvisosEstadodeCuenta { get; set; }

    public string? AdvertenciasEstadodeCuenta { get; set; }

    public string? AclaracionesEstadodeCuenta { get; set; }

    public bool? UsaDesembolso { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<PSV_TipoCredito> PSV_TipoCreditos { get; set; } = new List<PSV_TipoCredito>();
}