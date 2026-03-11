namespace Anfx.Pasivos.Application.Features.Configuracion.Dtos;

public class FondeadorListItemDto
{
    public int ID { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public int LineasCredito { get; set; }
    public int Contratos { get; set; }
    public string? ClaveCuentaContable { get; set; } = string.Empty;
}