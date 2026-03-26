using Anfx.Pasivos.Application.Common.Models.StoredProcedures;

namespace Anfx.Pasivos.Application.Common.Interfaces;

public interface IApplicationDbContextProcedures
{
    Task<List<usp_CarteraActiva_CIResult>> usp_CarteraActiva_CIAsync(int? idFondeador, int? idContratoPasivo, int? idContratoActivo, int? saldos, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
    Task<List<usp_CarteraActivaMensual_CIResult>> usp_CarteraActivaMensual_CIAsync(int? idFondeador, int? idContratoPasivo, int? idContratoActivo, int? saldos, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
    Task<List<usp_CarteraPasivaMensual_CIResult>> usp_CarteraPasivaMensual_CIAsync(int? idFondeador, int? idContratoPasivo, int? idContratoActivo, int? saldos, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
    Task<List<usp_PSV_AnticipoACapitalResult>> usp_PSV_AnticipoACapitalAsync(int? idTerminacion, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
    Task<List<usp_PSV_AplicaAnticipo_CIResult>> usp_PSV_AplicaAnticipo_CIAsync(int? idTerminacion, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
    Task<List<usp_PSV_LiquidacionResult>> usp_PSV_LiquidacionAsync(int? idTerminacion, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
    Task<int> usp_PSV_AnticipoAPlazoAsync(int? idTerminacion, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
    Task<List<usp_PSV_CalculaInteresResult>> usp_PSV_CalculaInteresAsync(int? idContrato, DateOnly? fechaCorte, decimal? montoAnticipo, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
    Task<List<usp_PSV_PagarCargosResult>> usp_PSV_PagarCargosAsync(
        int? idFondeador,
        int? idContrato,
        int? idUsuario,
        int? idTipoPago,
        int? idCuentaBancaria,
        DateOnly? fechaPago,
        string referencia,
        decimal? montoPago,
        IEnumerable<KeyItem> movimientos,
        OutputParameter<int> returnValue = null,
        CancellationToken cancellationToken = default);


    Task<List<usp_PSV_ProcesaMoratoriosResult>> usp_PSV_ProcesaMoratoriosAsync(DateTime? fechaProcesamiento, int? idContrato, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
    Task<List<usp_PSV_ProcesaVencimientosResult>> usp_PSV_ProcesaVencimientosAsync(int? idFondeador, int? idContrato, DateOnly? fechaInicial, DateOnly? fechaFinal, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
    Task<List<usp_PSV_GeneraTablaAmortizaResult>> usp_PSV_GeneraTablaAmortizaAsync(int? idContrato, bool? raiseError, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
    Task<List<usp_PSV_ActivarContratoResult>> usp_PSV_ActivarContratoAsync(int? idContrato, DateOnly? fechaActivacion, bool? raiseError, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
}
