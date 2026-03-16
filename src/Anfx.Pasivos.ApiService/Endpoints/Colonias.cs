using IResult = Microsoft.AspNetCore.Http.IResult;
using Ardalis.Result.AspNetCore;
using Anfx.Pasivos.Application.Features.Colonias.DTOs;
using Anfx.Pasivos.Application.Features.Colonias.Queries;
using Anfx.Pasivos.Application.Features.Colonias.Commands;
using Anfx.Pasivos.Application.Common.Interfaces;

namespace Anfx.Pasivos.ApiService.Endpoints;

public class Colonias : EndpointGroupBase
{
    public override string? GroupName => "colonias";

    public override void Map(RouteGroupBuilder groupBuilder)
    {
        var group = groupBuilder.MapGroup("/")
           .WithTags("Colonias")
           .RequireAuthorization();

        // GET all
        group.MapGet("/all", GetAll)
            .WithName("GetAllColonias")
            .WithSummary("Obtiene todas las colonias")
            .Produces<Result<IEnumerable<ColoniaDto>>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // GET paginados
        group.MapGet("/", GetPaginados)
            .WithName("GetColoniasPaginados")
            .WithSummary("Obtiene colonias paginadas y filtradas")
            .Produces<Result<PagedResultDto<ColoniaDto>>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // GET export to Excel
        group.MapGet("/exportar", ExportToExcel)
            .WithName("ExportColonias")
            .WithSummary("Exporta colonias filtradas a Excel")
            .Produces<byte[]>(StatusCodes.Status200OK, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // GET by id
        group.MapGet("/{id}", GetById)
            .WithName("GetColoniaById")
            .WithSummary("Obtiene una colonia por ID")
            .Produces<Result<ColoniaDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // GET códigos postales
        group.MapGet("/get-codigospostales", GetCodigosPostales)
            .WithName("GetCodigosPostales")
            .WithSummary("Obtiene códigos postales que coincidan con el parámetro")
            .Produces<Result<ICollection<SelectItemDto>>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // GET colonias por código postal
        group.MapGet("/get-cols-by-cp", GetColoniasByCodigoPostal)
            .WithName("GetColoniasByCodigoPostal")
            .WithSummary("Obtiene colonias por código postal")
            .Produces<Result<ColoniaComponentDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // GET colonias por ID (component)
        group.MapGet("/get-cols-by-id/{id}", GetColoniasById)
            .WithName("GetColoniasById")
            .WithSummary("Obtiene colonias por ID")
            .Produces<Result<ColoniaComponentDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // POST create
        group.MapPost("/", Create)
            .WithName("CreateColonia")
            .WithSummary("Crea una nueva colonia")
            .Accepts<CreateColoniaDto>("application/json")
            .Produces<Result<ColoniaDto>>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // PUT update
        group.MapPut("/{id}", Update)
            .WithName("UpdateColonia")
            .WithSummary("Actualiza una colonia existente")
            .Accepts<UpdateColoniaDto>("application/json")
            .Produces<Result<ColoniaDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // DELETE
        group.MapDelete("/{id}", Delete)
            .WithName("DeleteColonia")
            .WithSummary("Elimina una colonia")
            .Produces<Result>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }

    public async Task<IResult> GetAll(
        [FromServices] IQueryMediator queryMediator)
    {
        var result = await queryMediator.QueryAsync(new GetColoniasQuery());
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetPaginados(
        [FromServices] IQueryMediator queryMediator,
        [FromQuery] string? q = null,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDir = null)
    {
        // Validación de parámetros
        if (page < 1 || size < 1)
        {
            return Result.Error("Los parámetros de paginación deben ser mayores a 0")
                .ToCustomMinimalApiResult();
        }

        var result = await queryMediator.QueryAsync(new GetColoniasPagedQuery
        {
            SearchTerm = q,
            Page = page,
            Size = size,
            SortBy = sortBy,
            SortDir = sortDir
        });

        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetById(
        [FromServices] IQueryMediator queryMediator,
        int id)
    {
        var result = await queryMediator.QueryAsync(new GetColoniaByIdQuery { Id = id });
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetCodigosPostales(
        [FromServices] IQueryMediator queryMediator,
        [FromQuery] string codigoPostal)
    {
        var result = await queryMediator.QueryAsync(new GetCodigosPostalesQuery { CodigoPostal = codigoPostal });
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetColoniasByCodigoPostal(
        [FromServices] IQueryMediator queryMediator,
        [FromQuery] string codigoPostal)
    {
        var result = await queryMediator.QueryAsync(new GetColoniasByCodigoPostalQuery { CodigoPostal = codigoPostal });
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetColoniasById(
        [FromServices] IQueryMediator queryMediator,
        int id)
    {
        var result = await queryMediator.QueryAsync(new GetColoniasByIdQuery { Id = id });
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> Create(
        [FromServices] ICommandMediator commandMediator,
        [FromBody] CreateColoniaDto createDto)
    {
        var command = new CreateColoniaCommand
        {
            sColonia = createDto.sColonia,
            Estado = createDto.Estado,
            Municipio = createDto.Municipio,
            CodigoPostal = createDto.CodigoPostal
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> Update(
        [FromServices] ICommandMediator commandMediator,
        int id,
        [FromBody] UpdateColoniaDto updateDto)
    {
        var command = new UpdateColoniaCommand
        {
            Id = id,
            sColonia = updateDto.sColonia,
            Estado = updateDto.Estado,
            Municipio = updateDto.Municipio,
            CodigoPostal = updateDto.CodigoPostal
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> Delete(
        [FromServices] ICommandMediator commandMediator,
        int id)
    {
        var result = await commandMediator.SendAsync(new DeleteColoniaCommand { Id = id });
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> ExportToExcel(
        [FromServices] IQueryMediator queryMediator,
        [FromServices] IExcelExportService excelExportService,
        [FromQuery] string? q = null)
    {
        try
        {
            var colonias = await queryMediator.QueryAsync(new GetColoniasForExportQuery
            {
                SearchTerm = q
            });

            // Verificar si el resultado es exitoso
            if (!colonias.IsSuccess)
            {
                return colonias.ToCustomMinimalApiResult();
            }

            var excelBytes = excelExportService.ExportToExcel(
                colonias.Value,
                "Colonias",
                $"colonias_{DateTime.Now:yyyyMMdd_HHmmss}"
            );

            return Results.File(
                excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"colonias_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }
        catch (Exception ex)
        {
            var result = Result.Error($"Error al generar archivo Excel: {ex.Message}");
            return result.ToCustomMinimalApiResult();
        }
    }
}