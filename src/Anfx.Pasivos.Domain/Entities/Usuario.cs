namespace Anfx.Pasivos.Domain.Entities;

public class Usuario
{
    public int IdUsuario { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string UserPass { get; set; } = string.Empty;

    public DateTime? FechaRegistracion { get; set; }

    public int? IdUsuarioQRegistro { get; set; }

    public int? IdRol { get; set; }

    public string? NombreCompleto { get; set; }

    public string? Email { get; set; }

    public bool? Activo { get; set; }

    public int IdGenero { get; set; }

    public bool? EsPrimerInicio { get; set; }

    public bool? RequiereCambioPass { get; set; }

    public string? IdiomaUI { get; set; }

    public int? IdSucursal { get; set; }

    public int? IdEmpresa { get; set; }

    public bool? EsPromotor { get; set; }

    public string? ClavePromotor { get; set; }

    public int? IdTipoUsuarioCobranza { get; set; }

    public string? Telefono { get; set; }

    public string? TelefonoPromotor { get; set; }

    public bool? PerteneceAlComite { get; set; }

    public bool? AutorizaDMA { get; set; }

    public bool? EsCoordinador { get; set; }

    public bool? PermiteSeleccionarLineaCredito { get; set; }

    public bool? PermiteEdicionAltaContrato { get; set; }

    public bool? PermiteSupervisionAltaContrato { get; set; }

    public bool? PermiteAltaContrato { get; set; }

    public bool? PermiteModificacionFechaPago { get; set; }

    public virtual Rol? Rol { get; set; }


  
}
