namespace Anfx.Pasivos.Domain.Entities;

public class Rol

{
    public int Id { get; set; }
    public string sRol { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
