namespace Anfx.Pasivos.Domain.Entities;

public class Menu 
{
    public int Id { get; set; }
    public int? MenuId_Padre { get; set; }
    public string sMenu { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string Controlador { get; set; } = string.Empty;
    public string Accion { get; set; } = string.Empty;
    public string Icono { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}
