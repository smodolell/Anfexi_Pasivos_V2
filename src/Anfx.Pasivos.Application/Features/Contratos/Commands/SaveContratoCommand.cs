using Anfx.Pasivos.Application.Features.Contratos.DTOs;
namespace Anfx.Pasivos.Application.Features.Contratos.Commands;

public class SaveContratoCommand : ICommand<Result<int>>
{
    public int IdContrato { get; set; }

    public required ContratoPasivoEditDto Model { get; set; }

}

internal class SaveContratoCommandHandler : ICommandHandler<SaveContratoCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<ContratoPasivoEditDto> _validator;

    public SaveContratoCommandHandler(
        IApplicationDbContext context,
        IUnitOfWork unitOfWork,
        IValidator<ContratoPasivoEditDto> validator)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<int>> HandleAsync(
        SaveContratoCommand message,
        CancellationToken cancellationToken = default)
    {
        var model = message.Model;
        var idContrato = message.IdContrato;

        // 1. Validar con FluentValidation
        var validationResult = await _validator.ValidateAsync(model, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<int>.Invalid(validationResult.AsErrors());
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // 2. Obtener entidades relacionadas (siempre necesarias)
            var lineaCredito = await _context.PSV_LineaCredito
                .SingleAsync(r => r.IdLineaCredito == model.IdLineaCredito, cancellationToken);

            var periodicidad = await _context.SB_Periodicidad
                .SingleAsync(r => r.IdPeriodicidad == model.IdPeriodicidad, cancellationToken);

            var tipoCredito = await _context.PSV_TipoCredito
                .SingleAsync(r => r.IdTipoCredito == model.IdTipoCredito, cancellationToken);

            // 3. Determinar si es Create o Update
            PSV_Contrato? contrato;
            bool esNuevo = false;

            if (idContrato == 0)
            {
                // CREATE: Nuevo contrato
                contrato = new PSV_Contrato { VersionTabla = 1};
                esNuevo = true;
                _context.PSV_Contrato.Add(contrato);
            }
            else
            {
                // UPDATE: Contrato existente
                contrato = await _context.PSV_Contrato
                    .Include(c => c.PSV_ContratoPagoIrregular)
                    .SingleOrDefaultAsync(c => c.IdContrato == idContrato, cancellationToken);

                if (contrato == null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<int>.Invalid(new ValidationError("No existe el contrato a actualizar"));
                }

                // Verificar si se puede modificar
                if (contrato.IdEstatusContrato != 1)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<int>.Invalid(new ValidationError($"No se puede modificar un contrato en estado CAPTURADO"));
                }
            }

            // 4. Mapear datos al contrato
            MapContrato(model, contrato, esNuevo);

            // 5. Actualizar línea de crédito (diferente para Create y Update)
            if (esNuevo)
            {
                // CREATE: Restar todo el capital
                ActualizarLineaCreditoParaCreacion(lineaCredito, model.CapitalFinanciado);
            }
            else
            {
                // UPDATE: Calcular diferencia
                var contratoOriginal = await _context.PSV_Contrato
                    .AsNoTracking()
                    .SingleAsync(c => c.IdContrato == idContrato, cancellationToken);

                var diferenciaCapital = model.CapitalFinanciado - contratoOriginal.CapitalFinanciado ?? 0;
                ActualizarLineaCreditoParaActualizacion(lineaCredito, diferenciaCapital);
            }

            // 6. Actualizar contador de tipo de crédito (solo para nuevos)
            if (esNuevo)
            {
                contrato.FecActivacion = null;
                contrato.FecFinContrato = null;

                tipoCredito.Contador++;
            }

            // 7. Procesar pagos irregulares
            if (EsTablaIrregular(model))
            {
                // Limpiar pagos existentes en caso de Update
                if (!esNuevo && contrato.PSV_ContratoPagoIrregular.Any())
                {
                    _context.PSV_ContratoPagoIrregular.RemoveRange(contrato.PSV_ContratoPagoIrregular);
                }

                ProcesarPagosIrregulares(contrato, model);
            }
            else if (!esNuevo && contrato.PSV_ContratoPagoIrregular.Any())
            {
                // Si ya no es tabla irregular, eliminar pagos existentes
                _context.PSV_ContratoPagoIrregular.RemoveRange(contrato.PSV_ContratoPagoIrregular);
            }

            // 8. Guardar cambios en BD
            await _unitOfWork.SaveAsync(cancellationToken);

            var rel = await _context.PSV_RelLineaCreditoContrato
                .SingleOrDefaultAsync(r=>r.IdLineaCredito == lineaCredito.IdLineaCredito && r.IdContrato== contrato.IdContrato);

            if(rel == null)
            {
                rel = new PSV_RelLineaCreditoContrato
                {
                    IdContrato = contrato.IdContrato,
                    IdLineaCredito = lineaCredito.IdLineaCredito,
                };
                _context.PSV_RelLineaCreditoContrato.Add(rel);
            }

            await _unitOfWork.SaveAsync(cancellationToken);


            // 9. Generar tabla de amortización
            await GenerarTablaAmortizacionAsync(contrato.IdContrato, cancellationToken);

            // 10. Confirmar transacción
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result.Success(contrato.IdContrato);
        }
        catch (DbUpdateException ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            // Log del error
            return Result.Error($"Error al guardar el contrato: {ex.Message}");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            // Log del error
            return Result<int>.Error($"Error inesperado: {ex.Message}");
        }
    }

    private void MapContrato(ContratoPasivoEditDto model, PSV_Contrato entity, bool esNuevo)
    {
        // Datos básicos
        entity.IdPeriodicidad = model.IdPeriodicidad;
        entity.IdTipoCredito = model.IdTipoCredito;
        entity.IdMoneda = model.IdMoneda;
        entity.IdTasa = model.IdTasa;
        entity.IdTipoTablaAmortiza = model.IdTipoTablaAmortiza;
        entity.IdTipoCapitalizacion = model.IdTipoCapitalizacion;
        entity.IdTipoPagoCapital = model.IdTipoPagoCapital;
        entity.Contrato = model.Contrato;
        entity.Capital = model.CapitalFinanciado;
        entity.CapitalFinanciado = model.CapitalFinanciado;
        entity.Enganche = 0;
        entity.PorcEnganche = 0;
        entity.Plazo = model.Plazo;
        entity.Tasa = model.Tasa;
        entity.TasaBase = model.TasaBase;
        entity.TasaMora = model.TasaMora;
        entity.FecInicioContrato = model.FecInicioContrato ?? DateTime.Now;
        entity.FecPrimeraRenta = model.FecPrimeraRenta;
        entity.FecActivacion = model.FecActivacion;
        entity.FecFinContrato = model.FecFinContrato;
        entity.FechaFirmaContrato = model.FechaFirmaContrato;

        //// Versión de tabla
        if (esNuevo)
        {
            entity.IdEstatusContrato = 1;
            entity.VersionTabla = 1;
        }
        

        // Datos adicionales
        entity.PuntosMas = model.PuntosMas;
        entity.PuntosPor = model.PuntosPor;
        entity.TasaBaseMora = model.TasaBaseMora;
        entity.IdTasaMora = model.IdTasaMora;
        entity.PuntosMasMora = model.PuntosMasMora;
        entity.PuntosPorMora = model.PuntosPorMora;
        entity.FactorMora = model.FactorMora;
        //entity.SaldoInsoluto = model.SaldoInsoluto;
        //entity.BallonPayment = model.BallonPayment;
        //entity.PorcBallonPayment = model.PorcBallonPayment;
        //entity.ValorResidual = model.ValorResidual;
        //entity.PorcValorResidual = model.PorcValorResidual;
        //entity.DepositoEnGarantia = model.DepositoEnGarantia;
        //entity.OpcionDeCompra = model.OpcionDeCompra;
        //entity.PorcOpcionDeCompra = model.PorcOpcionDeCompra;
        entity.TasaIva = model.TasaIva;
        entity.IdTipoCalculoTasaVariable = model.IdTipoCalculoTasaVariable;
        entity.NroRentasDepositoGarantia = model.NroRentasDepositoGarantia;
        entity.IdTipoMantenimiento = model.IdTipoMantenimiento;
        entity.TasaMensual = model.TasaMensual;
        //entity.FechaCierre = model.FechaCierre;
        entity.TasaEsVariable = model.TasaEsVariable;
        entity.IdFondeador = model.IdFondeador;
        entity.FactorFIRA = model.FactorFIRA;
        entity.IdPeriodicidad_TTA = model.IdPeriodicidad_TTA;
        entity.NoPagosIrregulares = model.NoPagosIrregulares;
    }

    private void ActualizarLineaCreditoParaCreacion(PSV_LineaCredito lineaCredito, decimal capitalFinanciado)
    {
        lineaCredito.MontoDisponible -= capitalFinanciado;
        lineaCredito.MontoDispuesto += capitalFinanciado;
        lineaCredito.NoDisposiciones++;
        lineaCredito.FechaUltimaDisposicion = DateTime.Now;
    }

    private void ActualizarLineaCreditoParaActualizacion(PSV_LineaCredito lineaCredito, decimal diferenciaCapital)
    {
        if (diferenciaCapital != 0)
        {
            lineaCredito.MontoDisponible -= diferenciaCapital;
            lineaCredito.MontoDispuesto += diferenciaCapital;

            if (diferenciaCapital > 0)
            {
                lineaCredito.NoDisposiciones++;
            }
            else if (diferenciaCapital < 0)
            {
                lineaCredito.NoDisposiciones--;
            }

            lineaCredito.FechaUltimaDisposicion = DateTime.Now;
        }
    }

    private void ProcesarPagosIrregulares(PSV_Contrato contrato, ContratoPasivoEditDto model)
    {
        // Establecer fecha de primera renta
        if (model.IdTipoPagoCapital == 2 && model.Pagos != null)
        {
            var primerPago = model.Pagos.FirstOrDefault(f => f.NoPago == 1);
            if (primerPago?.FecVencimiento != null)
            {
                contrato.FecPrimeraRenta = primerPago.FecVencimiento.Value;
            }
        }

        // Agregar pagos irregulares
        if (model.Pagos != null && model.Pagos.Any())
        {
            foreach (var pago in model.Pagos)
            {
                var pagoIrregular = new PSV_ContratoPagoIrregular
                {
                    IdContrato = contrato.IdContrato,
                    NoPago = pago.NoPago,
                    Capital = pago.Capital,
                    VersionTabla = contrato.VersionTabla ?? 1,
                    FecVencimiento = pago.FecVencimiento ?? DateTime.Now,
                    NoAplicaCapital = false,
                    Procesado = false
                };

                // Si es el último pago y capitalización tipo 2, ajustar capital
                if (pago.NoPago == model.NoPagosIrregulares && model.IdTipoCapitalizacion == 2)
                {
                    pagoIrregular.Capital = model.CapitalFinanciado;
                }

                contrato.PSV_ContratoPagoIrregular.Add(pagoIrregular);
            }
        }
    }

    private async Task GenerarTablaAmortizacionAsync(int idContrato, CancellationToken cancellationToken)
    {
        try
        {
            // Llamar al procedimiento almacenado
            await _context.Procedures.usp_PSV_GeneraTablaAmortizaAsync(idContrato, true);
        }
        catch (Exception ex)
        {
            // Loguear el error
            throw new InvalidOperationException($"Error al generar la tabla de amortización para el contrato {idContrato}: {ex.Message}", ex);
        }
    }

    private bool EsTablaIrregular(ContratoPasivoEditDto model)
    {
        return new[] { 3, 4 }.Contains(model.IdTipoTablaAmortiza) &&
               model.IdTipoPagoCapital == 2;
    }
}