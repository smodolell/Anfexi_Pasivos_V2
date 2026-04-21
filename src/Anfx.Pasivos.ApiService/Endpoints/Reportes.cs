using Anfx.Pasivos.ApiService.Responces.Reportes;
using Anfx.Pasivos.Application.Features.Reportes.Commands;
using Anfx.Pasivos.Application.Features.Reportes.DTOs;
using Anfx.Pasivos.Application.Features.Reportes.Queries;
using Anfx.Pasivos.Application.Features.SelectLists.Queries;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Anfx.Pasivos.ApiService.Endpoints;

public class Reportes : EndpointGroupBase
{
    public override string? GroupName => "reportes";
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        var group = groupBuilder.MapGroup("/")
            .WithTags("Reportes");

        group.MapGet("dashboard", GetDashboard)
            .WithName("GetDashboard")
            .WithSummary("Obtiene datos del dashboard de reportes")
            .WithDescription("Obtiene información de cartera activa, pasiva y otros datos para el dashboard")
            .Produces<ApiResponseDto<DashboardResponse>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);


        group.MapGet("cartera-por-vencer/{Page?}/{PageSize?}", GetCarteraPorVencer)
            .WithName("GetCarteraPorVencer")
            .WithSummary("Obtiene cartera activa por vencer con paginación")
            .WithDescription("Obtiene listado paginado de cartera activa próxima a vencer")
            .Produces<ApiResponseDto<PagedResultDto<CarteraReporteDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);


        group.MapGet("cartera-pasiva-por-vencer", GetCarteraPasivaPorVencer)
            .WithName("GetCarteraPasivaPorVencer")
            .WithSummary("Obtiene cartera pasiva por vencer con paginación")
            .Produces<ApiResponseDto<PagedResultDto<CarteraReporteDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        #region Reporte
        group.MapGet("reporte/{id:int}", GetReporteById)
            .WithName("GetReporteById")
            .WithSummary("Obtiene un reporte por ID")
            .Produces<ApiResponseDto<ReporteEditDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("reporte/", GetReportes)
            .WithSummary("Obtiene reportes paginados y filtrados")
            .Produces<ApiResponseDto<PagedResultDto<ReporteListItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("reporte/search", SearchReportes)
            .WithSummary("Busca reportes para lista de selección")
            .Produces<ApiResponseDto<List<SelectReporteDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapPost("reporte/", CreateReporte)
            .WithName("CreateReporte")
            .WithSummary("Crea un nuevo reporte")
            .Accepts<ReporteEditDto>("application/json")
            .Produces<ApiResponseDto<int>>(StatusCodes.Status201Created)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapPut("reporte/{id:int}", UpdateReporte)
            .WithName("UpdateReporte")
            .WithSummary("Actualiza un reporte")
            .Accepts<ReporteEditDto>("application/json")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapDelete("reporte/{id:int}", DeleteReporte)
            .WithName("DeleteReporte")
            .WithSummary("Elimina un reporte y sus parámetros")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("reporte/{id:int}/configuracion", GetReporteConfiguracion)
            .WithName("GetReporteConfiguracion")
            .WithSummary("Obtiene la configuración de parámetros de un reporte para su ejecución")
            .Produces<ApiResponseDto<ReporteExecuteDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapPost("reporte/ejecutar", EjecutarReporte)
            .WithName("EjecutarReporte")
            .WithSummary("Ejecuta un reporte y retorna el archivo generado (Excel o Texto)")
            .Accepts<ReporteExecuteDto>("application/json")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);


        #endregion

        #region Archivo
        group.MapGet("archivo/", GetArchivos)
            .WithSummary("Obtiene archivos generados paginados, filtrable por reporte")
            .Produces<ApiResponseDto<PagedResultDto<ArchivoListItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("archivo/{id:guid}/download", DownloadArchivo)
            .WithName("DownloadArchivo")
            .WithSummary("Descarga un archivo generado por ID")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapDelete("archivo/{id:guid}", DeleteArchivo)
            .WithName("DeleteArchivo")
            .WithSummary("Elimina un archivo del disco y su registro")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);
        #endregion

        #region Parametro
        group.MapGet("parametro/{id:guid}", GetParametroById)
            .WithName("GetParametroById")
            .WithSummary("Obtiene un parámetro por ID")
            .Produces<ApiResponseDto<ParametroEditDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("parametro/", GetParametros)
            .WithSummary("Obtiene parámetros filtrados por reporte")
            .Produces<ApiResponseDto<List<ParametroListItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapPut("parametro/{id:guid}", UpdateParametro)
            .WithName("UpdateParametro")
            .WithSummary("Actualiza la configuración de un parámetro")
            .Accepts<ParametroEditDto>("application/json")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);
        #endregion

    }

    public async Task<IResult> GetDashboard(
        [FromServices] IQueryMediator queryMediator,
        [FromQuery] int? IdFondeador = null,
        [FromQuery] int? IdContratoPasivo = null,
        [FromQuery] int? IdContratoActivo = null,
        [FromQuery] int? Saldos = null,
        CancellationToken cancellationToken = default
    )
    {


        
        var dataActivo = await queryMediator.QueryAsync(new GetCarteraActivaQuery
        {
            IdFondeador = IdFondeador,
            IdContratoPasivo = IdContratoPasivo,
            IdContratoActivo = IdContratoActivo,
            Saldos = Saldos ?? 1

        });

        var dataPasivos = await queryMediator.QueryAsync(new GetCarteraPasivaQuery
        {
            IdFondeador = IdFondeador,
            IdContratoPasivo = IdContratoPasivo,
            IdContratoActivo = IdContratoActivo,
            Saldos = Saldos ?? 1

        });

        var dataMensualActiva = await queryMediator.QueryAsync(new GetCarteraActivaMensualQuery
        {
            IdFondeador = IdFondeador,
            IdContratoPasivo = IdContratoPasivo,
            IdContratoActivo = IdContratoActivo,
            Saldos = Saldos ?? 1

        });

        var dataMensualPasiva = await queryMediator.QueryAsync(new GetCarteraPasivaMensualQuery
        {
            IdFondeador = IdFondeador,
            IdContratoPasivo = IdContratoPasivo,
            IdContratoActivo = IdContratoActivo,
            Saldos = Saldos ?? 1

        });


        var result = new DashboardResponse
        {
            Activos = dataActivo.Value,
            Pasivos = dataPasivos.Value,
            ActivosMensual = dataMensualActiva.Value,
            PasivosMensual = dataMensualPasiva.Value
        };

        return Result.Success(result)
            .ToCustomMinimalApiResult();
    }


    public async Task<IResult> GetCarteraPorVencer(
        [FromServices] IQueryMediator queryMediator,
        [FromRoute] int? Page = 1,
        [FromRoute] int? PageSize = 10,
        [FromQuery] string? SearchText = null,
        [FromQuery] int? IdFondeador = null,
        [FromQuery] int? IdContratoPasivo = null,
        [FromQuery] int? IdContratoActivo = null,
        [FromQuery] int? Saldos = null,
        [FromQuery] string SortColumn = "FecVencimiento",
        [FromQuery] bool SortDescending = false,
        CancellationToken cancellationToken = default
    )
    {
        var query = new GetCarteraActivaPorVencer
        {
            SearchText = SearchText,
            IdFondeador = IdFondeador,
            IdContratoPasivo = IdContratoPasivo,
            IdContratoActivo = IdContratoActivo,
            Saldos = Saldos ?? 1,
            Page = Page ?? 1,
            PageSize = PageSize ?? 10,
            SortColumn = SortColumn,
            SortDescending = SortDescending
        };

        var result = await queryMediator.QueryAsync(query, cancellationToken);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetCarteraPasivaPorVencer(
       [FromServices] IQueryMediator queryMediator,
       [FromQuery] int Page = 1,
       [FromQuery] int PageSize = 10,
       [FromQuery] string? SearchText = null,
       [FromQuery] int? IdFondeador = null,
       [FromQuery] int? IdContratoPasivo = null,
       [FromQuery] int? IdContratoActivo = null,
       [FromQuery] int? Saldos = null,
       [FromQuery] string SortColumn = "FecVencimiento",
       [FromQuery] bool SortDescending = false,
       CancellationToken cancellationToken = default)
    {
        var query = new GetCarteraPasivaPorVencer
        {
            SearchText = SearchText,
            IdFondeador = IdFondeador,
            IdContratoPasivo = IdContratoPasivo,
            IdContratoActivo = IdContratoActivo,
            Saldos = Saldos ?? 1,
            Page = Page,
            PageSize = PageSize,
            SortColumn = SortColumn,
            SortDescending = SortDescending
        };

        var result = await queryMediator.QueryAsync(query, cancellationToken);
        return result.ToCustomMinimalApiResult();
    }

    #region Reporte
    public async Task<IResult> GetReporteById(
        [FromServices] IQueryMediator queryMediator,
        int id)
    {
        var result = await queryMediator.QueryAsync(new GetReporteByIdQuery { ReporteId = id });
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetReportes(
        [FromServices] IQueryMediator queryMediator,
        [FromQuery] string? q = null,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string sortColumn = nameof(ReporteListItemDto.NomReporte),
        [FromQuery] bool sortDescending = false)
    {
        var result = await queryMediator.QueryAsync(new GetReportesQuery
        {
            SearchText = q,
            Page = page,
            PageSize = size,
            SortColumn = sortColumn,
            SortDescending = sortDescending
        });
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> SearchReportes(
        [FromServices] IQueryMediator queryMediator,
        [FromQuery] string? q = null)
    {
        var result = await queryMediator.QueryAsync(new SearchReportesQuery { SearchText = q });
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> CreateReporte(
        [FromServices] ICommandMediator commandMediator,
        [FromBody] ReporteEditDto model)
    {
        var result = await commandMediator.SendAsync(new CreateReporteCommand { Model = model });
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> UpdateReporte(
        [FromServices] ICommandMediator commandMediator,
        [FromRoute] int id,
        [FromBody] ReporteEditDto model)
    {
        var result = await commandMediator.SendAsync(new UpdateReporteCommand { ReporteId = id, Model = model });
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> DeleteReporte(
        [FromServices] ICommandMediator commandMediator,
        [FromRoute] int id)
    {
        var result = await commandMediator.SendAsync(new DeleteReporteCommand { ReporteId = id });
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetReporteConfiguracion(
        [FromServices] IQueryMediator queryMediator,
        int id)
    {
        var result = await queryMediator.QueryAsync(new GetReporteConfiguracionQuery { ReporteId = id });
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> EjecutarReporte(
        [FromServices] ICommandMediator commandMediator,
        [FromBody] ReporteExecuteDto model,
        [FromQuery] bool guardarArchivo = false)
    {
        var result = await commandMediator.SendAsync(new EjecutarReporteCommand { Model = model, GuardarArchivo = guardarArchivo });
        if (!result.IsSuccess)
            return result.ToCustomMinimalApiResult();

        var export = result.Value;
        return Results.File(export.Data, export.ContentType, export.FileName);
    }

    
    #endregion

    #region Parametro
    public async Task<IResult> GetParametroById(
        [FromServices] IQueryMediator queryMediator,
        Guid id)
    {
        var result = await queryMediator.QueryAsync(new GetParametroByIdQuery { ParametroId = id });
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetParametros(
        [FromServices] IQueryMediator queryMediator,
        [FromQuery] int? reporteId = null,
        [FromQuery] string? q = null)
    {
        var result = await queryMediator.QueryAsync(new GetParametrosQuery
        {
            ReporteId = reporteId,
            SearchText = q
        });
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> UpdateParametro(
        [FromServices] ICommandMediator commandMediator,
        [FromRoute] Guid id,
        [FromBody] ParametroEditDto model)
    {
        model.ParametroId = id;
        var result = await commandMediator.SendAsync(new UpdateParametroCommand { Model = model });
        return result.ToCustomMinimalApiResult();
    }
    #endregion

    #region Archivo
    public async Task<IResult> GetArchivos(
        [FromServices] IQueryMediator queryMediator,
        [FromQuery] int? reporteId = null,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string sortColumn = nameof(ArchivoListItemDto.FechaCreacion),
        [FromQuery] bool sortDescending = true)
    {
        var result = await queryMediator.QueryAsync(new GetArchivosQuery
        {
            ReporteId = reporteId,
            Page = page,
            PageSize = size,
            SortColumn = sortColumn,
            SortDescending = sortDescending
        });
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> DownloadArchivo(
        [FromServices] IQueryMediator queryMediator,
        Guid id)
    {
        var result = await queryMediator.QueryAsync(new DownloadArchivoQuery { ArchivoId = id });
        if (!result.IsSuccess)
            return result.ToCustomMinimalApiResult();

        var export = result.Value;
        return Results.File(export.Data, export.ContentType, export.FileName);
    }

    public async Task<IResult> DeleteArchivo(
        [FromServices] ICommandMediator commandMediator,
        Guid id)
    {
        var result = await commandMediator.SendAsync(new DeleteArchivoCommand { ArchivoId = id });
        return result.ToCustomMinimalApiResult();
    }
    #endregion

}
