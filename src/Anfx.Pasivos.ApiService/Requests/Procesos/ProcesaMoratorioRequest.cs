namespace Anfx.Pasivos.ApiService.Requests.Procesos;

public class ProcesaMoratorioRequest
{
    /// <summary>
    /// Fecha de procesamiento de moratorios (opcional, por defecto fecha actual)
    /// </summary>
    public DateTime? FechaProcesamiento { get; set; }

    /// <summary>
    /// ID del contrato a procesar (opcional, si no se envía procesa todos)
    /// </summary>
    public int? IdContrato { get; set; }

    /// <summary>
    /// Número de contrato pasivo (opcional, alternativa a IdContrato)
    /// </summary>
    public string? ContratoPasivo { get; set; }
}
