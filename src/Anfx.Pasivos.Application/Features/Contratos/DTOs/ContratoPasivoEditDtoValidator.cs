namespace Anfx.Pasivos.Application.Features.Contratos.DTOs;

public class ContratoPasivoEditDtoValidator : AbstractValidator<ContratoPasivoEditDto>
{
    private readonly IApplicationDbContext _context;

    public ContratoPasivoEditDtoValidator(IApplicationDbContext context)
    {
        _context = context;

        // Validaciones de existencia
        RuleFor(x => x.IdLineaCredito)
            .MustAsync(LineaCreditoExistsAsync)
            .WithMessage("No existe Línea de Crédito")
            .WithErrorCode("LC001");

        RuleFor(x => x.IdPeriodicidad)
            .MustAsync(PeriodicidadExistsAsync)
            .WithMessage("No existe Periodicidad")
            .WithErrorCode("PER001");

        RuleFor(x => x.IdTipoCredito)
            .MustAsync(TipoCreditoExistsAsync)
            .WithMessage("No existe Tipo de Crédito")
            .WithErrorCode("TC001");

        RuleFor(x => x.IdMoneda)
            .MustAsync(MonedaExistsAsync)
            .WithMessage("No existe Moneda")
            .WithErrorCode("MON001");

        RuleFor(x => x.IdTasa)
            .MustAsync(TasaExistsAsync)
            .WithMessage("No existe Tasa")
            .WithErrorCode("TAS001");

        RuleFor(x => x.IdTipoTablaAmortiza)
            .MustAsync(TipoTablaAmortizaExistsAsync)
            .WithMessage("No existe Tipo de Tabla de Amortización")
            .WithErrorCode("TTA001");

        // Validaciones de capital
        RuleFor(x => x.CapitalFinanciado)
            .MustAsync(async (model, capital, cancellation) =>
                await MontoDisponibleSuficienteAsync(model.IdLineaCredito, capital, cancellation))
            .WithMessage("Se ha detectado que la línea de crédito no dispone de fondos para cubrir el Capital financiado solicitado")
            .WithErrorCode("LC002");

        RuleFor(x => x)
            .Must(x => x.CapitalFinanciado <= x.MaxCapitalDisponible)
            .WithMessage("El Capital Financiado no puede ser mayor al Capital Máximo Disponible")
            .WithErrorCode("CAP001");

 
        // Validación de plazo máximo
        RuleFor(x => x)
            .MustAsync(ValidarPlazoMaximoAsync)
            .WithMessage("El plazo máximo supera al permitido en la línea de Crédito")
            .WithErrorCode("LC003");

        // Validaciones de fechas
        RuleFor(x => x.FecInicioContrato)
            .NotNull()
            .WithMessage("La fecha de inicio de contrato es requerida")
            .WithErrorCode("FEC001");

        RuleFor(x => x)
            .Must(x => x.FecInicioContrato <= x.FecPrimeraRenta)
            .WithMessage("La fecha de inicio de contrato no puede ser superior a la fecha de primera renta")
            .WithErrorCode("FEC002");


        // Validaciones de tasas
        RuleFor(x => x.Tasa)
            .GreaterThan(0)
            .WithMessage("La tasa debe ser mayor a 0")
            .WithErrorCode("TAS002");

        RuleFor(x => x)
            .Must(x => x.Tasa == CalcularTasa(x))
            .WithMessage("La tasa calculada no coincide con los valores proporcionados")
            .WithErrorCode("TAS003");

        RuleFor(x => x.TasaMora)
            .GreaterThan(0)
            .WithMessage("La tasa de mora debe ser mayor a 0")
            .WithErrorCode("TAS004");

        // Validación de pagos irregulares
        RuleFor(x => x)
            .MustAsync(ValidarPagosIrregularesAsync)
            .When(x => new[] { 3, 4 }.Contains(x.IdTipoTablaAmortiza) &&
                       x.IdTipoPagoCapital == 2 &&
                       x.IdTipoCapitalizacion == 1)
            .WithMessage("La suma de los capitales de la tabla de pagos Irregulares no es igual al Capital Financiado")
            .WithErrorCode("PAG001");

        RuleFor(x => x)
            .Must(x => x.NoPagosIrregulares == (x.Pagos?.Count ?? 0))
            .When(x => x.NoPagosIrregulares.HasValue && x.Pagos != null)
            .WithMessage("El número de pagos irregulares no coincide con la cantidad de pagos proporcionados")
            .WithErrorCode("PAG002");

        // Validaciones de tipos de pago y capitalización
        RuleFor(x => x)
            .Must(x => x.IdTipoCapitalizacion.HasValue && x.IdTipoPagoCapital.HasValue)
            .When(x => new[] { 3, 4 }.Contains(x.IdTipoTablaAmortiza))
            .WithMessage("Para tablas de amortización tipo 3 o 4, se requieren Tipo de Capitalización y Tipo de Pago de Capital")
            .WithErrorCode("TAB001");

        // Validaciones de fondeador
        RuleFor(x => x.IdFondeador)
            .MustAsync(FondeadorExistsAsync)
            .WithMessage("No existe el Fondeador especificado")
            .WithErrorCode("FON001");

    
    }

