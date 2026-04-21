namespace Anfx.Pasivos.Domain.Entities;

public class Consecutivo
{
    public string NombreTabla { get; set; } = null!;

    public int IdConsecutivo { get; set; }

    public DateTime? FecUltimoCambio { get; set; }
}