using Microsoft.AspNetCore.Diagnostics;
using System.Diagnostics;
using Anfx.Pasivos.ApiService.Responces;

namespace Anfx.Pasivos.ApiService.Infrastructure;

public class CustomExceptionHandler : IExceptionHandler
{
    private readonly ILogger<CustomExceptionHandler> _logger;
    private readonly IHostEnvironment _environment;

    public CustomExceptionHandler(ILogger<CustomExceptionHandler> logger, IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Excepción capturada: {Message}", exception.Message);

        // Determinar el código de estado HTTP según el tipo de excepción
        var statusCode = exception switch
        {
            BadHttpRequestException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            ArgumentException => StatusCodes.Status400BadRequest,
            InvalidOperationException => StatusCodes.Status409Conflict,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        // Construir mensaje y errores apropiados según el ambiente
        var (message, errors) = GetErrorDetails(exception, statusCode);

        var response = new ApiResponseDto<object>
        {
            Success = false,
            Message = message,
            Data = null,
            Errors = errors,
            StatusCode = statusCode,
            Timestamp = DateTime.UtcNow,
            TraceId = Activity.Current?.Id ?? httpContext.TraceIdentifier
        };

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true; // Indica que la excepción ya fue manejada
    }

    private (string message, List<string> errors) GetErrorDetails(Exception exception, int statusCode)
    {
        var errors = new List<string>();

        // En desarrollo mostrar detalles completos
        if (_environment.IsDevelopment())
        {
            errors.Add(exception.Message);
            if (exception.InnerException != null)
            {
                errors.Add($"Inner: {exception.InnerException.Message}");
            }
            errors.Add($"StackTrace: {exception.StackTrace}");

            var message = exception switch
            {
                BadHttpRequestException => "Error en la solicitud",
                UnauthorizedAccessException => "Acceso no autorizado",
                ArgumentException => "Argumento inválido",
                InvalidOperationException => "Operación inválida",
                KeyNotFoundException => "Recurso no encontrado",
                _ => "Error interno del servidor"
            };

            return (message, errors);
        }

        // En producción mostrar mensajes genéricos y errores limitados
        errors.Add(GetProductionErrorMessage(exception, statusCode));

        var productionMessage = statusCode switch
        {
            StatusCodes.Status400BadRequest => "La solicitud no es válida",
            StatusCodes.Status401Unauthorized => "No autorizado",
            StatusCodes.Status403Forbidden => "Acceso denegado",
            StatusCodes.Status404NotFound => "Recurso no encontrado",
            StatusCodes.Status409Conflict => "Conflicto de datos",
            StatusCodes.Status503ServiceUnavailable => "Servicio no disponible",
            _ => "Error interno del servidor"
        };

        return (productionMessage, errors);
    }

    private string GetProductionErrorMessage(Exception exception, int statusCode)
    {
        return statusCode switch
        {
            StatusCodes.Status400BadRequest => "La solicitud no es válida. Por favor revise los datos enviados.",
            StatusCodes.Status401Unauthorized => "No está autorizado para realizar esta operación.",
            StatusCodes.Status403Forbidden => "No tiene permisos para acceder a este recurso.",
            StatusCodes.Status404NotFound => "El recurso solicitado no existe.",
            StatusCodes.Status409Conflict => "Conflicto con el estado actual del recurso.",
            StatusCodes.Status503ServiceUnavailable => "El servicio no está disponible temporalmente.",
            _ => "Ha ocurrido un error interno en el servidor. Por favor intente más tarde."
        };
    }
}