using Anfx.Pasivos.Application.Features.Configuracion.Dtos;
using Mapster;

namespace Anfx.Pasivos.Application.Features.Configuracion;


public class ConfiguracionMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<TipoCredito, RelLineaCreditoTipoCreditoDto>()
            .Map(o => o.TipoCredito, d => d.TipoCredito1)
            ;
    }
}
