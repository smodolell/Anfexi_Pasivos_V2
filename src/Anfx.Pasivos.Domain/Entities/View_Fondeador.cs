namespace Anfx.Pasivos.Domain.Entities;

public partial class View_Fondeador
{
    public int ID { get; set; }
    public string Titulo { get; set; }
    public int LineasCredito { get; set; }
    public int Contratos { get; set; }
    public string ClaveCuentaContable { get; set; }
}
