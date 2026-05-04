namespace Anfx.Pasivos.Application.Features.Reportes.DTOs;

public class ParametroEditDto
{
    public Guid ParametroId { get; set; }
    public int ReporteId { get; set; }
    public int? InputId { get; set; }
    public string NomParametro { get; set; } = "";
    public string TipoDato { get; set; } = "";
    public string? TablaRef { get; set; } = "";
    public string? ColumnaValor { get; set; } = "";
    public string? ColumnaTexto { get; set; } = "";
    public string Display { get; set; } = "";
    public int Order { get; set; } = 0;
}

public class ParametroEditDtoValidator : AbstractValidator<ParametroEditDto>
{
    public ParametroEditDtoValidator()
    {
        RuleFor(r => r.NomParametro)
            .NotEmpty()
            .MaximumLength(80)
            .WithName("Nombre del Parametro");

        RuleFor(r => r.Display)
            .NotEmpty()
            .MaximumLength(80)
            .WithName("Display");
    }
}
