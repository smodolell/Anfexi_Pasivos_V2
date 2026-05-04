namespace Anfx.Pasivos.Domain.Entities;

public class Contrato
{
    public int IdContrato { get; set; }

    public string Contrato1 { get; set; } = string.Empty;

    public int? IdTipoCredito { get; set; }

    public int? IdEstatusContrato { get; set; }

    public decimal? Capital { get; set; }

    public int IdPeriodicidad { get; set; }

    public int? Plazo { get; set; }

    public DateTime? FecActivacion { get; set; }

    public decimal? Tasa { get; set; }

    public decimal? TasaMora { get; set; }

    public decimal? TasaIva { get; set; }

    public int? VersionTabla { get; set; }

    public bool CapturaManualTAPasiva { get; set; }

    public virtual EstatusContrato EstatusContrato { get; set; } = null!;

    public virtual TipoCredito TipoCredito { get; set; } = null!;

    public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();

    public virtual ICollection<PSV_ContratoMinistracione> PSV_ContratoMinistraciones { get; set; } = new List<PSV_ContratoMinistracione>();

    public virtual ICollection<PSV_RelActivoPasivo> PSV_RelActivoPasivo { get; set; } = new List<PSV_RelActivoPasivo>();

    public virtual ICollection<TablaAmortiza> TablaAmortizas { get; set; } = new List<TablaAmortiza>();
}
