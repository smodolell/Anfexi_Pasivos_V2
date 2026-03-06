namespace Anfx.Pasivos.Application.Features.Catalogos.DTOs;

public class CuentaBancariaListItemDto
{
    public int Id { get; set; }
    public string Banco { get; set; } = string.Empty;
    public string CuentaBancaria { get; set; } = string.Empty;
    public string CLABE { get; set; } = string.Empty;
}
