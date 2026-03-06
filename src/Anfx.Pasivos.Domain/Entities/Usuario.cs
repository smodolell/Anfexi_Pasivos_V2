namespace Anfx.Pasivos.Domain.Entities;

public class Usuario
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UsuarioNombre { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }
    public bool Activo { get; set; } = true;
    public int RolId { get; set; }
    public virtual Rol Rol { get; set; } = null!;
}
