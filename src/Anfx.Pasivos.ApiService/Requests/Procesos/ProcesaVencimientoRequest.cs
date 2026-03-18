namespace Anfx.Pasivos.ApiService.Requests.Procesos;

public class ProcesaVencimientoRequest
{
    /// <summary>
    /// Fecha inicial del rango para procesar vencimientos
    /// </summary>
    public DateTime FechaInicial { get; set; }

    /// <summary>
    /// Fecha final del rango para procesar vencimientos
    /// </summary>
    public DateTime FechaFinal { get; set; }

    /// <summary>
    /// ID del fondeador para filtrar (opcional)
    /// </summary>
    public int? IdFondeador { get; set; }

    /// <summary>
    /// ID del contrato para filtrar (opcional)
    /// </summary>
    public int? IdContrato { get; set; }
}