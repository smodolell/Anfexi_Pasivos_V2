using Anfx.Pasivos.Application.Common.Models.StoredProcedures;

namespace Anfx.Pasivos.Application.Common.Interfaces;

public interface IApplicationDbContextProcedures
{
    Task<List<usp_CarteraActiva_CIResult>> usp_CarteraActiva_CIAsync(int? idFondeador, int? idContratoPasivo, int? idContratoActivo, int? saldos, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
    Task<List<usp_CarteraActivaMensual_CIResult>> usp_CarteraActivaMensual_CIAsync(int? idFondeador, int? idContratoPasivo, int? idContratoActivo, int? saldos, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
    Task<List<usp_CarteraPasivaMensual_CIResult>> usp_CarteraPasivaMensual_CIAsync(int? idFondeador, int? idContratoPasivo, int? idContratoActivo, int? saldos, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
}
