namespace Anfx.Pasivos.Domain.Entities;

public partial class Catalogo
{
    public System.Guid ID { get; set; }
    public string Tabla { get; set; }
    public string Value { get; set; }
    public int ValueType { get; set; }
    public decimal Orden { get; set; }
    public bool Active { get; set; }
}