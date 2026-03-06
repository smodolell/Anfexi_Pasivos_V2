namespace Anfx.Pasivos.Application.Features.Catalogos.DTOs;

public class TipoPagoDtoValidator : AbstractValidator<TipoPagoDto>
{
    public TipoPagoDtoValidator()
    {
        RuleFor(x => x.TipoPago)
            .NotEmpty().WithMessage("Requerido")
            .MaximumLength(150).WithMessage("debe ser < que 150 Car.");
    }
}
