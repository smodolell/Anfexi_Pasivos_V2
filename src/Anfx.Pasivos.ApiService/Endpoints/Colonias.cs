//using IResult = Microsoft.AspNetCore.Http.IResult;
//using Ardalis.Result.AspNetCore;
//using Anfx.Pasivos.Application.Features.Colonias.DTOs;
//using Anfx.Pasivos.Application.Features.Colonias.Queries;
//using Anfx.Pasivos.Application.Features.Colonias.Commands;
//using Anfx.Pasivos.Application.Common.Interfaces;

//namespace Anfx.Pasivos.ApiService.Endpoints;

//public class Colonias : EndpointGroupBase
//{
//    public override string? GroupName => "colonias";

//    public override void Map(RouteGroupBuilder groupBuilder)
//    {
//        var group = groupBuilder.MapGroup("/")
//           .WithTags("Colonias");
//           //.RequireAuthorization();

        
//        group.MapGet("colonia/all", GetColoniaAll)
//            .WithName("GetAllColonias")
//            .WithSummary("Obtiene todas las colonias")
//            .Produces<ApiResponseDto<IEnumerable<ColoniaDto>>>(StatusCodes.Status200OK)
//            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
//            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

//        // GET paginados
//        group.MapGet("colonia/", GetColoniaPaginados)
//            .WithName("GetColoniasPaginados")
//            .WithSummary("Obtiene colonias paginadas y filtradas")
//            .Produces<Result<PagedResultDto<ColoniaDto>>>(StatusCodes.Status200OK)
//            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
//            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
//            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

//        // GET export to Excel
//        group.MapGet("colonia/exportar", ExportToExcelColonia)
//            .WithName("ExportColonias")
//            .WithSummary("Exporta colonias filtradas a Excel")
//            .Produces<byte[]>(StatusCodes.Status200OK, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
//            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
//            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
//            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

//        // GET by id
//        group.MapGet("colonia/{id}", GetColoniaById)
//            .WithName("GetColoniaById")
//            .WithSummary("Obtiene una colonia por ID")
//            .Produces<ApiResponseDto<ColoniaDto>>(StatusCodes.Status200OK)
//            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
//            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
//            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

//        // GET códigos postales
//        group.MapGet("colonia/get-codigospostales", GetCodigosPostales)
//            .WithName("GetCodigosPostales")
//            .WithSummary("Obtiene códigos postales que coincidan con el parámetro")
//            .Produces<ApiResponseDto<ICollection<SelectItemDto>>>(StatusCodes.Status200OK)
//            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
//            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
//            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

//        // GET colonias por código postal
//        group.MapGet("colonia/get-cols-by-cp", GetColoniasByCodigoPostal)
//            .WithName("GetColoniasByCodigoPostal")
//            .WithSummary("Obtiene colonias por código postal")
//            .Produces<Result<ColoniaComponentDto>>(StatusCodes.Status200OK)
//            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
//            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
//            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
//            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

//        // GET colonias por ID (component)
//        group.MapGet("colonia/get-cols-by-id/{id}", GetColoniasById)
//            .WithName("GetColoniasById")
//            .WithSummary("Obtiene colonias por ID")
//            .Produces<ApiResponseDto<ColoniaComponentDto>>(StatusCodes.Status200OK)
//            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
//            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
//            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

//        // POST create
//        group.MapPost("colonia/", CreateColonia)
//            .WithName("CreateColonia")
//            .WithSummary("Crea una nueva colonia")
//            .Accepts<CreateColoniaDto>("application/json")
//            .Produces<ApiResponseDto<ColoniaDto>>(StatusCodes.Status201Created)
//            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
//            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
//            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

//        // PUT update
//        group.MapPut("colonia/{id}", UpdateColonia)
//            .WithName("UpdateColonia")
//            .WithSummary("Actualiza una colonia existente")
//            .Accepts<UpdateColoniaDto>("application/json")
//            .Produces<ApiResponseDto<ColoniaDto>>(StatusCodes.Status200OK)
//            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
//            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
//            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
//            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

//        // DELETE
//        group.MapDelete("colonia/{id}", DeleteColonia)
//            .WithName("DeleteColonia")
//            .WithSummary("Elimina una colonia")
//            .Produces<ApiResponseDto>(StatusCodes.Status200OK)
//            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
//            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
//            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);
//    }

//    public async Task<IResult> GetColoniaAll(
//        [FromServices] IQueryMediator queryMediator)
//    {
//        var result = await queryMediator.QueryAsync(new GetColoniasQuery());
//        return result.ToCustomMinimalApiResult();
//    }

