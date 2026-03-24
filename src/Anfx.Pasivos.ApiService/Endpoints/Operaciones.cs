using Anfx.Pasivos.Application.Features.Contratos.Commands;
using Anfx.Pasivos.Application.Features.Contratos.Queries;
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


        #region Anticipo


        group.MapGet("anticipo/{contrato}", GetAnticipoByContrato)
            .WithName("GetAnticipoByContrato")
            .WithSummary("Obtiene información para anticipo por contrato pasivo")
            .WithDescription("Retorna la configuración inicial para realizar un anticipo sobre un contrato pasivo activo específico, incluyendo la fecha sugerida basada en la tabla de amortización.")
            .Produces<ApiResponseDto<AnticipoDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);


        group.MapGet("anticipo/config", GetAnticipoConfig)
            .WithName("GetAnticipoConfig")
            .WithSummary("Obtiene configuración para anticipo/liquidación de contrato")
            .WithDescription("Retorna la configuración necesaria para procesar un anticipo o liquidación de contrato basado en el tipo de terminación seleccionado, incluyendo montos, porcentajes de IVA y validaciones específicas.")
            .Produces<ApiResponseDto<AnticipoConfigDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);


        group.MapGet("anticipo/interes", GetInteres)
            .WithName("GetInteres")
            .WithSummary("Calcula el interés para un anticipo")
            .WithDescription("Calcula el monto de intereses generados por un anticipo en un contrato pasivo, basado en la fecha de corte y el monto anticipado.")
            .Produces<ApiResponseDto<decimal>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapPost("anticipo/confirmar", ConfirmarAnticipo)
            .WithName("ConfirmarAnticipo")
            .WithSummary("Confirma y procesa un anticipo o liquidación")
            .WithDescription("Registra un anticipo a capital o liquidación sobre un contrato pasivo, aplicando el procedimiento correspondiente.")
            .Accepts<AnticipoDto>("application/json")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        #endregion


        #region Caja

        group.MapGet("caja/{contrato}", GetCajaByContrato)
            .WithName("GetCajaByContrato")
            .WithSummary("Obtiene información de caja por contrato pasivo")
            .WithDescription("Retorna la configuración inicial para registrar un pago en caja para un contrato específico.")
            .Produces<ApiResponseDto<CajaDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);



        group.MapPost("caja/confirmar", ConfirmarPagoCaja)
            .WithName("ConfirmarPagoCaja")
            .WithSummary("Confirma y procesa un pago de caja")
            .WithDescription("Registra un pago en caja, aplicando los movimientos seleccionados mediante el procedimiento almacenado usp_PSV_PagarCargosAsync.")
            .Accepts<CajaDto>("application/json")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        #endregion
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

    #region Anticipo

    public async Task<IResult> GetAnticipoByContrato(
    [FromServices] IQueryMediator queryMediator,
    [FromRoute] string contrato)
    {
        var query = new GetAnticipoQuery
        {
            ContratoPasivo = contrato
        };

        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetAnticipoConfig(
    [FromServices] IQueryMediator queryMediator,
    [FromQuery] int idTipoTerminacion,
    [FromQuery] int idContrato)
    {
        var query = new GetAnticipoConfigQuery
        {
            IdTipoTerminacion = idTipoTerminacion,
            IdContrato = idContrato
        };

        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetInteres(
    [FromServices] IQueryMediator queryMediator,
    [FromQuery] int idContrato,
    [FromQuery] DateOnly fechaCorte,
    [FromQuery] decimal montoAnticipo)
    {
        var query = new GetInteresQuery
        {
            IdContrato = idContrato,
            FechaCorte = fechaCorte,
            MontoAnticipo = montoAnticipo
        };

        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> ConfirmarAnticipo(
    [FromServices] ICommandMediator commandMediator,
    [FromBody] AnticipoDto request)
    {
        var command = new AnticipoConfirmCommand
        {
            IdContrato = request.IdContrato,
            IdTipoTerminacion = request.IdTipoTerminacion,
            IdTipoReduccion = request.IdTipoReduccion,
            FechaAnticipo = request.FechaAnticipo,
            MontoAnticipo = request.MontoAnticipo,
            MontoInteres = request.MontoInteres,
            MontoPena = request.MontoPena,
            MontoIVA_Interes = request.MontoIVA_Interes,
            MontoIVA_Pena = request.MontoIVA_Pena,
            MontoTotal = request.MontoTotal,
            EsLiquidacion = request.EsLiquidacion
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }


    #endregion

    #region Caja

    public async Task<IResult> GetCajaByContrato(
        [FromServices] IQueryMediator queryMediator,
        [FromRoute] string contrato)
    {
        var query = new GetCajaQuery
        {
            ContratoPasivo = contrato
        };

        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }


    public async Task<IResult> ConfirmarPagoCaja(
        [FromServices] ICommandMediator commandMediator,
        [FromBody] CajaDto request
    )
    {
        var command = new CajaConfirmCommand
        {
            Model = request
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }


    #endregion
}
