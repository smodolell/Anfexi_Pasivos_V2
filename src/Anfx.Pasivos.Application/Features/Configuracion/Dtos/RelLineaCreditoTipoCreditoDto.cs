namespace Anfx.Pasivos.Application.Features.Configuracion.Dtos;

public partial class RelLineaCreditoTipoCreditoDto
{
    public int IdLineaCredito { get; set; }
    public int IdTipoCredito { get; set; }
    public bool Seleccionado { get; set; }
    public string TipoCredito { get; set; }
}
