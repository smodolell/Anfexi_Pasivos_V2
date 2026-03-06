using Anfx.Pasivos.Application.Common.Interfaces;
using Anfx.Pasivos.Application.Features.TipoDirecciones.Commands;
using Anfx.Pasivos.Application.Features.TipoDirecciones.DTOs;
using Anfx.Pasivos.Application.Features.TipoDirecciones.Queries;
using IResult = Microsoft.AspNetCore.Http.IResult;


namespace Anfx.Pasivos.ApiService.Endpoints;

public class TiposDirecciones : EndpointGroupBase
{
    public override string? GroupName => "tiposdirecciones";

    public override void Map(RouteGroupBuilder groupBuilder)
    {
        var group = groupBuilder.MapGroup("/")
           .WithTags("TiposDirecciones")
           .RequireAuthorization(); 

        // GET all
        group.MapGet("/all", GetAll)
            .WithName("GetAllTiposDirecciones")
            .WithSummary("Obtiene todos los tipos de dirección")
            .Produces<Result<IEnumerable<TipoDireccionDto>>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // GET paginados
        group.MapGet("/", GetPaginados)
            .WithName("GetTiposDireccionesPaginados")
            .WithSummary("Obtiene tipos de dirección paginados y filtrados")
            .Produces<Result<PagedResultDto<TipoDireccionDto>>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // GET by id
        group.MapGet("/{id}", GetById)
            .WithName("GetTipoDireccionById")
            .WithSummary("Obtiene un tipo de dirección por ID")
            .Produces<Result<TipoDireccionDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // POST create
        group.MapPost("/", Create)
            .WithName("CreateTipoDireccion")
            .WithSummary("Crea un nuevo tipo de dirección")
            .Accepts<CreateTipoDireccionDto>("application/json")
            .Produces<Result<TipoDireccionDto>>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // PUT update
        group.MapPut("/{id}", Update)
            .WithName("UpdateTipoDireccion")
            .WithSummary("Actualiza un tipo de dirección existente")
            .Accepts<UpdateTipoDireccionDto>("application/json")
            .Produces<Result<TipoDireccionDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // DELETE
        group.MapDelete("/{id}", Delete)
            .WithName("DeleteTipoDireccion")
            .WithSummary("Elimina un tipo de dirección")
            .Produces<Result>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // GET export to Excel
        group.MapGet("/exportar", ExportToExcel)
            .WithName("ExportTiposDirecciones")
            .WithSummary("Exporta tipos de direcciones filtrados a Excel")
            .Produces<byte[]>(StatusCodes.Status200OK, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }

    public async Task<IResult> GetAll(
        [FromServices] IQueryMediator queryMediator)
    {

        var result = await queryMediator.QueryAsync(new GetTipoDireccionesQuery());
        return result.ToCustomMinimalApiResult();

    }

    public async Task<IResult> GetPaginados(
        [FromServices] IQueryMediator queryMediator,
        [FromQuery] string? q = null,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10)
    {

        var result = await queryMediator.QueryAsync(new GetTipoDireccionesPagedQuery
        {
            SearchTerm = q,
            Page = page,
            Size = size
        });

        return result.ToCustomMinimalApiResult();

    }

    public async Task<IResult> GetById(
        [FromServices] IQueryMediator queryMediator,
        int id)
    {
        var result = await queryMediator.QueryAsync(new GetTipoDireccionByIdQuery { Id = id });
        return result.ToCustomMinimalApiResult();

    }

    public async Task<IResult> Create(
        [FromServices] ICommandMediator commandMediator,
        [FromBody] CreateTipoDireccionDto createDto)
    {
        var command = new CreateTipoDireccionCommand
        {
            sTipoDireccion = createDto.sTipoDireccion
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> Update(
        [FromServices] ICommandMediator commandMediator,
        int id,
        [FromBody] UpdateTipoDireccionDto updateDto)
    {
        var command = new UpdateTipoDireccionCommand
        {
            Id = id,
            sTipoDireccion = updateDto.sTipoDireccion
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> Delete(
        [FromServices] ICommandMediator commandMediator,
        int id)
    {
        var result = await commandMediator.SendAsync(new DeleteTipoDireccionCommand { Id = id });
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> ExportToExcel(
        [FromServices] IQueryMediator queryMediator,
        [FromServices] IExcelExportService excelExportService,
        [FromQuery] string? q = null)
    {
        try
        {
            var tiposDirecciones = await queryMediator.QueryAsync(new GetTiposDireccionesForExportQuery
            {
                SearchTerm = q
            });

            var excelBytes = excelExportService.ExportToExcel(
                tiposDirecciones,
                "TiposDirecciones",
                $"tipos_direcciones_{DateTime.Now:yyyyMMdd_HHmmss}"
            );

            return Results.File(
                excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"tipos_direcciones_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }
        catch (Exception ex)
        {
            var result = Result.Error($"Error al generar archivo Excel {ex.Message}");
            return result.ToCustomMinimalApiResult();
        }
    }
}
