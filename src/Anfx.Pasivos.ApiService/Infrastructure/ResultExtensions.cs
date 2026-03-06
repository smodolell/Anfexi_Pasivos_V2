using Anfx.Pasivos.ApiService.Responces;
using Ardalis.Result;
using System.Diagnostics;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Anfx.Pasivos.ApiService.Infrastructure;

public static class ResultExtensions
{
    /// <summary>
    /// Convierte un Result<T> de Ardalis a IResult con formato personalizado
    /// </summary>
    public static IResult ToCustomMinimalApiResult<T>(this Result<T> result)
    {
        return result.Status switch
        {
            ResultStatus.Ok => HandleOk(result),
            ResultStatus.Created => HandleCreated(result),
            ResultStatus.NoContent => Results.NoContent(),

            // Errores
            ResultStatus.NotFound => HandleNotFound(result),
            ResultStatus.Invalid => HandleInvalid(result),
            ResultStatus.Error => HandleError(result),
            ResultStatus.Conflict => HandleConflict(result),
            ResultStatus.Unauthorized => HandleUnauthorized(result),
            ResultStatus.Forbidden => HandleForbidden(result),
            ResultStatus.Unavailable => HandleUnavailable(result),
            ResultStatus.CriticalError => HandleCriticalError(result),

            // Fallback
            _ => HandleUnknown(result)
        };
    }

    #region Handlers de Éxito

    private static IResult HandleOk<T>(Result<T> result)
    {
        var response = new ApiResponseDto<T>
        {
            Success = true,
            Message = result.SuccessMessage ?? "Operación exitosa",
            Data = result.Value,
            Errors = new List<string>(),
            StatusCode = StatusCodes.Status200OK,
            Timestamp = DateTime.UtcNow,
            TraceId = Activity.Current?.Id ?? HttpContextHelper.GetTraceId()
        };
        return Results.Ok(response);
    }

    private static IResult HandleCreated<T>(Result<T> result)
    {
        var response = new ApiResponseDto<T>
        {
            Success = true,
            Message = "Recurso creado exitosamente",
            Data = result.Value,
            Errors = new List<string>(),
            StatusCode = StatusCodes.Status201Created,
            Timestamp = DateTime.UtcNow,
            TraceId = Activity.Current?.Id ?? HttpContextHelper.GetTraceId()
        };

        return Results.Created(result.Location ?? string.Empty, response);
    }

    #endregion

    #region Handlers de Error con ApiResponseDto

    private static IResult HandleNotFound<T>(Result<T> result)
    {
        var response = new ApiResponseDto<T>
        {
            Success = false,
            Message = "Recurso no encontrado",
            Data = default,
            Errors = GetErrorsList(result),
            StatusCode = StatusCodes.Status404NotFound,
            Timestamp = DateTime.UtcNow,
            TraceId = Activity.Current?.Id ?? HttpContextHelper.GetTraceId()
        };

        return Results.NotFound(response);
    }

    private static IResult HandleInvalid<T>(Result<T> result)
    {
        var errors = result.ValidationErrors
            .Select(e => $"{e.Identifier}: {e.ErrorMessage}")
            .ToList();

        if (!errors.Any())
        {
            errors = GetErrorsList(result);
        }

        var response = new ApiResponseDto<T>
        {
            Success = false,
            Message = "Error de validación",
            Data = default,
            Errors = errors,
            StatusCode = StatusCodes.Status400BadRequest,
            Timestamp = DateTime.UtcNow,
            TraceId = Activity.Current?.Id ?? HttpContextHelper.GetTraceId()
        };

        return Results.BadRequest(response);
    }

    private static IResult HandleError<T>(Result<T> result)
    {
        var response = new ApiResponseDto<T>
        {
            Success = false,
            Message = IsDevelopment()
                ? "Error interno del servidor"
                : "Ha ocurrido un error inesperado",
            Data = default,
            Errors = GetErrorsList(result),
            StatusCode = StatusCodes.Status500InternalServerError,
            Timestamp = DateTime.UtcNow,
            TraceId = Activity.Current?.Id ?? HttpContextHelper.GetTraceId()
        };

        return Results.Json(response, statusCode: StatusCodes.Status500InternalServerError);
    }

