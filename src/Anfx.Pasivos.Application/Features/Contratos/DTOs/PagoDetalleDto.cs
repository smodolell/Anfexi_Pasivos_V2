namespace Anfx.Pasivos.Application.Features.Contratos.DTOs;

public class PagoDetalleDto
{
    public int IdPago { get; set; }
    public List<DetalleMovimientoPagoDto> Detalle { get; set; }
}