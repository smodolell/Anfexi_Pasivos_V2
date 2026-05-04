namespace Anfx.Pasivos.Domain.Entities;

public partial class PSV_Menu
{
    public int ID { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string Controller { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public decimal Orden { get; set; }
    public int? ParentID { get; set; }
    public bool Activo { get; set; }

}
