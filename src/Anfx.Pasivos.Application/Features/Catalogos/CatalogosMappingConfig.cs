using Anfx.Pasivos.Application.Features.Catalogos.DTOs;
using Anfx.Pasivos.Application.Features.Configuracion.Dtos;
using Mapster;

namespace Anfx.Pasivos.Application.Features.Catalogos;

public class CatalogosMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Tasa, TasaFijaListItemDto>()
            .Map(o => o.Id, d => d.IdTasa)
            .Map(o => o.Nombre, d => d.Tasa1);

        config.NewConfig<Tasa, TasaVariableListItemDto>()
            .Map(o => o.Id, d => d.IdTasa)
            .Map(o => o.Nombre, d => d.Tasa1)
            .Map(o => o.UltimoValor, d => d.TasaValors.OrderByDescending(v => v.FecValorTasa).Select(v => v.ValorTasa).FirstOrDefault())
            .Map(o => o.FecUltimoValor, d => d.TasaValors.OrderByDescending(v => v.FecValorTasa).Select(v => v.FecValorTasa).FirstOrDefault());

        config.NewConfig<TasaValor, TasaValorListItemDto>()
            .Map(o => o.Id, d => d.IdTasaValor);

        config.NewConfig<PSV_TipoCredito, TipoCreditoListItemDto>()
            .Map(o => o.Id, d => d.IdTipoCredito);

        config.NewConfig<PSV_EstatusContrato, EstatusContratoListItemDto>()
            .Map(o => o.Id, d => d.IdEstatusContrato);

        config.NewConfig<PSV_TipoPago, TipoPagoListItemDto>()
            .Map(o => o.Id, d => d.IdTipoPago);

        config.NewConfig<PSV_TipoTablaAmortiza, TipoTablaAmortizaListItemDto>()
            .Map(o => o.Id, d => d.IdTipoTablaAmortiza);

        config.NewConfig<PSV_Banco, BancoListItemDto>()
            .Map(o => o.Id, d => d.IdBanco);

        config.NewConfig<PSV_CuentaBancaria, CuentaBancariaListItemDto>()
            .Map(o => o.Id, d => d.IdCuentaBancaria)
            .Map(o => o.Banco, d => d.PSV_Banco.Banco);

    }
}
