namespace Anfx.Pasivos.Application.Features.Catalogos.DTOs;

public class TasaVariableListItemDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal? UltimoValor { get; set; }
    public DateTime? FecUltimoValor { get; set; }
}
