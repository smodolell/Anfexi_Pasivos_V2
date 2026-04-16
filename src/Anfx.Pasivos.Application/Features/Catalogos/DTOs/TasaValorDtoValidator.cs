namespace Anfx.Pasivos.Application.Features.Catalogos.DTOs;

public class TasaValorDtoValidator : AbstractValidator<TasaValorDto>
{
    public TasaValorDtoValidator()
    {
        RuleFor(x => x.ValorTasa)
            .GreaterThanOrEqualTo(0).WithMessage("El valor debe ser mayor o igual a 0");

        RuleFor(x => x.FecValorTasa)
            .NotEmpty().WithMessage("Requerido");
    }
}
