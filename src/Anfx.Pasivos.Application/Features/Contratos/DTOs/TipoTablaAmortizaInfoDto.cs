namespace Anfx.Pasivos.Application.Features.Contratos.DTOs;

public class TipoTablaAmortizaInfoDto
{
    public bool EsCapitalizable { get; set; }
    public string Error { get; set; } = string.Empty;
    public List<SelectItemDto> TipoCapitalizacion { get; set; } = new();
    public List<SelectItemDto> TipoPagoCapital { get; set; } = new();
}
