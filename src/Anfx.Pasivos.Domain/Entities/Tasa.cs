namespace Anfx.Pasivos.Domain.Entities;

public class Tasa
{
    public int IdTasa { get; set; }
    public string Tasa1 { get; set; } = string.Empty;
    public decimal? ValorTasa { get; set; }
    public DateTime? FecTasa { get; set; }
    public bool? EsVariable { get; set; }
    public bool Activo { get; set; }
    
    public virtual ICollection<PSV_LineaCredito> PSV_LineaCredito { get; set; } = new HashSet<PSV_LineaCredito>();
    public virtual ICollection<PSV_Contrato> PSV_Contrato { get; set; } = new HashSet<PSV_Contrato>();
    public virtual ICollection<PSV_Contrato> PSV_Contrato1 { get; set; } = new HashSet<PSV_Contrato>();
    public virtual ICollection<TasaValor> TasaValors { get; set; } = new HashSet<TasaValor>();
}
