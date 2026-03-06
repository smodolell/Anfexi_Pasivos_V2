using System.ComponentModel.DataAnnotations.Schema;

namespace Anfx.Pasivos.Domain.Entities;

public class PSV_TipoCredito
{
    
    public int IdTipoCredito { get; set; }
    public string TipoCredito { get; set; } = string.Empty;
    public int IdTipoMovimiento { get; set; }
    public string Prefijo { get; set; } = string.Empty;
    public string Sufijo { get; set; } = string.Empty;
    public int Contador { get; set; }
    public int IdTipoTablaAmortiza { get; set; }
    public bool Activo { get; set; }
    public int IdTipoMovimiento_Mora { get; set; }
    public int PeriodoGracia { get; set; }
    public int? IdEmpresa { get; set; }

    [ForeignKey(nameof(IdTipoMovimiento))]
    public virtual TipoMovimiento TipoMovimiento { get; set; } = null!;
    public virtual ICollection<PSV_Contrato> PSV_Contrato { get; set; } = new HashSet<PSV_Contrato>();
    public virtual PSV_TipoTablaAmortiza PSV_TipoTablaAmortiza { get; set; } = null!;
    public virtual TipoMovimiento TipoMovimiento1 { get; set; } = null!;
    public virtual ICollection<PSV_TipoTerminacion> PSV_TipoTerminacion { get; set; } = new HashSet<PSV_TipoTerminacion>();
    public virtual Empresa? Empresa { get; set; }
}
