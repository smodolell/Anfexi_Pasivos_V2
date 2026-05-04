namespace Anfx.Pasivos.Domain.Entities;

public  class Rol
{
    public int IdRol { get; set; }

    public string? Titulo { get; set; }

    public string? Descripcion { get; set; }

    public double? LevelAcceso { get; set; }

    public double? Orden { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
