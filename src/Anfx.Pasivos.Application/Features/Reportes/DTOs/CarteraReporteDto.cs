namespace Anfx.Pasivos.Application.Features.Reportes.DTOs;

public partial class CarteraReporteDto
{
    public string Contrato { get; set; } = string.Empty;
    public DateTime? FecVencimiento { get; set; }
    public decimal? Capital { get; set; }
    public decimal? Interes { get; set; }
    public decimal? Total { get; set; }
    public int IdFondeador { get; set; }
    public int IdContratoPasivo { get; set; }
    public int IdContratoActivo { get; set; }
}