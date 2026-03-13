namespace Anfx.Pasivos.Application.Common.Interfaces
{
    /// <summary>
    /// Servicio para generación segura de IDs consecutivos
    /// Historia Técnica: TEC-002 - Transacciones Serializables para Consecutivos
    /// Garantiza unicidad en ambientes de alta concurrencia
    /// </summary>
    public interface IConsecutivoService
    {
        /// <summary>
        /// Obtiene el siguiente ID consecutivo para una tabla específica
        /// Proceso:
        /// 1. Inicia TransactionScope con IsolationLevel.Serializable
        /// 2. Bloquea tabla Consecutivo con tablockx
        /// 3. Obtiene o crea consecutivo para la tabla
        /// 4. Incrementa consecutivo
        /// 5. Actualiza FecUltimoCambio
        /// 6. Commit de transacción
        ///
        /// Regla de Negocio: La generación de IDs debe usar transacciones serializables con bloqueo de tabla
        /// </summary>
        /// <param name="nombreTabla">Nombre de la tabla para la cual generar el consecutivo</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>El próximo ID consecutivo</returns>
        Task<(bool Success, int ConsecutivoGenerado, string ErrorMessage)> ObtenerSiguienteConsecutivoAsync(
            string nombreTabla,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene el consecutivo actual sin incrementarlo
        /// Útil para consultas
        /// </summary>
        Task<int> ObtenerConsecutivoActualAsync(string nombreTabla, CancellationToken cancellationToken = default);

        /// <summary>
        /// Inicializa un consecutivo para una tabla si no existe
        /// </summary>
        Task<bool> InicializarConsecutivoAsync(string nombreTabla, int valorInicial = 1, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reinicia el consecutivo de una tabla (usar con precaución)
        /// </summary>
        Task<bool> ReiniciarConsecutivoAsync(string nombreTabla, int nuevoValor = 1, CancellationToken cancellationToken = default);
    }
}
