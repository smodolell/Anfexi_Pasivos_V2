namespace Anfx.Pasivos.Application.Features.Reportes.DTOs;

public class ReporteEditDto
{
    public int ReporteId { get; set; }
    public string NomReporte { get; set; } = "";
    public string StoredProcedure { get; set; } = "";
    public int? ReporteFormatoId { get; set; }
    public bool Activo { get; set; }
}

public class ReporteEditDtoValidator : AbstractValidator<ReporteEditDto>
{
    public ReporteEditDtoValidator()
    {
        RuleFor(r => r.NomReporte)
            .NotEmpty()
            .WithName("Nombre del Reporte");

        RuleFor(r => r.StoredProcedure)
            .NotEmpty()
            .WithName("Procedimiento");

        RuleFor(r => r.ReporteFormatoId)
            .NotNull()
            .GreaterThanOrEqualTo(0)
            .WithName("Formato Salida");
    }
}
