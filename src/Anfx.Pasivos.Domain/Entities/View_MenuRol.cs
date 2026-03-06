namespace Anfx.Pasivos.Domain.Entities;

public partial class View_MenuRol
{
    public int RolID { get; set; }
    public int MenuID { get; set; }
    public string RutaOrder { get; set; }
    public string Ruta { get; set; }
    public Nullable<bool> Selected { get; set; }
}
