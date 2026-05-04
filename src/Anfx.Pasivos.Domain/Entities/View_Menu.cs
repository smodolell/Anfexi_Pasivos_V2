namespace Anfx.Pasivos.Domain.Entities;

public class View_Menu
{
    public string Titulo { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public decimal Orden { get; set; }
    public string Activo { get; set; } = string.Empty;
    public int ID { get; set; }
    public int? ParentID { get; set; }
}
