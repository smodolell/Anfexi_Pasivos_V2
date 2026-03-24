namespace Anfx.Pasivos.Application.Features.Contratos.DTOs;

public partial class RelActivoPasivoDto
{
    public int ID { get; set; }
    public string? Contrato { get; set; }
    public decimal? Capital { get; set; }
    public string? TipoCredito { get; set; }
    public string? Fondeador { get; set; }
    public int IdFondeador { get; set; }
    public int? IdContratoPasivo { get; set; }
    public int IdContratoPasivoLC { get; set; }
}
