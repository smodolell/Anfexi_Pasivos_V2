namespace Anfx.Pasivos.Application.Features.Catalogos.DTOs;

public class TasaValorListItemDto
{
    public int Id { get; set; }
    public decimal? ValorTasa { get; set; }
    public DateTime? FecValorTasa { get; set; }
    public DateTime? FecRegistroTasa { get; set; }
}
