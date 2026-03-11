namespace Anfx.Pasivos.Application.Features.Configuracion.Dtos;

public class LineaCreditoListItemDto
{
    public decimal MontoAprobado { get; set; }
    public decimal MontoDispuesto { get; set; }
    public decimal MontoDisponible { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public int Contratos { get; set; }
    public int ID { get; set; }
    public int FondeadorID { get; set; }
}