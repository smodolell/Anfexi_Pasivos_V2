namespace Anfx.Pasivos.Application.Features.Catalogos.DTOs;

public class EstatusContratoDtoValidator : AbstractValidator<EstatusContratoDto>
{
    public EstatusContratoDtoValidator()
    {
        RuleFor(x => x.EstatusContrato)
            .NotEmpty().WithMessage("Requerido")
            .MaximumLength(150).WithMessage("debe ser < que 150 Car.");
    }
}
