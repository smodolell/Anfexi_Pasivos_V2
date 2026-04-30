namespace Anfx.Pasivos.Domain.Entities;

public class View_MenuRol
{
    public int RolID { get; set; }
    public int MenuID { get; set; }
    public string RutaOrder { get; set; } = string.Empty;
    public string Ruta { get; set; } = string.Empty;
    public Nullable<bool> Selected { get; set; }
}
