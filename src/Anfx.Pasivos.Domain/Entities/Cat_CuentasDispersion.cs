namespace Anfx.Pasivos.Domain.Entities;

public class Cat_CuentasDispersion
{
    public int IdCuentaDispersion { get; set; }

    public string CuentaDispersion { get; set; } = string.Empty;

    public int? IdCuentaContable { get; set; }

    public virtual ICollection<PSV_ContratoMinistracione> PSV_ContratoMinistraciones { get; set; } = new List<PSV_ContratoMinistracione>();
}
