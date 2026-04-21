namespace Anfx.Pasivos.Application.Features.Reportes.DTOs;

public class ReporteExecuteParametroDto
{
    public Guid ParametroId { get; set; }
    public int InputId { get; set; }
    public string NomParametro { get; set; } = "";
    public string TipoDato { get; set; } = "";
    public bool ValueBoolean { get; set; }
    public string Value { get; set; } = "";
    public DateTime? ValueDateTime { get; set; }
    public string TablaRef { get; set; } = "";
    public string ColumnaValor { get; set; } = "";
    public string ColumnaTexto { get; set; } = "";
    public string Display { get; set; } = "";
    public int Order { get; set; }
    public List<SelectListItemDto> DropDownList { get; set; } = [];
}
