using Anfx.Pasivos.Application.Features.Reportes.DTOs;

namespace Anfx.Pasivos.ApiService.Responces.Reportes;

public class DashboardResponse
{

    public CarteraDto Activos { get; set; }
    public CarteraDto Pasivos { get; set; }


    public List<CarteraMensualDto> ActivosMensual { get; set; }
    public List<CarteraMensualDto> PasivosMensual { get; set; }
}
