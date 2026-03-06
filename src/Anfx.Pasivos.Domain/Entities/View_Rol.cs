namespace Anfx.Pasivos.Domain.Entities;

public partial class View_Rol
{
    public int ID { get; set; }
    public string Titulo { get; set; }
    public Nullable<double> Nivel { get; set; }
    public string Descripcion { get; set; }
    public Nullable<bool> Activo { get; set; }
}