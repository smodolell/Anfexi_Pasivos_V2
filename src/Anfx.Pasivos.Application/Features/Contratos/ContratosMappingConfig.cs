using Anfx.Pasivos.Application.Features.Contratos.DTOs;
using Mapster;

namespace Anfx.Pasivos.Application.Features.Contratos;


public class ContratosMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<PSV_Contrato, InfoGeneralContratoPasivoDto>()
            .Map(o => o.TipoCredito, d => d.PSV_TipoCredito.TipoCredito)
            .Map(o => o.TipoMoneda, d => d.SB_TipoMoneda.DescTipoMoneda)
            .Map(o => o.EstatusContrato, d => d.PSV_EstatusContrato.EstatusContrato)
            .Map(o => o.Periodicidad, d => d.SB_Periodicidad.DescPeriodicidad)
            .Map(o => o.Fondeador, d => d.PSV_Fondeador.Fondeador)
            
             ;
        
    }
}