//    public async Task<IResult> GetColoniaPaginados(
//        [FromServices] IQueryMediator queryMediator,
//        [FromQuery] string? q = null,
//        [FromQuery] int page = 1,
//        [FromQuery] int size = 10,
//        [FromQuery] string? sortBy = null,
//        [FromQuery] string? sortDir = null)
//    {
//        // Validación de parámetros
//        if (page < 1 || size < 1)
//        {
//            return Result.Error("Los parámetros de paginación deben ser mayores a 0")
//                .ToCustomMinimalApiResult();
//        }

//        var result = await queryMediator.QueryAsync(new GetColoniasPagedQuery
//        {
//            SearchTerm = q,
//            Page = page,
//            Size = size,
//            SortBy = sortBy,
//            SortDir = sortDir
//        });

//        return result.ToCustomMinimalApiResult();
//    }

//    public async Task<IResult> GetColoniaById(
//        [FromServices] IQueryMediator queryMediator,
//        int id)
//    {
//        var result = await queryMediator.QueryAsync(new GetColoniaByIdQuery { Id = id });
//        return result.ToCustomMinimalApiResult();
//    }

//    public async Task<IResult> GetCodigosPostales(
//        [FromServices] IQueryMediator queryMediator,
//        [FromQuery] string codigoPostal)
//    {
//        var result = await queryMediator.QueryAsync(new GetCodigosPostalesQuery { CodigoPostal = codigoPostal });
//        return result.ToCustomMinimalApiResult();
//    }

//    public async Task<IResult> GetColoniasByCodigoPostal(
//        [FromServices] IQueryMediator queryMediator,
//        [FromQuery] string codigoPostal)
//    {
//        var result = await queryMediator.QueryAsync(new GetColoniasByCodigoPostalQuery { CodigoPostal = codigoPostal });
//        return result.ToCustomMinimalApiResult();
//    }

//    public async Task<IResult> GetColoniasById(
//        [FromServices] IQueryMediator queryMediator,
//        int id)
//    {
//        var result = await queryMediator.QueryAsync(new GetColoniasByIdQuery { Id = id });
//        return result.ToCustomMinimalApiResult();
//    }

//    public async Task<IResult> CreateColonia(
//        [FromServices] ICommandMediator commandMediator,
//        [FromBody] CreateColoniaDto createDto)
//    {
//        var command = new CreateColoniaCommand
//        {
//            sColonia = createDto.sColonia,
//            Estado = createDto.Estado,
//            Municipio = createDto.Municipio,
//            CodigoPostal = createDto.CodigoPostal
//        };

//        var result = await commandMediator.SendAsync(command);
//        return result.ToCustomMinimalApiResult();
//    }

//    public async Task<IResult> UpdateColonia(
//        [FromServices] ICommandMediator commandMediator,
//        int id,
//        [FromBody] UpdateColoniaDto updateDto)
//    {
//        var command = new UpdateColoniaCommand
//        {
//            Id = id,
//            sColonia = updateDto.sColonia,
//            Estado = updateDto.Estado,
//            Municipio = updateDto.Municipio,
//            CodigoPostal = updateDto.CodigoPostal
//        };

//        var result = await commandMediator.SendAsync(command);
//        return result.ToCustomMinimalApiResult();
//    }

//    public async Task<IResult> DeleteColonia(
//        [FromServices] ICommandMediator commandMediator,
//        int id)
//    {
//        var result = await commandMediator.SendAsync(new DeleteColoniaCommand { Id = id });
//        return result.ToCustomMinimalApiResult();
//    }

//    public async Task<IResult> ExportToExcelColonia(
//        [FromServices] IQueryMediator queryMediator,
//        [FromServices] IExcelExportService excelExportService,
//        [FromQuery] string? q = null)
//    {
//        try
//        {
//            var colonias = await queryMediator.QueryAsync(new GetColoniasForExportQuery
//            {
//                SearchTerm = q
//            });

//            // Verificar si el resultado es exitoso
//            if (!colonias.IsSuccess)
//            {
//                return colonias.ToCustomMinimalApiResult();
//            }

//            var excelBytes = excelExportService.ExportToExcel(
//                colonias.Value,
//                "Colonias",
//                $"colonias_{DateTime.Now:yyyyMMdd_HHmmss}"
//            );

//            return Results.File(
//                excelBytes,
//                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
//                $"colonias_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
//        }
//        catch (Exception ex)
//        {
//            var result = Result.Error($"Error al generar archivo Excel: {ex.Message}");
//            return result.ToCustomMinimalApiResult();
//        }
//    }
//}