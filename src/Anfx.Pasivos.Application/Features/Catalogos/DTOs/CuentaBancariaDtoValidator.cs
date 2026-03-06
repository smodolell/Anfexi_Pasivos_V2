namespace Anfx.Pasivos.Application.Features.Catalogos.DTOs;

public class CuentaBancariaDtoValidator : AbstractValidator<CuentaBancariaDto>
{
    private readonly IApplicationDbContext _context;

    public CuentaBancariaDtoValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.IdBanco)
            .NotEmpty().WithMessage("Requerido")
            .MustAsync(ExistInBancos).WithMessage("El Banco especificado no existe");

        RuleFor(x => x.CuentaBancaria)
            .NotEmpty().WithMessage("Requerido")
            .MaximumLength(50).WithMessage("debe ser < que 50 Car.");

        RuleFor(x => x.CLABE)
            .NotEmpty().WithMessage("Requerido")
            .MaximumLength(18).WithMessage("debe ser < que 18 Car.");
    }

    private async Task<bool> ExistInBancos(int idBanco, CancellationToken cancellationToken)
    {
        return await _context.PSV_Banco.AnyAsync(b => b.IdBanco == idBanco, cancellationToken);
    }
}
