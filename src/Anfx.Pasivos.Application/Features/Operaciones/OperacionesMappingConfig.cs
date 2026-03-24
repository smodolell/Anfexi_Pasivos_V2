using Anfx.Pasivos.Application.Features.Operaciones.DTOs;
using Mapster;

namespace Anfx.Pasivos.Application.Features.Operaciones;

public class OperacionesMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<PSV_Contrato, CargoAdicionalViewDto>()
            .Map(o => o.IdContratoPasivo, d => d.IdContrato)
            .Map(o => o.TipoCredito, d => d.PSV_TipoCredito.TipoCredito)
            .Map(o => o.PSV_EstatusContrato, d => d.PSV_EstatusContrato.EstatusContrato)
            .Map(o => o.Periodicidad, d => d.SB_Periodicidad.DescPeriodicidad)
            .Map(o => o.TipoMoneda, d => d.SB_TipoMoneda.DescTipoMoneda)
            .Map(o => o.Fondeador, d => d.PSV_Fondeador.Fondeador)
            ;
    }
}
