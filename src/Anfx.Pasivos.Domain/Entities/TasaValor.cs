
namespace Anfx.Pasivos.Domain.Entities;

public class TasaValor
{
    public int IdTasaValor { get; set; }

    public int? IdTasa { get; set; }

    public decimal? ValorTasa { get; set; }

    public DateTime? FecValorTasa { get; set; }

    public DateTime? FecRegistroTasa { get; set; }

    public virtual Tasa Tasa { get; set; } = null!;
}