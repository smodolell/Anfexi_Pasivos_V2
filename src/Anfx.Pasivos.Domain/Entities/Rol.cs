namespace Anfx.Pasivos.Domain.Entities;

//public class Rol

//{
//    public int Id { get; set; }
//    public string sRol { get; set; } = string.Empty;
//    public string Descripcion { get; set; } = string.Empty;
//    public bool Activo { get; set; } = true;
//    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
//}



public  class Rol
{
    public int IdRol { get; set; }

    public string? Titulo { get; set; }

    public string? Descripcion { get; set; }

    public double? LevelAcceso { get; set; }

    public double? Orden { get; set; }

    //public bool TodasLasSucursales { get; set; }

    //public bool TodasLasEmpresas { get; set; }

    //public bool PermiteBuscarPorClienteCaja { get; set; }

    //public bool PermiteSeleccMovimientoAPagar { get; set; }

    //public bool PermitePersonaRelacionadaReg { get; set; }

    //public bool PermiteGrupoRiesgo { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