    private static IResult HandleConflict<T>(Result<T> result)
    {
        var response = new ApiResponseDto<T>
        {
            Success = false,
            Message = "Conflicto de datos",
            Data = default,
            Errors = GetErrorsList(result),
            StatusCode = StatusCodes.Status409Conflict,
            Timestamp = DateTime.UtcNow,
            TraceId = Activity.Current?.Id ?? HttpContextHelper.GetTraceId()
        };

        return Results.Conflict(response);
    }

    private static IResult HandleUnauthorized<T>(Result<T> result)
    {
        var response = new ApiResponseDto<T>
        {
            Success = false,
            Message = "No autorizado",
            Data = default,
            Errors = GetErrorsList(result),
            StatusCode = StatusCodes.Status401Unauthorized,
            Timestamp = DateTime.UtcNow,
            TraceId = Activity.Current?.Id ?? HttpContextHelper.GetTraceId()
        };

        return Results.Json(response, statusCode: StatusCodes.Status401Unauthorized);
    }

    private static IResult HandleForbidden<T>(Result<T> result)
    {
        var response = new ApiResponseDto<T>
        {
            Success = false,
            Message = "Acceso denegado",
            Data = default,
            Errors = GetErrorsList(result),
            StatusCode = StatusCodes.Status403Forbidden,
            Timestamp = DateTime.UtcNow,
            TraceId = Activity.Current?.Id ?? HttpContextHelper.GetTraceId()
        };

        return Results.Json(response, statusCode: StatusCodes.Status403Forbidden);
    }

    private static IResult HandleUnavailable<T>(Result<T> result)
    {
        var response = new ApiResponseDto<T>
        {
            Success = false,
            Message = "Servicio no disponible",
            Data = default,
            Errors = GetErrorsList(result),
            StatusCode = StatusCodes.Status503ServiceUnavailable,
            Timestamp = DateTime.UtcNow,
            TraceId = Activity.Current?.Id ?? HttpContextHelper.GetTraceId()
        };

        return Results.Json(response, statusCode: StatusCodes.Status503ServiceUnavailable);
    }

    private static IResult HandleCriticalError<T>(Result<T> result)
    {
        var response = new ApiResponseDto<T>
        {
            Success = false,
            Message = IsDevelopment()
                ? "Error crítico del sistema"
                : "Ha ocurrido un error crítico. Por favor contacte al administrador.",
            Data = default,
            Errors = GetErrorsList(result),
            StatusCode = StatusCodes.Status500InternalServerError,
            Timestamp = DateTime.UtcNow,
            TraceId = Activity.Current?.Id ?? HttpContextHelper.GetTraceId()
        };

        return Results.Json(response, statusCode: StatusCodes.Status500InternalServerError);
    }

    private static IResult HandleUnknown<T>(Result<T> result)
    {
        var response = new ApiResponseDto<T>
        {
            Success = false,
            Message = IsDevelopment()
                ? "Error desconocido"
                : "Ha ocurrido un error no controlado",
            Data = default,
            Errors = GetErrorsList(result),
            StatusCode = StatusCodes.Status500InternalServerError,
            Timestamp = DateTime.UtcNow,
            TraceId = Activity.Current?.Id ?? HttpContextHelper.GetTraceId()
        };

        return Results.Json(response, statusCode: StatusCodes.Status500InternalServerError);
    }

    #endregion

    #region Métodos auxiliares

    private static List<string> GetErrorsList(Ardalis.Result.IResult result)
    {
        var errors = new List<string>();

        if (result.Errors?.Any() == true)
        {
            errors.AddRange(result.Errors);
        }

        if (result.ValidationErrors?.Any() == true)
        {
            errors.AddRange(result.ValidationErrors.Select(e => e.ErrorMessage));
        }

        return errors;
    }

    private static bool IsDevelopment()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
    }

    #endregion
}

/// <summary>
/// Helper para obtener TraceId desde HttpContext
/// </summary>
public static class HttpContextHelper
{
    private static readonly AsyncLocal<HttpContext?> _currentContext = new();

    public static void SetHttpContext(HttpContext? context) => _currentContext.Value = context;

    public static string GetTraceId()
    {
        return _currentContext.Value?.TraceIdentifier ??
               Activity.Current?.Id ??
               Guid.NewGuid().ToString();
    }
}