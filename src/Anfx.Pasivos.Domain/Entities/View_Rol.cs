namespace Anfx.Pasivos.Domain.Entities;

public class View_Rol
{
    public int ID { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public double? Nivel { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public bool? Activo { get; set; }
}