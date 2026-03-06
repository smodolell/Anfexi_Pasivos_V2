namespace Anfx.Pasivos.Application.Features.Configuracion.Dtos;

public class TipoTablaAmortizaDtoValidator : AbstractValidator<TipoTablaAmortizaDto>
{
    public TipoTablaAmortizaDtoValidator()
    {
        RuleFor(x => x.TipoTablaAmortiza)
            .NotEmpty().WithMessage("Requerido")
            .MaximumLength(150).WithMessage("debe ser < que 150 Car.");
    }
}
