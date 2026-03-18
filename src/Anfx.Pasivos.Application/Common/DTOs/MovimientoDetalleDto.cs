namespace Anfx.Pasivos.Application.Common.DTOs;

public class MovimientoDetalleDto
{
    public int IdMovimiento { get; set; }
    public List<DetallePagoMovimientoDto> Detalle { get; set; } = new List<DetallePagoMovimientoDto>();
}
