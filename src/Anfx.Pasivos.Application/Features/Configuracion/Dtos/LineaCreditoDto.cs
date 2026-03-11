namespace Anfx.Pasivos.Application.Features.Configuracion.Dtos;

public class LineaCreditoDto
{
    public int IdLineaCredito { get; set; }
    public bool? TipoTasa { get; set; }
    public string Fondeador { get; set; } = string.Empty;
    public int IdFondeador { get; set; }
    public int IdMoneda { get; set; }
    public string Moneda { get; set; } = string.Empty;
    public decimal MontoAprobado { get; set; }
    public decimal MontoDispuesto { get; set; }
    public decimal MontoDisponible { get; set; }
    public decimal MontoRevolvente { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public DateTime? FechaUltimaDisposicion { get; set; }
    public DateTime? FechaMaxDisposicion { get; set; }
    public DateTime? FechaAmpliacion { get; set; }
    public int NoDisposiciones { get; set; }
    public int PlazoMaximo { get; set; }
    public bool EsRevolvente { get; set; }
    public bool Activo { get; set; }
    public int? IdTasa { get; set; }
    public decimal? Tasa { get; set; }
}