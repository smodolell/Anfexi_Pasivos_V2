namespace Anfx.Pasivos.Domain.Entities;

public partial class Genero
{
    public Genero()
    {
        this.Usuario = new HashSet<Usuario>();
    }

    public int IdGenero { get; set; }
    public string Titulo { get; set; }

    public virtual ICollection<Usuario> Usuario { get; set; }
}