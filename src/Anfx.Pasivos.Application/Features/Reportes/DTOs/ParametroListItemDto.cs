namespace Anfx.Pasivos.Application.Features.Reportes.DTOs;

public class ParametroListItemDto
{
    public Guid Id { get; set; }
    public string NomParametro { get; set; } = "";
    public string TipoDato { get; set; } = "";
    public string NomInput { get; set; } = "";
    public string TablaRef { get; set; } = "";
    public string ColumnaValor { get; set; } = "";
    public string ColumnaTexto { get; set; } = "";
    public string Display { get; set; } = "";
    public int Order { get; set; } = 0;
}
