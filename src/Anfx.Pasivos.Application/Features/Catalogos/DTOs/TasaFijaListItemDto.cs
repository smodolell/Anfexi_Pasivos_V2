namespace Anfx.Pasivos.Application.Features.Catalogos.DTOs;

public class TasaFijaListItemDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal? ValorTasa { get; set; }
    public DateTime? FecTasa { get; set; }
}
