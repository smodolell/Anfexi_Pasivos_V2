namespace Anfx.Pasivos.Domain.Entities;

public partial class PSV_ContratoMinistracione
{
    public int IdContratoMinistraciones { get; set; }

    public int? IdContrato { get; set; }

    public int? IdFondeador { get; set; }

    public int? IdCuentaDeposito { get; set; }

    public decimal? MontoDeposito { get; set; }

    public DateTime? FechaFondeo { get; set; }

    public int? IdCuentaDispercion { get; set; }

    public decimal? MontoDispersion { get; set; }

    public DateTime? FechaDispersion { get; set; }

    public virtual Contrato Contrato { get; set; }

    public virtual PSV_FondeadoresCuenta FondeadoresCuenta { get; set; }

    public virtual Cat_CuentasDispersion CuentasDispersion { get; set; }

    public virtual PSV_Fondeador Fondeador { get; set; }
}
