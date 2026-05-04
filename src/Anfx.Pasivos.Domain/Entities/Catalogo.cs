namespace Anfx.Pasivos.Domain.Entities;

public class Catalogo
{
    public Guid ID { get; set; }
    public string Tabla { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int ValueType { get; set; }
    public decimal Orden { get; set; }
    public bool Active { get; set; }
}