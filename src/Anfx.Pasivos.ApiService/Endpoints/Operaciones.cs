using Anfx.Pasivos.Application.Features.Operaciones.Commands;
using Anfx.Pasivos.Application.Features.Operaciones.DTOs;
using Anfx.Pasivos.Application.Features.Operaciones.Queries;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Anfx.Pasivos.ApiService.Endpoints;

public class Operaciones : EndpointGroupBase
{
    public override string? GroupName => "operaciones";

    public override void Map(RouteGroupBuilder groupBuilder)
    {
        var group = groupBuilder.MapGroup("/")
         .WithTags("Operaciones");


        group.MapGet("tipo-movimiento/{id}/config", GetTipoMovimientoConfig)
            .WithName("GetTipoMovimientoConfig")
            .WithSummary("Obtiene configuración de un tipo de movimiento")
            .WithDescription("Retorna los indicadores de generación de IVA para capital e intereses de un tipo de movimiento específico.")
            .Produces<ApiResponseDto<TipoMovimientoConfigDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);


        group.MapGet("cargo-adicional", GetCargoAdicional)
         .WithName("GetCargoAdicional")
         .WithSummary("Obtiene información de cargos adicionales para un contrato pasivo")
         .WithDescription("Retorna los cargos adicionales, movimientos, saldo vencido y saldo insoluto de un contrato pasivo específico para su procesamiento.")
         .Produces<ApiResponseDto<CargoAdicionalViewDto>>(StatusCodes.Status200OK)
         .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
         .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
         .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
         .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);


        group.MapGet("cargo-adicional/nuevo/{idContrato}", GetNewCargoAdicional)
            .WithName("GetNewCargoAdicional")
            .WithSummary("Obtiene un nuevo cargo adicional con valores por defecto")
            .WithDescription("Retorna un objeto CargoAdicionalDto inicializado con valores por defecto (fecha actual e IVA 16%) para un contrato específico.")
            .Produces<ApiResponseDto<CargoAdicionalDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapPost("cargo-adicional", CreateCargoAdicional)
            .WithName("CreateCargoAdicional")
            .WithSummary("Crea un nuevo cargo adicional")
            .WithDescription("Registra un nuevo cargo adicional (movimiento) para un contrato pasivo, calculando los montos de capital, intereses e IVA correspondientes.")
            .Accepts<CargoAdicionalDto>("application/json")
            .Produces<ApiResponseDto<int>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapPut("cargo-adicional/{idMovimiento}", UpdateCargoAdicional)
            .WithName("UpdateCargoAdicional")
            .WithSummary("Actualiza un cargo adicional existente")
            .WithDescription("Modifica los datos de un cargo adicional (movimiento) existente, identificado por su ID.")
            .Accepts<CargoAdicionalDto>("application/json")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapDelete("cargo-adicional/{idMovimiento}", DeleteCargoAdicional)
            .WithName("DeleteCargoAdicional")
            .WithSummary("Elimina un cargo adicional")
            .WithDescription("Elimina lógicamente (o físicamente) un cargo adicional existente, identificado por su ID. Solo permite eliminar movimientos que no estén procesados ni tengan pagos aplicados.")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);
    }

    public async Task<IResult> GetTipoMovimientoConfig(
        [FromServices] IQueryMediator queryMediator,
        [FromRoute] int id)
    {
        var query = new GetTipoMovimientoConfigQuery
        {
            IdTipoMovimiento = id
        };

        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    #region CargoAdicional
    public async Task<IResult> GetCargoAdicional(
      [FromServices] IQueryMediator queryMediator,
      [FromQuery] string? contratoPasivo)
    {
        var query = new GetCargoAdicionalQuery
        {
            ContratoPasivo = contratoPasivo
        };

        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }


    public async Task<IResult> GetNewCargoAdicional(
        [FromServices] IQueryMediator queryMediator,
        [FromRoute] int idContrato
    )
    {
        var query = new GetNewCargoAdicionalQuery
        {
            IdContrato = idContrato
        };

        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> CreateCargoAdicional(
        [FromServices] ICommandMediator commandMediator,
        [FromBody] CargoAdicionalDto model
    )
    {
        var command = new CreateCargoAdicionalCommand
        {
            Model = model
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> UpdateCargoAdicional(
        [FromServices] ICommandMediator commandMediator,
        [FromRoute] int idMovimiento,
        [FromBody] CargoAdicionalDto model
    )
    {
        var command = new UpdateCargoAdicionalCommand
        {
            IdMovimiento = idMovimiento,
            Model = model
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> DeleteCargoAdicional(
        [FromServices] ICommandMediator commandMediator,
        [FromRoute] int idMovimiento
    )
    {
        var command = new DeleteCargoAdicionalCommand
        {
            IdMovimiento = idMovimiento
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    #endregion


}
