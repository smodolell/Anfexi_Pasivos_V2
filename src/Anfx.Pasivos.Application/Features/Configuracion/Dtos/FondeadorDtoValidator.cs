namespace Anfx.Pasivos.Application.Features.Configuracion.Dtos;

public class FondeadorDtoValidator : AbstractValidator<FondeadorEditDto>
{
    public FondeadorDtoValidator()
    {
        RuleFor(x => x.Fondeador)
            .NotEmpty().WithMessage("El título es requerido")
            .MaximumLength(200).WithMessage("El título no puede exceder los 200 caracteres");

        RuleFor(x => x.ClaveCuentaContable)
            .MaximumLength(50).WithMessage("La clave de cuenta contable no puede exceder los 50 caracteres")
            .When(x => !string.IsNullOrEmpty(x.ClaveCuentaContable)); 
    }
}