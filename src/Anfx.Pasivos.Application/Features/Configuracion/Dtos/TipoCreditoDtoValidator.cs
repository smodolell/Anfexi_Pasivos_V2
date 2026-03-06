namespace Anfx.Pasivos.Application.Features.Configuracion.Dtos;

public class TipoCreditoDtoValidator : AbstractValidator<TipoCreditoDto>
{
    private readonly IApplicationDbContext _context;

    public TipoCreditoDtoValidator(IApplicationDbContext context)
    {
        this._context = context;


        // Validación para TipoCredito
        RuleFor(x => x.TipoCredito)
            .NotEmpty().WithMessage("Requerido")
            .MaximumLength(150).WithMessage("debe ser < que 150 Car.");

        // Validación para IdTipoMovimiento
        RuleFor(x => x.IdTipoMovimiento)
            .NotEmpty().WithMessage("Requerido")
            .GreaterThan(0).WithMessage("Debe ser un ID válido mayor a 0")
            .MustAsync(ExistInTipoMovimiento)
            .WithMessage("El ID de Tipo Movimiento Mora no existe en la base de datos"); ;

        // Validación para Prefijo
        RuleFor(x => x.Prefijo)
            .NotEmpty().WithMessage("Requerido")
            .MaximumLength(10).WithMessage("debe ser < que 10 Car.");

        // Validación para Sufijo
        RuleFor(x => x.Sufijo)
            .NotEmpty().WithMessage("Requerido")
            .MaximumLength(10).WithMessage("debe ser < que 10 Car.");

        // Validación para Contador
        RuleFor(x => x.Contador)
            .GreaterThanOrEqualTo(0).WithMessage("El contador no puede ser negativo");

        // Validación para IdTipoTablaAmortiza
        RuleFor(x => x.IdTipoTablaAmortiza)
            .NotEmpty().WithMessage("Requerido")
            .GreaterThan(0).WithMessage("Debe ser un ID válido mayor a 0")
            .MustAsync(ExistInTipoTablaAmortiza)
            .WithMessage("El ID de Tipo TablaAmortiza no existe en la base de datos");
        ;

        // Validación para IdTipoMovimiento_Mora
        RuleFor(x => x.IdTipoMovimiento_Mora)
         .NotEmpty().WithMessage("Requerido")
         .GreaterThan(0).WithMessage("Debe ser un ID válido mayor a 0")
         .MustAsync(ExistInTipoMovimiento)
         .WithMessage("El ID de Tipo Movimiento Mora no existe en la base de datos");

        // Reglas condicionales adicionales
        When(x => !string.IsNullOrEmpty(x.Prefijo), () =>
        {
            RuleFor(x => x.Prefijo)
                .Matches(@"^[A-Za-z0-9]*$").WithMessage("El prefijo solo puede contener caracteres alfanuméricos");
        });

        When(x => !string.IsNullOrEmpty(x.Sufijo), () =>
        {
            RuleFor(x => x.Sufijo)
                .Matches(@"^[A-Za-z0-9]*$").WithMessage("El sufijo solo puede contener caracteres alfanuméricos");
        });

        // Validación personalizada para asegurar que Prefijo y Sufijo no sean iguales
        RuleFor(x => x)
            .Must(x => x.Prefijo != x.Sufijo)
            .WithMessage("El prefijo y sufijo no pueden ser iguales");
    
    }

    private async Task<bool> ExistInTipoMovimiento(int idTipoMovimiento, CancellationToken cancellationToken)
    {
        return await _context.TipoMovimiento
            .AnyAsync(tm => tm.IdTipoMovimiento == idTipoMovimiento, cancellationToken);
    }

    private async Task<bool> ExistInTipoTablaAmortiza(int idTipoTablaAmortiza, CancellationToken cancellationToken)
    {
        return await _context.PSV_TipoTablaAmortiza
            .AnyAsync(tm => tm.IdTipoTablaAmortiza == idTipoTablaAmortiza, cancellationToken);
    }
}
