using Anfx.Pasivos.Application.Features.Reportes.DTOs;
using Mapster;

namespace Anfx.Pasivos.Application.Features.Reportes;

public class ReportesMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RSP_Reporte, ReporteEditDto>()
            .Map(o => o.ReporteId, d => d.Id);

        config.NewConfig<RSP_Reporte, SelectReporteDto>()
            .Map(o => o.ReporteId, d => d.Id);

        config.NewConfig<RSP_Parametro, ParametroEditDto>()
            .Map(o => o.ParametroId, d => d.Id);

        config.NewConfig<RSP_Parametro, ParametroListItemDto>()
            .Map(o => o.NomInput, d => d.RSP_Input.NomInput);

        config.NewConfig<RSP_Archivo, ArchivoListItemDto>()
            .Map(o => o.NomReporte, d => d.RSP_Reporte.NomReporte);
    }
}
