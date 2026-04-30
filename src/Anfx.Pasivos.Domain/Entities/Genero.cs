namespace Anfx.Pasivos.Domain.Entities;

public class Genero
{

    public int IdGenero { get; set; }
    public string Titulo { get; set; } = string.Empty;

    public virtual ICollection<Usuario> Usuario { get; set; } = new HashSet<Usuario>();
}