namespace Anfx.Pasivos.Domain.Entities;

public partial class View_Menu
{
    public string Titulo { get; set; }
    public string Scope { get; set; }
    public string Icon { get; set; }
    public decimal Orden { get; set; }
    public string Activo { get; set; }
    public int ID { get; set; }
    public Nullable<int> ParentID { get; set; }
}
