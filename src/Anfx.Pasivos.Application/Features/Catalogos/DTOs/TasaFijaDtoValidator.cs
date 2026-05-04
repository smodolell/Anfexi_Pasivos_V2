namespace Anfx.Pasivos.Application.Features.Catalogos.DTOs;

public class TasaFijaDtoValidator : AbstractValidator<TasaFijaDto>
{
    public TasaFijaDtoValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("Requerido")
            .MaximumLength(100).WithMessage("debe ser < que 100 Car.");

        RuleFor(x => x.ValorTasa)
            .GreaterThanOrEqualTo(0).WithMessage("El valor debe ser mayor o igual a 0");
    }
}
