namespace Anfx.Pasivos.Application.Features.Configuracion.Dtos;

public class LineaCreditoEditDtoValidator : AbstractValidator<LineaCreditoEditDto>
{
    public LineaCreditoEditDtoValidator()
    {
        RuleFor(x => x.IdFondeador)
            .NotEmpty().WithMessage("El fondeador es requerido")
            .GreaterThan(0).WithMessage("El fondeador debe ser válido");

        RuleFor(x => x.IdMoneda)
            .NotEmpty().WithMessage("La moneda es requerida")
            .GreaterThan(0).WithMessage("La moneda debe ser válida")
            .WithName("Moneda");

        RuleFor(x => x.MontoAprobado)
            .NotEmpty().WithMessage("El monto aprobado es requerido")
            .GreaterThan(0).WithMessage("El monto aprobado debe ser mayor a 0")
            .LessThanOrEqualTo(99999999.99m).WithMessage("El monto aprobado no puede exceder 99,999,999.99");

        RuleFor(x => x.FechaAprobacion)
            .NotEmpty().WithMessage("La fecha de aprobación es requerida");

        RuleFor(x => x.PlazoMaximo)
            .GreaterThan(0).WithMessage("El plazo máximo debe ser mayor a 0")
            .LessThanOrEqualTo(1000).WithMessage("El plazo máximo no puede exceder 1000 meses");

        RuleFor(x => x.IdTasa)
            .NotEmpty().WithMessage("El tipo de tasa es requerido")
            .GreaterThan(0).WithMessage("El tipo de tasa debe ser válido");

        RuleFor(x => x.Tasa)
            .NotEmpty().WithMessage("La tasa es requerida")
            .GreaterThan(0).WithMessage("La tasa debe ser mayor a 0")
            .LessThanOrEqualTo(100).WithMessage("La tasa no puede exceder 100%");

        // Validaciones condicionales
        RuleFor(x => x.FechaMaxDisposicion)
            .GreaterThanOrEqualTo(x => x.FechaAprobacion)
            .When(x => x.FechaMaxDisposicion.HasValue && x.FechaAprobacion.HasValue)
            .WithMessage("La fecha máxima de disposición debe ser posterior a la fecha de aprobación");

        RuleFor(x => x.FechaAmpliacion)
            .GreaterThanOrEqualTo(x => x.FechaAprobacion)
            .When(x => x.FechaAmpliacion.HasValue && x.FechaAprobacion.HasValue)
            .WithMessage("La fecha de ampliación debe ser posterior a la fecha de aprobación");

        RuleFor(x => x.MontoRevolvente)
            .LessThanOrEqualTo(x => x.MontoAprobado)
            .When(x => x.EsRevolvente)
            .WithMessage("El monto revolvente no puede ser mayor al monto aprobado");
    }
}