    #region Métodos de validación

    private async Task<bool> LineaCreditoExistsAsync(int idLineaCredito, CancellationToken cancellation)
    {
        return await _context.PSV_LineaCredito
            .AnyAsync(r => r.IdLineaCredito == idLineaCredito, cancellation);
    }

    private async Task<bool> PeriodicidadExistsAsync(int idPeriodicidad, CancellationToken cancellation)
    {
        return await _context.SB_Periodicidad
            .AnyAsync(r => r.IdPeriodicidad == idPeriodicidad, cancellation);
    }

    private async Task<bool> TipoCreditoExistsAsync(int idTipoCredito, CancellationToken cancellation)
    {
        return await _context.PSV_TipoCredito
            .AnyAsync(r => r.IdTipoCredito == idTipoCredito, cancellation);
    }

    private async Task<bool> MonedaExistsAsync(int idMoneda, CancellationToken cancellation)
    {
        return await _context.SB_TipoMoneda
            .AnyAsync(r => r.IdTipoMoneda == idMoneda, cancellation);
    }

    private async Task<bool> TasaExistsAsync(int idTasa, CancellationToken cancellation)
    {
        return await _context.Tasa
            .AnyAsync(r => r.IdTasa == idTasa, cancellation);
    }

    private async Task<bool> TipoTablaAmortizaExistsAsync(int idTipoTablaAmortiza, CancellationToken cancellation)
    {
        return await _context.PSV_TipoTablaAmortiza
            .AnyAsync(r => r.IdTipoTablaAmortiza == idTipoTablaAmortiza, cancellation);
    }

    private async Task<bool> FondeadorExistsAsync(int idFondeador, CancellationToken cancellation)
    {
        return await _context.PSV_Fondeador
            .AnyAsync(r => r.IdFondeador == idFondeador, cancellation);
    }

    private async Task<bool> MontoDisponibleSuficienteAsync(int idLineaCredito, decimal capitalFinanciado, CancellationToken cancellation)
    {
        var lineaCredito = await _context.PSV_LineaCredito
            .SingleOrDefaultAsync(r => r.IdLineaCredito == idLineaCredito, cancellation);

        return lineaCredito != null && lineaCredito.MontoDisponible >= capitalFinanciado;
    }

    private async Task<bool> ValidarPlazoMaximoAsync(ContratoPasivoEditDto model, CancellationToken cancellation)
    {
        var lineaCredito = await _context.PSV_LineaCredito
            .SingleOrDefaultAsync(r => r.IdLineaCredito == model.IdLineaCredito, cancellation);

        var periodicidad = await _context.SB_Periodicidad
            .SingleOrDefaultAsync(r => r.IdPeriodicidad == model.IdPeriodicidad, cancellation);

        if (lineaCredito == null || periodicidad == null)
            return false;

        var plazoCalculado = periodicidad.ParamMes!.Value * (model.Plazo / periodicidad.NoPagosMes!.Value);
        return plazoCalculado <= lineaCredito.PlazoMaximo;
    }

    private static async Task<bool> ValidarPagosIrregularesAsync(ContratoPasivoEditDto model, CancellationToken cancellation)
    {
        if (model.Pagos == null || !model.Pagos.Any())
            return true;

        var capital = model.Pagos.Sum(s => s.Capital);
        return Math.Abs(capital - model.CapitalFinanciado) < 0.01m; // Tolerancia para decimales
    }


    private static decimal CalcularTasa(ContratoPasivoEditDto model)
    {
        // Lógica para calcular la tasa según tipo de tasa
        if (model.TipoTasa == true) // Tasa fija
        {
            return model.TasaBase + model.PuntosMas - (model.PuntosPor);
        }
        else // Tasa variable
        {
            // Lógica para tasa variable
            return model.TasaBase + model.PuntosMas - (model.PuntosPor);
        }
    }

    #endregion
}