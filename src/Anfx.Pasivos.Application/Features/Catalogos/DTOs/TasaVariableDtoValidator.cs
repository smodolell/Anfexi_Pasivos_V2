namespace Anfx.Pasivos.Application.Features.Catalogos.DTOs;

public class TasaVariableDtoValidator : AbstractValidator<TasaVariableDto>
{
    public TasaVariableDtoValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("Requerido")
            .MaximumLength(100).WithMessage("debe ser < que 100 Car.");
    }
}
