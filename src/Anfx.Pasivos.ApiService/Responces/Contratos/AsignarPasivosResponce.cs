namespace Anfx.Pasivos.ApiService.Responces.Contratos;

public class AsignarPasivosResponce
{
    /// <summary>
    /// Lista de IDs de contratos activos a asignar
    /// </summary>
    public int[]? ListaContratos { get; set; }

    /// <summary>
    /// ID de un contrato activo específico a asignar
    /// </summary>
    public int? ContratosActivos { get; set; }
}
