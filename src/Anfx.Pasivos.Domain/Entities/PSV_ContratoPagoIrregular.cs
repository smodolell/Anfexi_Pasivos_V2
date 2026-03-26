namespace Anfx.Pasivos.Domain.Entities;

public partial class PSV_ContratoPagoIrregular
{
    public int NoPago { get; set; }
    public int IdContrato { get; set; }
    public decimal Capital { get; set; }
    public DateTime FecVencimiento { get; set; }
    public int VersionTabla { get; set; }
    public bool NoAplicaCapital { get; set; }
    public bool Procesado { get; set; }

    public virtual PSV_Contrato PSV_Contrato { get; set; }
}
