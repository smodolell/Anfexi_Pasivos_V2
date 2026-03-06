namespace Anfx.Pasivos.Application.Features.Catalogos.DTOs;

public class BancoDtoValidator : AbstractValidator<BancoDto>
{
    public BancoDtoValidator()
    {
        RuleFor(x => x.Banco)
            .NotEmpty().WithMessage("Requerido")
            .MaximumLength(150).WithMessage("debe ser < que 150 Car.");
    }
}
