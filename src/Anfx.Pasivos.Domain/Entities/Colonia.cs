namespace Anfx.Pasivos.Domain.Entities;

public class Colonia 
{
    public int Id { get; set; }
    public string sColonia { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Municipio { get; set; } = string.Empty;
    public string CodigoPostal { get; set; } = string.Empty;
}
