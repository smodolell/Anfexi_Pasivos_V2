namespace Anfx.Pasivos.Application.Features.Operaciones.DTOs;

public class CajaDtoValidator : AbstractValidator<CajaDto>
{
    private readonly IApplicationDbContext _context;

    public CajaDtoValidator(IApplicationDbContext context)
    {
        _context = context;

        // Validaciones básicas de campos requeridos
        RuleFor(x => x.IdTipoPago)
            .NotEmpty().WithMessage("El tipo de pago es requerido")
            .GreaterThan(0).WithMessage("El tipo de pago debe ser válido");

        RuleFor(x => x.IdBanco)
            .NotEmpty().WithMessage("El banco es requerido")
            .GreaterThan(0).WithMessage("El banco debe ser válido");

        RuleFor(x => x.IdCuentaBancaria)
            .NotEmpty().WithMessage("La cuenta bancaria es requerida")
            .GreaterThan(0).WithMessage("La cuenta bancaria debe ser válida");

        RuleFor(x => x.FechaPago)
            .NotEmpty().WithMessage("La fecha de pago es requerida")
            .Must(FechaNoFutura).WithMessage("La fecha de pago no puede ser futura")
            .Must(FechaNoMayorActual).WithMessage("La fecha de pago no puede ser mayor a la fecha actual");

        RuleFor(x => x.MontoPago)
            .NotEmpty().WithMessage("El monto del pago es requerido")
            .GreaterThan(0).WithMessage("El monto del pago debe ser mayor a cero")
            .PrecisionScale(18, 2, false).WithMessage("El monto del pago debe tener máximo 2 decimales");

        RuleFor(x => x.Referencia)
            .MaximumLength(50).WithMessage("La referencia no puede exceder los 50 caracteres")
            .When(x => !string.IsNullOrWhiteSpace(x.Referencia));

        // Validación condicional para ContratoPasivo
        RuleFor(x => x.ContratoPasivo)
            .NotEmpty().WithMessage("El contrato pasivo es requerido")
            .MaximumLength(50).WithMessage("El contrato pasivo no puede exceder los 50 caracteres")
            .MustAsync(ContratoExiste).WithMessage("El contrato pasivo no existe")
            .When(x => string.IsNullOrEmpty(x.ContratoPasivo) || x.IdContrato == null);

        // Validación condicional para IdContrato
        RuleFor(x => x.IdContrato)
            .NotEmpty().WithMessage("El ID del contrato es requerido")
            .GreaterThan(0).WithMessage("El ID del contrato debe ser válido")
            .MustAsync(ContratoExistePorId).WithMessage("El contrato no existe")
            .When(x => x.IdContrato.HasValue && string.IsNullOrEmpty(x.ContratoPasivo));

        // Validación de IdFondeador
        RuleFor(x => x.IdFondeador)
            .NotEmpty().WithMessage("El fondeador es requerido")
            .GreaterThan(0).WithMessage("El fondeador debe ser válido")
            .MustAsync(FondeadorExiste).WithMessage("El fondeador no existe")
            .When(x => x.IdFondeador.HasValue);

        // Validación de la lista de movimientos
        RuleFor(x => x.Movimientos)
            .NotEmpty().WithMessage("Debe incluir al menos un movimiento")
            .Must(movimientos => movimientos.Any(m => m.Seleccionado))
            .WithMessage("Debe seleccionar al menos un movimiento para realizar el pago");


        // Validaciones de integridad referencial
        RuleFor(x => x.IdTipoPago)
            .MustAsync(TipoPagoExiste).WithMessage("El tipo de pago seleccionado no existe");

        RuleFor(x => x.IdBanco)
            .MustAsync(BancoExiste).WithMessage("El banco seleccionado no existe");

        RuleFor(x => x.IdCuentaBancaria)
            .MustAsync(CuentaBancariaExiste).WithMessage("La cuenta bancaria seleccionada no existe");

        // Validar que la cuenta bancaria pertenezca al banco seleccionado
        RuleFor(x => x)
            .MustAsync(async (dto, cancellation) =>
                await CuentaPerteneceABanco(dto.IdCuentaBancaria, dto.IdBanco, cancellation))
            .WithMessage("La cuenta bancaria no pertenece al banco seleccionado");
    }

    // Validaciones síncronas
    private bool FechaNoFutura(DateOnly fecha)
    {
        return fecha <= DateOnly.FromDateTime(DateTime.Now);
    }

    private bool FechaNoMayorActual(DateOnly fecha)
    {
        return fecha <= DateOnly.FromDateTime(DateTime.Now);
    }

    // Validaciones asíncronas con base de datos
    private async Task<bool> ContratoExiste(string contratoPasivo, CancellationToken cancellationToken)
    {
        return await _context.PSV_Contrato
            .AnyAsync(c => c.Contrato == contratoPasivo, cancellationToken);
    }

    private async Task<bool> ContratoExistePorId(int? idContrato, CancellationToken cancellationToken)
    {
        if (!idContrato.HasValue) return false;
        return await _context.PSV_Contrato
            .AnyAsync(c => c.IdContrato == idContrato.Value, cancellationToken);
    }

    private async Task<bool> FondeadorExiste(int? idFondeador, CancellationToken cancellationToken)
    {
        if (!idFondeador.HasValue) return false;
        return await _context.PSV_Fondeador
            .AnyAsync(f => f.IdFondeador == idFondeador.Value, cancellationToken);
    }

    private async Task<bool> TipoPagoExiste(int idTipoPago, CancellationToken cancellationToken)
    {
        return await _context.PSV_TipoPago
            .AnyAsync(t => t.IdTipoPago == idTipoPago, cancellationToken);
    }

    private async Task<bool> BancoExiste(int idBanco, CancellationToken cancellationToken)
    {
        return await _context.PSV_Banco
            .AnyAsync(b => b.IdBanco == idBanco, cancellationToken);
    }

    private async Task<bool> CuentaBancariaExiste(int idCuentaBancaria, CancellationToken cancellationToken)
    {
        return await _context.PSV_CuentaBancaria
            .AnyAsync(c => c.IdCuentaBancaria == idCuentaBancaria, cancellationToken);
    }

    private async Task<bool> CuentaPerteneceABanco(int idCuentaBancaria, int idBanco, CancellationToken cancellationToken)
    {
        var cuenta = await _context.PSV_CuentaBancaria
            .FirstOrDefaultAsync(c => c.IdCuentaBancaria == idCuentaBancaria, cancellationToken);

        return cuenta != null && cuenta.IdBanco == idBanco;
    }
}
